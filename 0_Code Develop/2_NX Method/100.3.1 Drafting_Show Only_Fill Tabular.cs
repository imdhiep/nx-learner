using System;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using NXOpen;
using NXOpen.UF;

public class MainForm : Form
{

    private static NXOpen.Session theSession;
    private static NXOpen.Part workPart;
    private static NXOpen.UF.UFSession theUfSession;
    /* private static NXOpen.UI theUI; */

    //Khai báo các biến đối tượng Form
    private System.Windows.Forms.ComboBox cbPartNo;
    private System.Windows.Forms.ComboBox cbPartName;
    private System.Windows.Forms.ComboBox cbQuantity;
    private System.Windows.Forms.ComboBox cbSize;
    private System.Windows.Forms.ComboBox cbMaterial;
    private System.Windows.Forms.ComboBox cbRemarks;
    private System.Windows.Forms.Button btnWrite;
    private System.Windows.Forms.Button btnShowOnly;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label lbStatus;
    private System.Windows.Forms.CheckBox ckbBodyAssign;

    private PartProcessor partProcessor;
    private SheetProcessor sheetProcessor;
    private TabularNoteProcessor tabularNoteProcessor;


    public MainForm() // Constructor
    {
        InitializeComponent();
        this.Shown += MainForm_Shown;
        LoadPartNos();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        this.cbPartNo = new System.Windows.Forms.ComboBox();
        this.cbPartName = new System.Windows.Forms.ComboBox();
        this.cbQuantity = new System.Windows.Forms.ComboBox();
        this.cbSize = new System.Windows.Forms.ComboBox();
        this.cbMaterial = new System.Windows.Forms.ComboBox();
        this.cbRemarks = new System.Windows.Forms.ComboBox();
        this.btnWrite = new System.Windows.Forms.Button();
        this.btnShowOnly = new System.Windows.Forms.Button();
        this.pictureBox = new System.Windows.Forms.PictureBox();
        this.lbStatus = new System.Windows.Forms.Label();

        // cbPartNo
        this.cbPartNo.Location = new System.Drawing.Point(12, 12);
        this.cbPartNo.Size = new System.Drawing.Size(121, 21);
        this.cbPartNo.SelectedIndexChanged += new System.EventHandler(this.cbPartNo_SelectedIndexChanged);
        this.cbPartNo.Text = ("Click to Select");

        // cbPartName
        this.cbPartName.Location = new System.Drawing.Point(12, 39);
        this.cbPartName.Size = new System.Drawing.Size(121, 21);

        // cbQuantity
        this.cbQuantity.Location = new System.Drawing.Point(12, 66);
        this.cbQuantity.Size = new System.Drawing.Size(121, 21);

        // cbSize
        this.cbSize.Location = new System.Drawing.Point(12, 93);
        this.cbSize.Size = new System.Drawing.Size(121, 21);

        // cbMaterial
        this.cbMaterial.Location = new System.Drawing.Point(12, 120);
        this.cbMaterial.Size = new System.Drawing.Size(121, 21);

        // cbRemarks
        this.cbRemarks.Location = new System.Drawing.Point(12, 147);
        this.cbRemarks.Size = new System.Drawing.Size(121, 21);

        // btnWrite
        this.btnWrite.Location = new System.Drawing.Point(160, 180);
        this.btnWrite.Size = new System.Drawing.Size(121, 30);
        this.btnWrite.Text = "Write Tabular";
        this.btnWrite.UseVisualStyleBackColor = true;
        this.btnWrite.Click += new EventHandler(btnWrite_Click);

        // btnShowOnly
        this.btnShowOnly.Location = new System.Drawing.Point(12, 180);
        this.btnShowOnly.Size = new System.Drawing.Size(121, 30);
        this.btnShowOnly.UseVisualStyleBackColor = true;
        this.btnShowOnly.Text = "Show Only";
        this.btnShowOnly.Click += new System.EventHandler(this.btnShowOnly_Click);

        // pictureBox
        this.pictureBox.Location = new System.Drawing.Point(150, 20);
        this.pictureBox.Size = new System.Drawing.Size(130, 130);
        this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;

        // lbStatus
        this.lbStatus.AutoSize = true;
        this.lbStatus.Location = new System.Drawing.Point(139, 15);
        this.lbStatus.Size = new System.Drawing.Size(40, 13);
        this.lbStatus.ForeColor = System.Drawing.Color.DarkRed;
        this.lbStatus.Text = "Session loading...";

        // Form1
        this.ClientSize = new System.Drawing.Size(300, 225);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.Text = "Drafting Tabular";
        this.ShowIcon = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.TopMost = true;

        this.Load += (sender, e) =>
        {
            this.Location = new System.Drawing.Point(this.Location.X + 500, this.Location.Y + 100);
        };

        this.Controls.Add(this.cbPartNo);
        this.Controls.Add(this.cbPartName);
        this.Controls.Add(this.cbQuantity);
        this.Controls.Add(this.cbSize);
        this.Controls.Add(this.cbMaterial);
        this.Controls.Add(this.cbRemarks);
        this.Controls.Add(this.btnWrite);
        this.Controls.Add(this.btnShowOnly);
        this.Controls.Add(this.lbStatus);
        this.Controls.Add(this.pictureBox);

        this.ResumeLayout(false);
    }
    private void LoadPartNos()
    {
        // Get the path to the XML file
        string folderPath = Path.GetDirectoryName(NXOpen.Session.GetSession().Parts.Work.FullPath);
        string xmlFilePath = Path.Combine(folderPath, "xmlList.xml");

        // Load PartNos from the XML file
        List<string> partNos = XmlFileProcessor.LoadTagPartNo(xmlFilePath, ref lbStatus);

        // Populate ComboBox with PartNos
        foreach (var partNo in partNos)
        {
            cbPartNo.Items.Add(partNo);
        }
    }

