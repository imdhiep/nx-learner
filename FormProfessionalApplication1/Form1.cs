using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing;
using NXOpen;
using NXOpen.UF;

namespace PartList
{

    #region File: Form.cs
    public partial class MainForm : Form
    {
        private Dictionary<string, List<string>> partNosByFilter;
        private IPartListService _partListService;
        private NXProcess _nxHandler;
        private ILoggingService _logger;

        public MainForm()
        {
            InitializeComponent();
            _logger = new LoggingService(lbStatus);
            _nxHandler = new NXProcess(_logger);
            _partListService = new PartListService();
            LoadPartNosFromXML();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Chỉnh vị trí Form xuất hiện
            this.Location = new System.Drawing.Point(this.Location.X - 600, this.Location.Y - 150);
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy session và workPart từ NX
                NXOpen.Session session = NXProcess.theSession;
                NXOpen.Part workPart = NXProcess.workPart;

                string partNo = cbPartNo.Text;
                string partName = cbPartName.Text;
                string quantity = cbQuantity.Text;
                string size = cbSize.Text;
                string material = cbMaterial.Text;
                string remarks = cbRemarks.Text;
                string image = txtImageLink.Text;

                if (rbtnRemoveTag.Checked)
                {
                    // Nếu Remove Tag được chọn, xóa phần tử Part có PartNo tương ứng
                    _partListService.RemovePart(partNo);
                    LoadPartNosFromXML();
                }
                else
                {
                    if (rbtnBodyAssign.Checked)
                    {
                        // Ẩn Form khi thực hiện Body Assign, sau đó hiện lại khi xong
                        this.Hide();
                        string bbSize = _nxHandler.BodyAssign(partNo, partName);
                        cbSize.Text = bbSize;
                        this.Show();
                    }
                    if (rbtnScreenShot.Checked)
                    {
                        string capturedImage;
                        _nxHandler.CaptureImage(partNo, partName, size, out capturedImage);
                        txtImageLink.Text = capturedImage;
                    }
                    if (rbtnWriteXML.Checked)
                    {
                        Part newPart = new Part(partNo, partName, quantity, size, material, remarks, image);
                        _partListService.UpdatePart(newPart);
                        LoadPartNosFromXML();
                    }
                }
                lbStatus.Text = "Operation Completed!";
                CycleRadioButtons();
            }
            catch (Exception)
            {
                lbStatus.Text = "Operation Failed!";
            }
        }

        private void CycleRadioButtons()
        {
            // Xoay vòng qua 3 radio button: Body Assign -> Screen Shot -> Write XML -> Body Assign
            if (rbtnBodyAssign.Checked)
            {
                rbtnBodyAssign.Checked = false;
                rbtnScreenShot.Checked = true;
            }
            else if (rbtnScreenShot.Checked)
            {
                rbtnScreenShot.Checked = false;
                rbtnWriteXML.Checked = true;
            }
            else if (rbtnWriteXML.Checked)
            {
                rbtnWriteXML.Checked = false;
                rbtnBodyAssign.Checked = true;
            }
        }

        private void cbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilter = cbFilter.SelectedItem.ToString();
            if (selectedFilter == "All")
            {
                UpdatePartNoComboBox("All");
            }
            else
            {
                UpdatePartNoComboBox(selectedFilter);
            }
        }

        private void LoadPartNosFromXML()
        {
            string folderPath = Path.GetDirectoryName(Session.GetSession().Parts.Work.FullPath);
            string xmlFilePath = Path.Combine(folderPath, "xmlList.xml");

            if (File.Exists(xmlFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                partNosByFilter = new Dictionary<string, List<string>>();
                List<string> allPartNos = new List<string>();

                foreach (XmlNode part in doc.GetElementsByTagName("Part"))
                {
                    XmlNode partNoNode = part["PartNo"];
                    if (partNoNode != null)
                    {
                        string partNo = partNoNode.InnerText;
                        allPartNos.Add(partNo);

                        if (partNo.Length >= 4)
                        {
                            string filterKey = partNo.Substring(0, 2);
                            if (!partNosByFilter.ContainsKey(filterKey))
                            {
                                partNosByFilter[filterKey] = new List<string>();
                            }
                            partNosByFilter[filterKey].Add(partNo);
                        }
                    }
                }
                allPartNos.Sort();
                partNosByFilter["All"] = allPartNos;

                cbFilter.Items.Clear();
                List<string> filterKeys = new List<string>(partNosByFilter.Keys);
                filterKeys.Sort();

                cbFilter.Items.Add("All");
                foreach (string filter in filterKeys)
                {
                    if (filter != "All")
                    {
                        cbFilter.Items.Add(filter);
                    }
                }
                UpdatePartNoComboBox("All");
            }
            else
            {
                //lbStatus.Text = "Không thấy file XML";
            }
        }

        private void UpdatePartNoComboBox(string filterKey)
        {
            cbPartNo.Items.Clear();
            cbPartNo.Items.Add("New Part No");
            if (partNosByFilter.ContainsKey(filterKey))
            {
                List<string> partNos = partNosByFilter[filterKey];
                partNos.Sort();
                foreach (string partNo in partNos)
                {
                    cbPartNo.Items.Add(partNo);
                }
            }
        }

        private void cbPartNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string partNo = cbPartNo.SelectedItem.ToString();
                if (partNo == "New Part No")
                {
                    cbPartName.Text = "";
                    cbQuantity.Text = "";
                    cbSize.Text = "";
                    cbMaterial.Text = "";
                    cbRemarks.Text = "";
                    pictureBox.Image = null;
                    lbStatus.Text = "";
                    txtImageLink.Text = "";
                    return;
                }

                XmlDocument doc = new XmlDocument();
                string folderPath = Path.GetDirectoryName(Session.GetSession().Parts.Work.FullPath);
                string xmlFilePath = Path.Combine(folderPath, "xmlList.xml");

                if (File.Exists(xmlFilePath))
                {
                    doc.Load(xmlFilePath);
                    bool partFound = false;
                    foreach (XmlNode part in doc.GetElementsByTagName("Part"))
                    {
                        if (part["PartNo"] != null && part["PartNo"].InnerText == partNo)
                        {
                            partFound = true;
                            cbPartName.Text = part["PartName"].InnerText;
                            cbQuantity.Text = part["Quantity"].InnerText;
                            cbSize.Text = part["Size"].InnerText;
                            cbMaterial.Text = part["Material"].InnerText;
                            cbRemarks.Text = part["Remark"].InnerText;
                            string imageFilePath = part["Image"].InnerText;
                            txtImageLink.Text = imageFilePath;
                            break;
                        }
                    }
                    if (!partFound)
                    {
                        lbStatus.Text = "PartNo not found.";
                    }
                }
                else
                {
                    MessageBox.Show("XML file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lbStatus.Text = "Error: " + ex.Message;
            }
        }

        private void txtImageLink_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtImageLink.Text))
            {
                pictureBox.Image = System.Drawing.Image.FromFile(txtImageLink.Text);
            }
            else
            {
                pictureBox.Image = null;
            }
        }
    }
    #endregion

 
}


