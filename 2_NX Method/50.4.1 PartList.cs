using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing;
using NXOpen;
using NXOpen.UF;

#region Logging : 
// Interface Logging
public interface ILoggingService
{
    void Log(string message);
}

// Logging Service (write Console or Status Label)
public class LoggingService : ILoggingService
{
    private Label _statusLabel;

    public LoggingService(Label statusLabel)
    {
        _statusLabel = statusLabel;
    }

    public void Log(string message)
    {
        if (_statusLabel != null)
        {
            _statusLabel.Text = message;
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}
#endregion

#region NX Open Handler: Các phương thức xử lý NX (BodyAssign, CaptureImage, SelectBody)
public class NXHandler
{
    private static Session theSession;
    private static UFSession theUfSession;
    private static NXOpen.Part workPart;
    private readonly ILoggingService _logger;

    public NXHandler(ILoggingService logger)
    {
        _logger = logger;
        try
        {
            theSession = Session.GetSession();
            theUfSession = UFSession.GetUFSession();
            workPart = theSession.Parts.Work;
            _logger.Log("Session Already... ");
        }
        catch (Exception ex)
        {
            _logger.Log("Lỗi khởi tạo NX Session: " + ex.Message);
        }
    }

    /// <summary>
    /// BodyAssign: Đổi tên các body được chọn theo chuỗi PartNo-PartName,
    /// tính bounding box và trả về chuỗi kích thước dạng "LxWxH".
    /// </summary>
    public string BodyAssign(string partNo, string partName)
    {
        try
        {
            UI ui = UI.GetUI();
            NXObject[] selectedObjects = SelectBody(ui);
            if (selectedObjects != null && selectedObjects.Length > 0)
            {
                string baseName = partNo + "-" + partName;
                int index = 1;
                foreach (NXObject obj in selectedObjects)
                {
                    ObjectGeneralPropertiesBuilder opBuilder =
                        workPart.PropertiesManager.CreateObjectGeneralPropertiesBuilder(new NXObject[] { obj });
                    opBuilder.Name = baseName + " [" + index.ToString() + "]";
                    opBuilder.Commit();
                    opBuilder.Destroy();
                    index++;
                }

                // Lấy bounding box của đối tượng đầu tiên
                UFSession ufSession = UFSession.GetUFSession();
                NXOpen.Tag csys = NXOpen.Tag.Null;
                double[] min_corner = new double[3];
                double[,] directions = new double[3, 3];
                double[] distances = new double[3];

                ufSession.Csys.AskWcs(out csys);
                ufSession.Modl.AskBoundingBoxExact(selectedObjects[0].Tag, csys, min_corner, directions, distances);

                Array.Sort(distances);
                Array.Reverse(distances);
                string sizeFormatted = String.Format("{0:F1}x{1:F1}x{2:F1}", distances[0], distances[1], distances[2]);
                _logger.Log("BodyAssign thành công. Kích thước: " + sizeFormatted);
                return sizeFormatted;
            }
            else
            {
                _logger.Log("No body selected!");
                return "";
            }
        }
        catch (Exception ex)
        {
            _logger.Log("Body Assign Failed: " + ex.Message);
            return "";
        }
    }

    /// <summary>
    /// CaptureImage: Chụp ảnh màn hình của phần được hiển thị và lưu vào thư mục "Pic".
    /// Trả về đường dẫn ảnh thông qua out parameter.
    /// </summary>
    public void CaptureImage(string partNo, string partName, string size, out string imagePath)
    {
        imagePath = "";
        try
        {
            if (partNo == "Part No" || partNo == "New Part No")
            {
                _logger.Log("Capture Fail: Nhập PartNo !");
                return;
            }

            string fullFilePath = workPart.FullPath;
            string folderPath = Path.GetDirectoryName(fullFilePath);
            string picFolderPath = Path.Combine(folderPath, "Pic");

            if (!Directory.Exists(picFolderPath))
            {
                Directory.CreateDirectory(picFolderPath);
            }

            string imageFileName = partNo;
            if (!string.IsNullOrEmpty(partName))
            {
                imageFileName += "-" + partName;
            }
            imageFileName += "-" + DateTime.Now.ToString("yyMMdd_HH-mm-ss") + ".jpg";
            imagePath = Path.Combine(folderPath, "Pic", imageFileName);

            UFSession ufSession = UFSession.GetUFSession();
            ufSession.Disp.CreateImage(imagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

            _logger.Log("Capture Successful!");
        }
        catch (Exception ex)
        {
            _logger.Log("Capture Fail: " + ex.Message);
        }
    }

    /// <summary>
    /// SelectBody: Cho phép người dùng chọn body từ giao diện NX.
    /// </summary>
    private static NXObject[] SelectBody(UI ui)
    {
        NXObject[] objects = null;
        Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1];
        maskArray[0].Type = NXOpen.UF.UFConstants.UF_solid_type;
        maskArray[0].Subtype = 0;
        maskArray[0].SolidBodySubtype = NXOpen.UF.UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY;

        Selection.Response response = ui.SelectionManager.SelectObjects(
            "Select body",
            "Select a Body",
            Selection.SelectionScope.AnyInAssembly,
            Selection.SelectionAction.ClearAndEnableSpecific,
            false,
            false,
            maskArray,
            out objects);

        if (response == Selection.Response.Ok)
            return objects;
        return null;
    }
}
#endregion

#region Data Model & Part List Service: Xử lý dữ liệu Part và danh sách Part (XML-based)
public class Part
{
    public string PartNo { get; set; }
    public string PartName { get; set; }
    public string Quantity { get; set; }  // Ví dụ "6EA"
    public string Size { get; set; }
    public string Material { get; set; }
    public string Remarks { get; set; }
    public string ImagePath { get; set; }

    public Part(string partNo, string partName, string quantity, string size, string material, string remarks, string imagePath)
    {
        PartNo = partNo;
        PartName = partName;
        Quantity = quantity;
        Size = size;
        Material = (material.Trim() == "0" ? "" : material);
        Remarks = remarks;
        ImagePath = imagePath;
    }
}

public interface IPartListService
{
    List<Part> LoadParts();
    void UpdatePart(Part part);
    void RemovePart(string partNo);
}

public class PartListService : IPartListService
{
    private readonly string xmlFilePath;

    public PartListService()
    {
        Session session = Session.GetSession();
        string partFullPath = session.Parts.Work.FullPath;
        string folderPath = Path.GetDirectoryName(partFullPath);
        xmlFilePath = Path.Combine(folderPath, "xmlList.xml");
    }

    public List<Part> LoadParts()
    {
        List<Part> parts = new List<Part>();
        if (File.Exists(xmlFilePath))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            XmlNodeList partNodes = doc.GetElementsByTagName("Part");
            foreach (XmlNode node in partNodes)
            {
                string partNo = (node["PartNo"] != null ? node["PartNo"].InnerText : "");
                string partName = (node["PartName"] != null ? node["PartName"].InnerText : "");
                string quantity = (node["Quantity"] != null ? node["Quantity"].InnerText : "");
                string size = (node["Size"] != null ? node["Size"].InnerText : "");
                string material = (node["Material"] != null ? node["Material"].InnerText : "");
                string remarks = (node["Remark"] != null ? node["Remark"].InnerText : (node["Remarks"] != null ? node["Remarks"].InnerText : ""));
                string imagePath = (node["Image"] != null ? node["Image"].InnerText : "");
                Part part = new Part(partNo, partName, quantity, size, material, remarks, imagePath);
                parts.Add(part);
            }
        }
        return parts;
    }

    public void UpdatePart(Part part)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(xmlFilePath))
            {
                doc.Load(xmlFilePath);
                XmlNode existingPart = null;
                foreach (XmlNode node in doc.GetElementsByTagName("Part"))
                {
                    if (node["PartNo"] != null && node["PartNo"].InnerText == part.PartNo)
                    {
                        existingPart = node;
                        break;
                    }
                }
                if (existingPart != null)
                {
                    existingPart["PartName"].InnerText = part.PartName;
                    existingPart["Quantity"].InnerText = part.Quantity;
                    existingPart["Size"].InnerText = part.Size;
                    existingPart["Material"].InnerText = part.Material;
                    existingPart["Remark"].InnerText = part.Remarks;
                    existingPart["Image"].InnerText = part.ImagePath;
                }
                else
                {
                    XmlElement newPart = doc.CreateElement("Part");
                    newPart.AppendChild(CreateElement(doc, "PartNo", part.PartNo));
                    newPart.AppendChild(CreateElement(doc, "PartName", part.PartName));
                    newPart.AppendChild(CreateElement(doc, "Quantity", part.Quantity));
                    newPart.AppendChild(CreateElement(doc, "Size", part.Size));
                    newPart.AppendChild(CreateElement(doc, "Material", part.Material));
                    newPart.AppendChild(CreateElement(doc, "Type", ""));
                    newPart.AppendChild(CreateElement(doc, "Remark", part.Remarks));
                    newPart.AppendChild(CreateElement(doc, "Image", part.ImagePath));

                    if (doc.DocumentElement != null)
                    {
                        doc.DocumentElement.AppendChild(newPart);
                    }
                    else
                    {
                        XmlElement root = doc.CreateElement("Parts");
                        root.AppendChild(newPart);
                        doc.AppendChild(root);
                    }
                }
            }
            else
            {
                XmlDocument newDoc = new XmlDocument();
                XmlElement root = newDoc.CreateElement("Parts");
                XmlElement newPart = newDoc.CreateElement("Part");

                newPart.AppendChild(CreateElement(newDoc, "PartNo", part.PartNo));
                newPart.AppendChild(CreateElement(newDoc, "PartName", part.PartName));
                newPart.AppendChild(CreateElement(newDoc, "Quantity", part.Quantity));
                newPart.AppendChild(CreateElement(newDoc, "Size", part.Size));
                newPart.AppendChild(CreateElement(newDoc, "Material", part.Material));
                newPart.AppendChild(CreateElement(newDoc, "Type", ""));
                newPart.AppendChild(CreateElement(newDoc, "Remark", part.Remarks));
                newPart.AppendChild(CreateElement(newDoc, "Image", part.ImagePath));

                root.AppendChild(newPart);
                newDoc.AppendChild(root);
                doc = newDoc;
            }
            doc.Save(xmlFilePath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("XML Update Fail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void RemovePart(string partNo)
    {
        try
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists(xmlFilePath))
            {
                doc.Load(xmlFilePath);
                XmlNode nodeToRemove = null;
                foreach (XmlNode node in doc.GetElementsByTagName("Part"))
                {
                    if (node["PartNo"] != null && node["PartNo"].InnerText == partNo)
                    {
                        nodeToRemove = node;
                        break;
                    }
                }
                if (nodeToRemove != null)
                {
                    nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
                    doc.Save(xmlFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Remove Part Fail: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private XmlElement CreateElement(XmlDocument doc, string name, string value)
    {
        XmlElement element = doc.CreateElement(name);
        element.InnerText = value;
        return element;
    }
}
#endregion

#region Main Form: Giao diện chính, chứa các điều khiển UI và gọi xử lý nghiệp vụ
public class MainForm : Form
{
    private ComboBox cbFilter;
    private ComboBox cbPartNo;
    private ComboBox cbPartName;
    private ComboBox cbQuantity;
    private ComboBox cbSize;
    private ComboBox cbMaterial;
    private ComboBox cbRemarks;
    private Button btnWrite;
    private PictureBox pictureBox;
    private Label lbStatus;
    private TextBox txtImageLink;
    private RadioButton rbtnBodyAssign;
    private RadioButton rbtnScreenShot;
    private RadioButton rbtnAutoWrite;
    private RadioButton rbtnRemoveTag;  // Radio button "Remove Tag" (không tham gia vòng lặp)

    private Dictionary<string, List<string>> partNosByFilter;
    private IPartListService _partListService;
    private NXHandler _nxHandler;
    private ILoggingService _logger;

    public MainForm()
    {
        InitializeComponent();
        _logger = new LoggingService(lbStatus);
        _nxHandler = new NXHandler(_logger);
        _partListService = new PartListService();
        LoadPartNosFromXML();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.cbFilter = new ComboBox();
        this.cbPartNo = new ComboBox();
        this.cbPartName = new ComboBox();
        this.cbQuantity = new ComboBox();
        this.cbSize = new ComboBox();
        this.cbMaterial = new ComboBox();
        this.cbRemarks = new ComboBox();
        this.btnWrite = new Button();
        this.pictureBox = new PictureBox();
        this.lbStatus = new Label();
        this.txtImageLink = new TextBox();
        this.rbtnBodyAssign = new RadioButton();
        this.rbtnScreenShot = new RadioButton();
        this.rbtnAutoWrite = new RadioButton();
        this.rbtnRemoveTag = new RadioButton();

        // cbFilter
        this.cbFilter.Location = new System.Drawing.Point(12, 12);
        this.cbFilter.Size = new System.Drawing.Size(121, 21);
        this.cbFilter.SelectedIndexChanged += new EventHandler(this.cbFilter_SelectedIndexChanged);
        this.cbFilter.Text = "Select Filter";

        // cbPartNo
        this.cbPartNo.Location = new System.Drawing.Point(12, 39);
        this.cbPartNo.Size = new System.Drawing.Size(121, 21);
        this.cbPartNo.SelectedIndexChanged += new EventHandler(this.cbPartNo_SelectedIndexChanged);
        this.cbPartNo.Text = "New Part No";

        // cbPartName
        this.cbPartName.Location = new System.Drawing.Point(12, 66);
        this.cbPartName.Size = new System.Drawing.Size(121, 21);
        this.cbPartName.Items.AddRange(new string[] { "Shaft", "Pin", "Base", "Part" });

        // cbQuantity
        this.cbQuantity.Location = new System.Drawing.Point(12, 93);
        this.cbQuantity.Size = new System.Drawing.Size(121, 21);
        this.cbQuantity.Items.AddRange(new string[] { "1EA", "2EA", "3EA", "4EA", "5EA", "6EA", "7EA", "8EA", "9EA", "10EA" });

        // cbMaterial
        this.cbMaterial.Location = new System.Drawing.Point(12, 120);
        this.cbMaterial.Size = new System.Drawing.Size(121, 21);
        this.cbMaterial.Items.AddRange(new string[] { "S45C", "SKD11", "SUS304", "Al6061", "Brass", "POM", "Acrylic" });

        // cbRemarks
        this.cbRemarks.Location = new System.Drawing.Point(12, 147);
        this.cbRemarks.Size = new System.Drawing.Size(121, 21);
        this.cbRemarks.Items.AddRange(new string[] { "40HRC ± 2", "54HRC ± 2", "60HRC ± 2", "Ni/Cr Anod", "Black Anod", "White Anod", "Hard Anod" });

        // cbSize
        this.cbSize.Location = new System.Drawing.Point(12, 174);
        this.cbSize.Size = new System.Drawing.Size(121, 21);
        this.cbSize.Items.AddRange(new string[] { "Size" });

        // btnWrite
        this.btnWrite.Location = new System.Drawing.Point(160, 250);
        this.btnWrite.Size = new System.Drawing.Size(121, 24);
        this.btnWrite.Text = "Write";
        this.btnWrite.UseVisualStyleBackColor = true;
        this.btnWrite.Click += new EventHandler(this.btnWrite_Click);

        // pictureBox
        this.pictureBox.Location = new System.Drawing.Point(150, 40);
        this.pictureBox.Size = new System.Drawing.Size(130, 130);
        this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        this.pictureBox.BorderStyle = BorderStyle.Fixed3D;

        // txtImageLink
        this.txtImageLink.Location = new System.Drawing.Point(12, 200);
        this.txtImageLink.Size = new System.Drawing.Size(121, 75);
        this.txtImageLink.Multiline = true;
        this.txtImageLink.ScrollBars = ScrollBars.Vertical;
        this.txtImageLink.TextChanged += new EventHandler(this.txtImageLink_TextChanged);

        // lbStatus
        this.lbStatus.AutoSize = true;
        this.lbStatus.Location = new System.Drawing.Point(139, 15);
        this.lbStatus.Size = new System.Drawing.Size(40, 13);
        this.lbStatus.ForeColor = Color.DarkRed;

        // rbtnBodyAssign
        this.rbtnBodyAssign.Location = new System.Drawing.Point(160, 170);
        this.rbtnBodyAssign.Size = new System.Drawing.Size(100, 17);
        this.rbtnBodyAssign.Text = "Body Assign";
        this.rbtnBodyAssign.Checked = true;

        // rbtnScreenShot
        this.rbtnScreenShot.Location = new System.Drawing.Point(160, 190);
        this.rbtnScreenShot.Size = new System.Drawing.Size(100, 17);
        this.rbtnScreenShot.Text = "Screen Shot";

        // rbtnAutoWrite
        this.rbtnAutoWrite.Location = new System.Drawing.Point(160, 210);
        this.rbtnAutoWrite.Size = new System.Drawing.Size(100, 17);
        this.rbtnAutoWrite.Text = "Write XML";

        // rbtnRemoveTag (không tham gia vòng lặp xoay vòng)
        this.rbtnRemoveTag.Location = new System.Drawing.Point(160, 230);
        this.rbtnRemoveTag.Size = new System.Drawing.Size(100, 17);
        this.rbtnRemoveTag.Text = "Remove Tag";

        // Form settings
        this.ClientSize = new System.Drawing.Size(300, 300);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.Text = "Part List";
        this.ShowIcon = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.TopMost = true;
        this.Load += new EventHandler(this.MainForm_Load);

        this.Controls.Add(this.cbFilter);
        this.Controls.Add(this.cbPartNo);
        this.Controls.Add(this.cbPartName);
        this.Controls.Add(this.cbQuantity);
        this.Controls.Add(this.cbSize);
        this.Controls.Add(this.cbMaterial);
        this.Controls.Add(this.cbRemarks);
        this.Controls.Add(this.txtImageLink);
        this.Controls.Add(this.btnWrite);
        this.Controls.Add(this.lbStatus);
        this.Controls.Add(this.pictureBox);
        this.Controls.Add(this.rbtnScreenShot);
        this.Controls.Add(this.rbtnBodyAssign);
        this.Controls.Add(this.rbtnAutoWrite);
        this.Controls.Add(this.rbtnRemoveTag);

        this.ResumeLayout(false);
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
            Session session = Session.GetSession();
            NXOpen.Part workPart = session.Parts.Work;
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
        // Xoay vòng qua 3 radio button: Body Assign -> Screen Shot -> Write XML -> Body Assign
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
            lbStatus.Text = "Không thấy file XML";
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
            pictureBox.Image = Image.FromFile(txtImageLink.Text);
        }
        else
        {
            pictureBox.Image = null;
        }
    }
}
#endregion

#region Entry Point: Class Program
public class Program
{
    public static void Main(string[] args)
    {
        Application.Run(new MainForm());
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
    }
}
#endregion