    private async void MainForm_Shown(object sender, EventArgs e)
    {


        await Task.Run(() =>
        {
            theSession = NXOpen.Session.GetSession();
            theUfSession = NXOpen.UF.UFSession.GetUFSession();
        });

        partProcessor = new PartProcessor(theSession);
        sheetProcessor = new SheetProcessor(theSession);
        tabularNoteProcessor = new TabularNoteProcessor(theSession, theUfSession);

        this.Invoke((Action)(() =>
        {
            lbStatus.Text = "Session Already!";
        }));

    }


    private void cbPartNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string partNo = cbPartNo.SelectedItem.ToString();

        // Tải các giá trị tương ứng từ file XML
        string folderPath = Path.GetDirectoryName(NXOpen.Session.GetSession().Parts.Work.FullPath); // Lấy thư mục chứa tệp hiện tại
        string xmlFilePath = Path.Combine(folderPath, "xmlList.xml");

        // Call LoadPartProperties method to populate the ComboBoxes and load the image
        XmlFileProcessor.LoadTagPartProperties(xmlFilePath, partNo, cbPartName, cbQuantity, cbSize, cbMaterial, cbRemarks, pictureBox, lbStatus);
    }
    private void btnShowOnly_Click(object sender, EventArgs e)
    {
        string partNo = cbPartNo.SelectedItem.ToString();
        string partName = cbPartName.SelectedItem.ToString();

        // Call RenameSheet directly within btnShowOnly_Click
        sheetProcessor.RenameSheet(partNo, partName);  // Corrected here

        // Now calling ShowOnly from the PartVisibilityManager
        partProcessor.ShowOnly(partNo);
    }
    private void btnWrite_Click(object sender, EventArgs e)
    {
        try
        {
            this.Hide();

            // Extract values from ComboBoxes
            string partNo = cbPartNo.Text;
            string partName = cbPartName.Text;
            string quantity = cbQuantity.Text;
            string size = cbSize.Text;
            string material = cbMaterial.Text;
            string remarks = cbRemarks.Text;

            // Call method to update tabular note with values from ComboBoxes
            tabularNoteProcessor.UpdateTabularNote(partNo, partName, quantity, size, material, remarks); // Use instance of TabularNoteProcessor
            lbStatus.Text = "Write tabular successful";

            this.Show();
        }
        catch (Exception ex)
        {
            lbStatus.Text = "Write Fail :" + ex.GetType().Name;
            this.Show();
        }
    }
}

public class XmlFileProcessor
{
    public static List<string> LoadTagPartNo(string xmlFilePath, ref Label lbStatus)
    {
        List<string> partNos = new List<string>();

        if (File.Exists(xmlFilePath))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);

            // Get part numbers from XML
            foreach (XmlNode part in doc.GetElementsByTagName("Part"))
            {
                XmlNode partNoNode = part["PartNo"];
                if (partNoNode != null)
                {
                    partNos.Add(partNoNode.InnerText);
                }
            }

