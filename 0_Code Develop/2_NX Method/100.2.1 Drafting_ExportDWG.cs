using System;
using System.IO;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;

namespace dwgExport
{
    public class MainForm : Form
    {
        public MainForm() // Constructor khởi tạo form
        {
            InitializeComponent();
        }

        private System.Windows.Forms.Button btExportDWG;
        private System.Windows.Forms.Button btClose;
        private Button btGenerate;
        private TextBox tbScale;
        private RichTextBox txInDirection;
        private RichTextBox tbSheet;
        private Label lbStatus;
        private System.Windows.Forms.RichTextBox txbDirect;

        private void InitializeComponent()
        {
            this.btExportDWG = new System.Windows.Forms.Button();
            this.btClose = new System.Windows.Forms.Button();
            this.txbDirect = new System.Windows.Forms.RichTextBox();
            this.btGenerate = new System.Windows.Forms.Button();
            this.tbScale = new System.Windows.Forms.TextBox();
            this.txInDirection = new System.Windows.Forms.RichTextBox();
            this.tbSheet = new System.Windows.Forms.RichTextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btExportDWG
            // 
            this.btExportDWG.Location = new System.Drawing.Point(88, 210);
            this.btExportDWG.ForeColor = System.Drawing.Color.DarkRed;
            this.btExportDWG.Name = "btExportDWG";
            this.btExportDWG.Size = new System.Drawing.Size(94, 28);
            this.btExportDWG.TabIndex = 3;
            this.btExportDWG.Text = "DWG Export";
            this.btExportDWG.UseVisualStyleBackColor = true;
            this.btExportDWG.Click += new System.EventHandler(this.btExportDWG_Click);
            // 
            // btClose
            // 
            this.btClose.BackColor = System.Drawing.Color.Transparent;
            this.btClose.ForeColor = System.Drawing.Color.DarkRed;
            this.btClose.Location = new System.Drawing.Point(140, 3);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(42, 23);
            this.btClose.TabIndex = 2;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = false;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // txbDirect
            // 
            this.txbDirect.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txbDirect.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txbDirect.Location = new System.Drawing.Point(8, 94);
            this.txbDirect.Margin = new System.Windows.Forms.Padding(2);
            this.txbDirect.Name = "txbDirect";
            this.txbDirect.Size = new System.Drawing.Size(174, 55);
            this.txbDirect.TabIndex = 1;
            this.txbDirect.Text = "Output file direction";
            // 
            // btGenerate
            // 
            this.btGenerate.Location = new System.Drawing.Point(88, 177);
            this.btGenerate.ForeColor = System.Drawing.Color.DarkRed;
            this.btGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(94, 28);
            this.btGenerate.TabIndex = 5;
            this.btGenerate.Text = "Generate";
            this.btGenerate.UseVisualStyleBackColor = true;
            this.btGenerate.Click += new System.EventHandler(this.btGenerate_Click);
            // 
            // tbScale
            // 
            this.tbScale.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbScale.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbScale.ForeColor = System.Drawing.Color.Black;
            this.tbScale.Location = new System.Drawing.Point(8, 153);
            this.tbScale.Margin = new System.Windows.Forms.Padding(2);
            this.tbScale.Name = "tbScale";
            this.tbScale.Size = new System.Drawing.Size(173, 13);
            this.tbScale.TabIndex = 6;
            this.tbScale.Text = "Scale value based on Base View";
            // 
            // txInDirection
            // 
            this.txInDirection.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txInDirection.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txInDirection.Location = new System.Drawing.Point(8, 31);
            this.txInDirection.Margin = new System.Windows.Forms.Padding(2);
            this.txInDirection.Name = "txInDirection";
            this.txInDirection.Size = new System.Drawing.Size(173, 34);
            this.txInDirection.TabIndex = 7;
            this.txInDirection.Text = "Input direction";
            // 
            // tbSheet
            // 
            this.tbSheet.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbSheet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSheet.Location = new System.Drawing.Point(8, 69);
            this.tbSheet.Margin = new System.Windows.Forms.Padding(2);
            this.tbSheet.Name = "tbSheet";
            this.tbSheet.Size = new System.Drawing.Size(174, 21);
            this.tbSheet.TabIndex = 8;
            this.tbSheet.Text = "Sheet Name";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.ForeColor = System.Drawing.Color.DarkRed;
            this.lbStatus.Location = new System.Drawing.Point(5, 8);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(46, 13);
            this.lbStatus.TabIndex = 9;
            this.lbStatus.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(188, 250);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.tbSheet);
            this.Controls.Add(this.txInDirection);
            this.Controls.Add(this.tbScale);
            this.Controls.Add(this.btGenerate);
            this.Controls.Add(this.txbDirect);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.btExportDWG);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += (sender, e) => 
                {
                    this.Location = new System.Drawing.Point(this.Location.X - 500, this.Location.Y - 150); //Chỉnh vị trí Form xuất hiện
                };

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btGenerate_Click(object sender, EventArgs e)
        {
            NXOpen.Session theSession = NXOpen.Session.GetSession();
            NXOpen.Part workPart = theSession.Parts.Work;

            string fullPath1 = workPart.FullPath;


            NXOpen.Drawings.DrawingSheet currentDrawingSheet = workPart.DrawingSheets.CurrentDrawingSheet;

            if (currentDrawingSheet != null)//Kiểm tra có sheet file không
            {
			//Ép kiểu dữ liệu
                NXOpen.Drawings.DraftingDrawingSheet draftingDrawingSheet = (NXOpen.Drawings.DraftingDrawingSheet)currentDrawingSheet;

                string sheetName = draftingDrawingSheet.Name;

                txbDirect.Text = fullPath1.Substring(0, fullPath1.Length - 4) + "-" + sheetName + ".dwg";

                txInDirection.Text = fullPath1;

                tbSheet.Text = sheetName;

                // Lấy tất cả các View của sheet hiện tại
                NXOpen.Drawings.DraftingView[] views = draftingDrawingSheet.GetDraftingViews();
                bool hasBaseView = false;

                foreach (NXOpen.Drawings.DraftingView draftingView in views)
                {
                    // Ép kiểu an toàn từ DraftingView sang BaseView
                    NXOpen.Drawings.BaseView baseView = draftingView as NXOpen.Drawings.BaseView;

                    if (baseView != null)
                    {
                        hasBaseView = true; // Có BaseView
                        // Lấy tỷ lệ của BaseView
                        double scale = baseView.Style.General.Scale;

                        // Ghi tỷ lệ vào tbScale, chuyển đổi sang string
                        tbScale.Text = scale.ToString("0.##");

                    }
                }

                if (!hasBaseView)
                {
                   lbStatus.Text = "No BaseView found !"; // Thông báo nếu không có BaseView
                }
            }
            else
            {
                lbStatus.Text = "No current sheet found !";  // Thông báo nếu không có sheet hiện tại
            }
        }

private void btExportDWG_Click(object sender, EventArgs e)
{
    // Nếu lbStatus.Text là một trong các lỗi, không thực hiện tiếp
    if (lbStatus.Text == "No BaseView found!" || lbStatus.Text == "No current sheet found!")
    {
        return;  // Dừng lại nếu điều kiện này đúng, không tiếp tục kiểm tra
    }

    // Kiểm tra giá trị trong tbScale.Text chỉ khi không có lỗi về BaseView hoặc Sheet
    double scaleValue;
    if (!double.TryParse(tbScale.Text, out scaleValue))
    {
        lbStatus.Text = "Invalid Scale Value!";
        return;  // Dừng lại nếu giá trị scale không hợp lệ
    }

    try
    {
        // Nếu giá trị hợp lệ, tiếp tục thực hiện các bước
	  lbStatus.Text = "Exporting";
        BackupMappingFile();

        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.Part workPart = theSession.Parts.Work;
        NXOpen.Part displayPart = theSession.Parts.Display;

        string fullPath1 = workPart.FullPath;

        // Khởi tạo Undo Mark
        NXOpen.Session.UndoMarkId markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

        // Tạo DXF/DWG Creator
        NXOpen.DxfdwgCreator dxfdwgCreator1 = theSession.DexManager.CreateDxfdwgCreator();

        // Cấu hình các tùy chọn xuất
        dxfdwgCreator1.ExportData = NXOpen.DxfdwgCreator.ExportDataOption.Drawing;
        dxfdwgCreator1.AutoCADRevision = NXOpen.DxfdwgCreator.AutoCADRevisionOptions.R2007; // Set to 2007 Revision
        dxfdwgCreator1.ExportScaleOption = NXOpen.DxfdwgCreator.ExportScaleOptions.BaseView;
        dxfdwgCreator1.ViewEditMode = true;
        dxfdwgCreator1.ExportScaleValue = string.Format("{0}:1", scaleValue);
        dxfdwgCreator1.SettingsFile = "C:\\Program Files\\Siemens\\NX 12.0\\DXFDWG\\dxfdwg.def";
        dxfdwgCreator1.OutputFileType = NXOpen.DxfdwgCreator.OutputFileTypeOption.Dwg;
        dxfdwgCreator1.OutputFile = txbDirect.Text;
        dxfdwgCreator1.FlattenAssembly = false;
        dxfdwgCreator1.InputFile = txInDirection.Text;

        dxfdwgCreator1.TextFontMappingFile = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_TextFont_logfile.txt";
        dxfdwgCreator1.WidthFactorMode = NXOpen.DxfdwgCreator.WidthfactorMethodOptions.AutomaticCalculation;
        dxfdwgCreator1.CrossHatchMappingFile = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_CrossHatch_logfile.txt";
        dxfdwgCreator1.LineFontMappingFile = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_LineFont_logfile.txt";

        // Thiết lập Layer Mask và Drawing List
        dxfdwgCreator1.LayerMask = "*";
        dxfdwgCreator1.DrawingList = tbSheet.Text;

        // Commit Export
        NXOpen.NXObject nXObject1 = dxfdwgCreator1.Commit();

        // Final clean up
        theSession.DeleteUndoMark(markId1, null);
        dxfdwgCreator1.Destroy();
    }
    catch (Exception)
    {
        // Nếu có lỗi, thông báo lỗi và ngừng thực hiện
        lbStatus.Text = "Exporting Error"; 
    }
}

	
	//
private void BackupMappingFile()
{
    try
    {
        string sourcePath1 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_TextFont.txt";
        string sourcePath2 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_CrossHatch.txt";
        string sourcePath3 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_LineFont.txt";

        string backupPath1 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_TextFont_logfile.txt";
        string backupPath2 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_CrossHatch_logfile.txt";
        string backupPath3 = "E:\\UG-CODE1\\000_Code Dev\\Drafting\\Export DWG\\data\\Map_LineFont_logfile.txt";

        // Sao chép các file
        File.Copy(sourcePath1, backupPath1, true);
        File.Copy(sourcePath2, backupPath2, true);
        File.Copy(sourcePath3, backupPath3, true);

    }
    catch (Exception)
    {
    }
}

        // Phương thức Giải phóng tài nguyên
        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }


    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Application.Run(new MainForm());
        }
    }
}


