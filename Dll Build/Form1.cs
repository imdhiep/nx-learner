using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NXOpen;
using NXOpen.Drawings;
using NXOpen.UF;

namespace FormWriteTabular
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // Đăng ký sự kiện Load để gán tiêu đề mặc định từ NXOpen Work Part
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Session theSession = Session.GetSession();
                if (theSession != null && theSession.Parts.Work != null)
                {
                    string fullPath = theSession.Parts.Work.FullPath;
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                    cbTitleName.Text = fileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FullPath Error. " + ex.Message);
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy session và Work Part hiện hành từ NXOpen
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;

                // Lấy danh sách các Drawing Sheet kiểu DraftingDrawingSheet
                List<DraftingDrawingSheet> sheetList = new List<DraftingDrawingSheet>();
                foreach (DrawingSheet sheet in workPart.DrawingSheets)
                {
                    DraftingDrawingSheet dSheet = sheet as DraftingDrawingSheet;
                    if (dSheet != null)
                    {
                        sheetList.Add(dSheet);
                    }
                }
                if (sheetList.Count == 0)
                {
                    MessageBox.Show("Don't have any WorkSheet");
                    return;
                }

                // Lấy danh sách các Tabular Note có số hàng >= 3
                UFSession ufSession = UFSession.GetUFSession();
                List<Tag> tabularList = new List<Tag>();
                Tag currentTab = NXOpen.Tag.Null;
                while (true)
                {
                    ufSession.Obj.CycleObjsInPart(workPart.Tag, UFConstants.UF_tabular_note_type, ref currentTab);
                    if (currentTab.Equals(NXOpen.Tag.Null))
                        break;

                    int type, subtype;
                    ufSession.Obj.AskTypeAndSubtype(currentTab, out type, out subtype);
                    if (type != UFConstants.UF_tabular_note_type || subtype != UFConstants.UF_tabular_note_section_subtype)
                        continue;

                    Tag tableTag = NXOpen.Tag.Null;
                    try
                    {
                        ufSession.Tabnot.AskTabularNoteOfSection(currentTab, out tableTag);
                    }
                    catch (NXOpen.NXException)
                    {
                        continue;
                    }

                    int numRows = 0;
                    ufSession.Tabnot.AskNmRows(tableTag, out numRows);
                    if (numRows >= 3)
                        tabularList.Add(currentTab);
                }

                if (sheetList.Count > tabularList.Count)
                {
                    MessageBox.Show("Don't Enough Row");
                    return;
                }

                // Lấy giá trị từ các ComboBox
                string designNo = cbDesignNo.Text;
                string projectName = cbProjectName.Text;
                string titleName = cbTitleName.Text;

                // Duyệt qua từng sheet và ghi thông tin vào Tabular Note tương ứng
                for (int i = 0; i < sheetList.Count; i++)
                {
                    DraftingDrawingSheet sheet = sheetList[i];
                    sheet.Open();

                    Tag tabNote = tabularList[i];
                    Tag tableTag = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskTabularNoteOfSection(tabNote, out tableTag);

                    int numCols = 0;
                    ufSession.Tabnot.AskNmColumns(tableTag, out numCols);
                    if (numCols < 1)
                        continue;

                    Tag col0 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskNthColumn(tableTag, 0, out col0);

                    // Hàng 1: DESIGN NO
                    Tag row0 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskNthRow(tableTag, 0, out row0);
                    Tag cell0 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskCellAtRowCol(row0, col0, out cell0);
                    ufSession.Tabnot.SetCellText(cell0, designNo);

                    // Hàng 2: PROJECT NAME
                    Tag row1 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskNthRow(tableTag, 1, out row1);
                    Tag cell1 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskCellAtRowCol(row1, col0, out cell1);
                    ufSession.Tabnot.SetCellText(cell1, projectName);

                    // Hàng 3: TITLE NAME
                    Tag row2 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskNthRow(tableTag, 2, out row2);
                    Tag cell2 = NXOpen.Tag.Null;
                    ufSession.Tabnot.AskCellAtRowCol(row2, col0, out cell2);
                    ufSession.Tabnot.SetCellText(cell2, titleName);
                }
                MessageBox.Show("Done!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                this.Close();
            }
        }

        // Các điều khiển được khai báo công khai để file .cs chính có thể truy cập (nếu cần)
        public ComboBox cbDesignNo;
        public ComboBox cbProjectName;
        public ComboBox cbTitleName;
        public Button btnWrite;
    }
}
