using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using NXOpen;
using NXOpen.UF;
using System.Threading.Tasks;

namespace ProfessionalFormApplication1
{
    public partial class MainForm : Form
    {
        private Dictionary<string, List<string>> partNosByFilter;
        private IPartListService _partListService;
        private NXHandler _nxHandler;
        private ILoggingService _logger;

        public MainForm()
        {
            InitializeComponent();
            this.Shown += new EventHandler(MainForm_Shown);
            _logger = new LoggingService(lbStatus);
            _nxHandler = new NXHandler(_logger);
            _partListService = new PartListService();
            LoadPartNosFromXML();
        }

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            await Task.Run(delegate
            {
                Program.theSession = Session.GetSession();
                Program.workPart = Program.theSession.Parts.Work;
            });
            lbStatus.Text = "Không thấy file XML";
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                string partNo = cbPartNo.Text;
                string partName = cbPartName.Text;
                string quantity = cbQuantity.Text;
                string size = cbSize.Text;
                string material = cbMaterial.Text;
                string remarks = cbRemarks.Text;
                string image = txtImageLink.Text;

                if (rbtnRemoveTag.Checked)
                {
                    _partListService.RemovePart(partNo);
                    LoadPartNosFromXML();
                }
                else
                {
                    if (rbtnBodyAssign.Checked)
                    {
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
                    if (rbtnAutoWrite.Checked)
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
            if (rbtnBodyAssign.Checked)
            {
                rbtnBodyAssign.Checked = false;
                rbtnScreenShot.Checked = true;
            }
            else if (rbtnScreenShot.Checked)
            {
                rbtnScreenShot.Checked = false;
                rbtnAutoWrite.Checked = true;
            }
            else if (rbtnAutoWrite.Checked)
            {
                rbtnAutoWrite.Checked = false;
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
                    string partNo = (part["PartNo"] != null ? part["PartNo"].InnerText : "");
                    if (!string.IsNullOrEmpty(partNo))
                    {
                        allPartNos.Add(partNo);
                        if (partNo.Length >= 4)
                        {
                            string filterKey = partNo.Substring(0, 2);
                            if (!partNosByFilter.ContainsKey(filterKey))
                            {
                                partNosByFilter.Add(filterKey, new List<string>());
                            }
                            partNosByFilter[filterKey].Add(partNo);
                        }
                    }
                }
                allPartNos.Sort();
                partNosByFilter["All"] = allPartNos;

                cbFilter.Items.Clear();
                // Thay vì dùng LINQ, sử dụng vòng lặp để chuyển các key sang List
                List<string> filterKeys = new List<string>();
                foreach (string key in partNosByFilter.Keys)
                {
                    filterKeys.Add(key);
                }
                filterKeys.Sort();

                cbFilter.Items.Add("All");
                for (int i = 0; i < filterKeys.Count; i++)
                {
                    if (filterKeys[i] != "All")
                    {
                        cbFilter.Items.Add(filterKeys[i]);
                    }
                }
                UpdatePartNoComboBox("All");
            }
            else
            {
                lbStatus.Text = "Không thấy file XML";
            }
        }

        private void UpdatePartNoComboBox(string filterKey)
        {
            cbPartNo.Items.Clear();
            cbPartNo.Items.Add("New Part No");
            if (partNosByFilter != null && partNosByFilter.ContainsKey(filterKey))
            {
                List<string> partNos = partNosByFilter[filterKey];
                partNos.Sort();
                for (int i = 0; i < partNos.Count; i++)
                {
                    cbPartNo.Items.Add(partNos[i]);
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

                string folderPath = Path.GetDirectoryName(Session.GetSession().Parts.Work.FullPath);
                string xmlFilePath = Path.Combine(folderPath, "xmlList.xml");

                if (File.Exists(xmlFilePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlFilePath);
                    bool partFound = false;
                    foreach (XmlNode part in doc.GetElementsByTagName("Part"))
                    {
                        if (part["PartNo"] != null && part["PartNo"].InnerText == partNo)
                        {
                            partFound = true;
                            cbPartName.Text = (part["PartName"] != null ? part["PartName"].InnerText : "");
                            cbQuantity.Text = (part["Quantity"] != null ? part["Quantity"].InnerText : "");
                            cbSize.Text = (part["Size"] != null ? part["Size"].InnerText : "");
                            cbMaterial.Text = (part["Material"] != null ? part["Material"].InnerText : "");
                            cbRemarks.Text = (part["Remark"] != null ? part["Remark"].InnerText : "");
                            string imageFilePath = (part["Image"] != null ? part["Image"].InnerText : "");
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
                try
                {
                    pictureBox.Image = Image.FromFile(txtImageLink.Text);
                }
                catch (Exception)
                {
                    pictureBox.Image = null;
                }
            }
            else
            {
                pictureBox.Image = null;
            }
        }
    }
}