            // Sort the part numbers
            partNos.Sort();
        }
        else
        {
            lbStatus.Text = "XML file not found";
        }

        return partNos;
    }

    public static void LoadTagPartProperties(string xmlFilePath, string partNo,
                                          ComboBox cbPartName, ComboBox cbQuantity,
                                          ComboBox cbSize, ComboBox cbMaterial,
                                          ComboBox cbRemarks, PictureBox pictureBox, Label lbStatus)
    {
        try
        {
            if (File.Exists(xmlFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                foreach (XmlNode part in doc.GetElementsByTagName("Part"))
                {
                    if (part["PartNo"] != null && part["PartNo"].InnerText == partNo)
                    {
                        // Populate combo boxes with values from the XML
                        cbPartName.Items.Add(part["PartName"].InnerText);
                        cbQuantity.Items.Add(part["Quantity"].InnerText);
                        cbSize.Items.Add(part["Size"].InnerText);
                        cbMaterial.Items.Add(part["Material"].InnerText);
                        cbRemarks.Items.Add(part["Remark"].InnerText);

                        // Set selected items for ComboBoxes
                        cbPartName.SelectedItem = part["PartName"].InnerText;
                        cbQuantity.SelectedItem = part["Quantity"].InnerText;
                        cbSize.SelectedItem = part["Size"].InnerText;
                        cbMaterial.SelectedItem = part["Material"].InnerText;
                        cbRemarks.SelectedItem = part["Remark"].InnerText;

                        // Load and display image
                        if (System.IO.File.Exists(part["Image"].InnerText))
                        {
                            pictureBox.Image = Image.FromFile(part["Image"].InnerText);
                        }
                        else
                        {
                            pictureBox.Image = null;
                        }

                        break;
                    }
                }
            }
            else
            {
                lbStatus.Text = "XML file not found";
            }
        }
        catch (Exception ex)
        {
            lbStatus.Text = ex.GetType().Name;
        }
    }
}

public class PartProcessor
{
    private NXOpen.Session theSession;
    private NXOpen.Part workPart;

    public PartProcessor(NXOpen.Session session)
    {
        this.theSession = session;
        this.workPart = theSession.Parts.Work;
    }

    public void ShowOnly(string partNo)
    {
        // Hide all bodies in the current part
        HideAllBodies();

        // Find and show the body with the name starting with partNo (e.g., "101-PART [1]")
        foreach (Body body in workPart.Bodies)
        {
            if (body.Name.StartsWith(partNo))
            {
                List<DisplayableObject> objectsToShow = new List<DisplayableObject> { body };
                theSession.DisplayManager.ShowObjects(objectsToShow.ToArray(), DisplayManager.LayerSetting.ChangeLayerToSelectable);
                break; // Assuming one body matches, no need to continue after finding the correct body
            }
        }
    }

    public void HideAllBodies()
    {
        // Hide all bodies in the work part
        var markId = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Hide All Bodies");
        theSession.SetUndoMarkName(markId, "Hide All Bodies");

        theSession.DisplayManager.HideByType("SHOW_HIDE_TYPE_SOLID_BODIES", DisplayManager.ShowHideScope.AnyInAssembly);

        int nErrs = theSession.UpdateManager.DoUpdate(markId);
        if (nErrs > 0)
        {
            Console.WriteLine("Errors occurred while hiding bodies.");
        }

        theSession.DeleteUndoMark(markId, null);
    }
}

public class SheetProcessor
{
    private NXOpen.Session theSession;
    private NXOpen.Part workPart;

    public SheetProcessor(NXOpen.Session session)
    {
        this.theSession = session;
        this.workPart = theSession.Parts.Work;
    }

    public void RenameSheet(string partNo, string partName)
    {
        // Set the undo mark and rename the view
        theSession.SetUndoMark(Session.MarkVisibility.Visible, "Rename View");
        workPart.Views.WorkView.SetName(partNo + " - " + partName);
    }
}

public class TabularNoteProcessor
{
    private NXOpen.Session theSession;
    private NXOpen.UF.UFSession theUfSession;

    public TabularNoteProcessor(NXOpen.Session session, NXOpen.UF.UFSession ufSession)
    {
        this.theSession = session;
        this.theUfSession = ufSession;
    }

    public void UpdateTabularNote(string partNo, string partName, string quantity, string size, string material, string remarks)
    {
        const int rowNumber = 1;
        const int startColumn = 1;

        Tag myTableTag = Tag.Null;
        Tag myTableSection = Tag.Null;

        if (theSession.Parts.Work == null)
        {
            throw new Exception("No active part found.");
        }

        const string undoMarkName = "NXJ journal";
        Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, undoMarkName);

        // Create a dictionary with values passed from the form
        Dictionary<int, string> values = new Dictionary<int, string>
        {
            { 1, string.IsNullOrEmpty(partNo) ? "" : partNo },
            { 2, string.IsNullOrEmpty(partName) ? "" : partName },
            { 3, string.IsNullOrEmpty(quantity) ? "" : quantity },
            { 4, string.IsNullOrEmpty(size) ? "" : size },
            { 5, string.IsNullOrEmpty(material) ? "" : material },
            { 6, string.IsNullOrEmpty(remarks) ? "" : remarks }
        };

        // Select a tabular note
        Tag tabularNote = Tag.Null;
        if (SelectTabularNote(ref tabularNote) != Selection.Response.Ok)
        {
            throw new Exception("No tabular note selected.");
        }

        theUfSession.Tabnot.AskTabularNoteOfSection(tabularNote, out myTableTag);

        // Read the number of columns and rows of the table
        int numCols;
        theUfSession.Tabnot.AskNmColumns(myTableTag, out numCols);
        if (numCols < startColumn)
        {
            throw new Exception("Selected table has fewer columns than required.");
        }

        int numRows;
        theUfSession.Tabnot.AskNmRows(myTableTag, out numRows);
        if (numRows < rowNumber)
        {
            throw new Exception("Selected table has fewer rows than required.");
        }

        Tag rowTag = Tag.Null;
        theUfSession.Tabnot.AskNthRow(myTableTag, rowNumber - 1, out rowTag);
        if (rowTag == Tag.Null)
        {
            throw new Exception("Row not found.");
        }

        // Fill in the values into the table
        for (int colIndex = startColumn; colIndex < startColumn + values.Count; colIndex++)
        {
            if (colIndex - 1 >= numCols)
            {
                break;
            }

            Tag colTag = Tag.Null;
            theUfSession.Tabnot.AskNthColumn(myTableTag, colIndex - 1, out colTag);
            if (colTag == Tag.Null)
            {
                return;
            }

            Tag myCellTag = Tag.Null;
            theUfSession.Tabnot.AskCellAtRowCol(rowTag, colTag, out myCellTag);

            // Assign value directly from the values dictionary
            string newText = values.ContainsKey(colIndex) ? values[colIndex] : " ";

            theUfSession.Tabnot.SetCellText(myCellTag, newText); // Set the value (or a space if empty)
        }
    }

    private Selection.Response SelectTabularNote(ref Tag tabularNote)
    {
        string message = "Select a tabular note";
        string title = "Select a tabular note";
        int scope = UFConstants.UF_UI_SEL_SCOPE_ANY_IN_ASSEMBLY;
        int response = 0;
        Tag obj = Tag.Null;
        Tag view = Tag.Null;
        double[] cursor = new double[3];
        UFUi.SelInitFnT ip = new UFUi.SelInitFnT(InitProc);

        theUfSession.Ui.LockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);

        try
        {
            theUfSession.Ui.SelectWithSingleDialog(
                message,
                title,
                scope,
                ip,
                IntPtr.Zero,
                out response,
                out tabularNote,
                cursor,
                out view);
        }
        finally
        {
            theUfSession.Ui.UnlockUgAccess(UFConstants.UF_UI_FROM_CUSTOM);
        }

        if (response != UFConstants.UF_UI_OBJECT_SELECTED)
        {
            return Selection.Response.Cancel;
        }
        else
        {
            return Selection.Response.Ok;
        }
    }

    private int InitProc(IntPtr select_, IntPtr userdata)
    {
        int numTriples = 1;
        UFUi.Mask[] maskTriples = new UFUi.Mask[1];

        maskTriples[0].object_type = UFConstants.UF_tabular_note_type;
        maskTriples[0].object_subtype = UFConstants.UF_tabular_note_section_subtype;
        maskTriples[0].solid_type = 0;

        theUfSession.Ui.SetSelMask(select_, UFUi.SelMaskAction.SelMaskClearAndEnableSpecific, numTriples, maskTriples);
        return UFConstants.UF_UI_SEL_SUCCESS;
    }
}


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
