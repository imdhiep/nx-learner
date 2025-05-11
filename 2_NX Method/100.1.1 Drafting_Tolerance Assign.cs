using System;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;

namespace AssignTolerance
{
    public class MainForm : Form
    {
        public MainForm() // Constructor khởi tạo form
        {  
            InitializeComponent();
        }

        private System.Windows.Forms.Label lbUpperTolerance;
        private System.Windows.Forms.Label lbLowerTolerance;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.ComboBox cbLowerTolerance;
        private System.Windows.Forms.ComboBox cbUpperTolerance;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Label lbFormName;

        private void InitializeComponent()
        {
            this.lbUpperTolerance = new System.Windows.Forms.Label();
            this.lbLowerTolerance = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            this.cbLowerTolerance = new System.Windows.Forms.ComboBox();
            this.cbUpperTolerance = new System.Windows.Forms.ComboBox();
            this.btClose = new System.Windows.Forms.Button();
            this.lbFormName = new System.Windows.Forms.Label();    
            this.SuspendLayout();

            // Thiết lập giao diện cho các thành phần giao diện người dùng
            //Close button
            this.btClose.BackColor = System.Drawing.Color.White;
            this.btClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btClose.Location = new System.Drawing.Point(225, 5);
            this.btClose.Name = "button1";
            this.btClose.Size = new System.Drawing.Size(25, 25);
            this.btClose.Text = "X";
            this.btClose.UseVisualStyleBackColor = false;
            this.btClose.Click += new System.EventHandler(this.button1_Click);
 
            //Form name label
            this.lbFormName.AutoSize = true;
            this.lbFormName.Location = new System.Drawing.Point(10, 10);
            this.lbFormName.Name = "label1";
            this.lbFormName.Size = new System.Drawing.Size(216, 13);
            this.lbFormName.Text = "Ultra Tolerance";
            this.lbFormName.ForeColor = System.Drawing.Color.DarkRed;

            //Upper tolerance label
            this.lbUpperTolerance.Location = new System.Drawing.Point(10, 50);
            this.lbUpperTolerance.Name = "lbUpperTolerance";
            this.lbUpperTolerance.Size = new System.Drawing.Size(100, 13);
            this.lbUpperTolerance.Text = "Upper Tolerance";

            //Lower tolerance label
            this.lbLowerTolerance.Location = new System.Drawing.Point(10, 80);
            this.lbLowerTolerance.Name = "lbLowerTolerance";
            this.lbLowerTolerance.Size = new System.Drawing.Size(100, 13);
            this.lbLowerTolerance.Text = "Lower Tolerance";

            //Upper tolerance combobox
            this.cbUpperTolerance.FormattingEnabled = true;
            this.cbUpperTolerance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbUpperTolerance.Location = new System.Drawing.Point(110, 48);
            this.cbUpperTolerance.Size = new System.Drawing.Size(80, 20);
            this.cbUpperTolerance.ForeColor = System.Drawing.Color.DarkRed;

            //Lower tolerance combobox
            this.cbLowerTolerance.FormattingEnabled = true;
            this.cbLowerTolerance.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbLowerTolerance.Location = new System.Drawing.Point(110, 78);
            this.cbLowerTolerance.Size = new System.Drawing.Size(80, 20);
            this.cbLowerTolerance.ForeColor = System.Drawing.Color.DarkRed;

            //Write button
            this.btnWrite.BackColor = System.Drawing.Color.White;

            this.btnWrite.Location = new System.Drawing.Point(200, 48);
            this.btnWrite.Size = new System.Drawing.Size(50, 50);
            this.btnWrite.Text = "WRITE";
            this.btnWrite.UseVisualStyleBackColor = false;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);

            // Form Setup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(265, 110);
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Opacity = 0.9D;
            this.Text = "Ultra Tolerance";
            this.TopMost = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += (sender, e) => 
                {
                    this.Location = new System.Drawing.Point(this.Location.X, this.Location.Y - 260); //Chỉnh vị trí Form xuất hiện
                    this.BeginInvoke((Action)(() => cbUpperTolerance.Focus()));
                };

            // Thêm các điều khiển vào form
            this.Controls.Add(this.lbFormName);
            this.Controls.Add(this.btClose);
            this.Controls.Add(this.lbUpperTolerance);
            this.Controls.Add(this.lbLowerTolerance);
            this.Controls.Add(this.cbUpperTolerance);
            this.Controls.Add(this.cbLowerTolerance);
            this.Controls.Add(this.btnWrite);

            // Hoàn tất việc thiết lập layout
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            double upperTolerance = 0.0;
            double lowerTolerance = 0.0;

            if (!double.TryParse(cbUpperTolerance.Text, out upperTolerance) || !double.TryParse(cbLowerTolerance.Text, out lowerTolerance))
            {
                return;
            }
            else
            {
                this.Hide();
            }

        NXOpen.Session theSession = NXOpen.Session.GetSession(); //Biến lưu phiên làm việc Session 
        NXOpen.Part workPart = theSession.Parts.Work; //Biến lưu workpart

        NXOpen.TaggedObject selectedObject = null; //Biến lưu đối tượng chọn, được trả về từ cửa sổ NXOpen.Selection.Response
        Selection.Response response; //Biến lưu kết quả của cửa sổ chọn Dim

        // Vòng lặp mở cửa sổ chọn đối tượng
        response = NXProcess.SelectDim(out selectedObject);

        while (response == Selection.Response.Ok && selectedObject != null)
        {
            	// Kiểm tra 2 số có giá trị tuyệt đối bằng nhau, và trái dấu
			if (Math.Abs(upperTolerance) == Math.Abs(lowerTolerance) && upperTolerance * lowerTolerance < 0)
            {
                NXProcess.OneLineTolerance(theSession, workPart, selectedObject, upperTolerance); // Truyền các giá trị vào phương thức
            }
            else
            {
                NXProcess.TwoLinesTolerance(theSession, workPart, selectedObject, upperTolerance, lowerTolerance); // Truyền các giá trị vào phương thức 
            }
            response = NXProcess.SelectDim(out selectedObject);
        }

        NXProcess.GetUnloadOption("");
        this.Show();
    }
}

    // class chứa các phương thức xử lý đối tượng NX
    public class NXProcess
        {
        // Phương thức Ghi dung sai Oneline (±)
        public static void OneLineTolerance(NXOpen.Session theSession, NXOpen.Part workPart, NXOpen.TaggedObject selectedObject, double upperTolerance)
        {
            NXOpen.Session.UndoMarkId markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start"); //Tạo mark để lưu trạng thái hành động trên Session
            /*
                Do phương thức lựa chọn Selection.Response trả về đối tượng selectedObject có kiểu class NXOpen.TaggedObject,
                nhưng các phương thức ghi dung sai như LinearDimensionBuilder, RadiusDimensionBuilder... lại nằm trong class NXOpen.Annotations.Dimension
                nên cần phải ép kiểu dữ liệu từ NXOpen.TaggedObject sang NXOpen.Annotations.Dimension để thực hiện
                Vì NXOpen.Annotations.Dimensions là lớp con của NXOpen.TaggedObject nên có thể ép kiểu được   
            */
            // Ép kiểu và kiểm tra
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension 
                || typeDim is NXOpen.Annotations.VerticalDimension 
                || typeDim is NXOpen.Annotations.CylindricalDimension 
                || typeDim is NXOpen.Annotations.PerpendicularDimension 
                || typeDim is NXOpen.Annotations.ParallelDimension)
            {
                 // Xử lý HorizontalDimension và VerticalDimension
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                linearDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension 
                || typeDim is NXOpen.Annotations.DiameterDimension
                || typeDim is NXOpen.Annotations.HoleDimension)
            {
                // Xử lý RadiusDimension
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                radialDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension 
                || typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                // Ép kiểu typeDim về OrdinateDimension. Do phương thức ghi dung sai OrdinateVertial và Horizontal không nằm trong NXOpen.Annotations.Dimension
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                    ordinateDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

         // Phương thức Ghi dung sai Twoline (+/-)
       public static void TwoLinesTolerance(NXOpen.Session theSession, NXOpen.Part workPart, NXOpen.TaggedObject selectedObject, double upperTolerance, double lowerTolerance)
        {
            NXOpen.Session.UndoMarkId markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Start");

            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension 
                || typeDim is NXOpen.Annotations.VerticalDimension 
                || typeDim is NXOpen.Annotations.CylindricalDimension 
                || typeDim is NXOpen.Annotations.PerpendicularDimension 
                || typeDim is NXOpen.Annotations.ParallelDimension)
            {
                 // Xử lý HorizontalDimension và VerticalDimension
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                linearDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                linearDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;                

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension 
                || typeDim is NXOpen.Annotations.DiameterDimension
                || typeDim is NXOpen.Annotations.HoleDimension)
            {
                // Xử lý RadiusDimension
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                radialDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                radialDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension 
                || typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                // Ép kiểu typeDim về OrdinateDimension. Do phương thức ghi dung sai OrdinateVertial và Horizontal không nằm trong NXOpen.Annotations.Dimension
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                    ordinateDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;                    

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

	    // PHương thức lựa chọn đối tượng
        public static Selection.Response SelectDim(out NXOpen.TaggedObject obj)
        {
            obj = null;
            NXOpen.UI ui = NXOpen.UI.GetUI();
            Selection.Response resp = Selection.Response.Cancel;

            string prompt = "Select dimensions";
            string message = "Select dimensions";
            Selection.SelectionScope scope = Selection.SelectionScope.WorkPart;
            Selection.SelectionAction selAction = Selection.SelectionAction.ClearAndEnableSpecific;

            Selection.MaskTriple[] selectionMaskArray = new Selection.MaskTriple[1];
            selectionMaskArray[0].Type = UFConstants.UF_dimension_type;
            selectionMaskArray[0].Subtype = 0;
            selectionMaskArray[0].SolidBodySubtype = 0;

            bool includeFeatures = false;
            bool keepHighlighted = false;

            Point3d cursor = new Point3d();

            resp = ui.SelectionManager.SelectTaggedObject(
                prompt,
                message,
                scope,
                selAction,
                includeFeatures,
                keepHighlighted,
                selectionMaskArray,
                out obj,
                out cursor
            );

            if (resp == Selection.Response.ObjectSelected || resp == Selection.Response.ObjectSelectedByName)
            {
                return Selection.Response.Ok;
            }
            else
            {
                return Selection.Response.Cancel;
            }
        }

	    // Phương thức Giải phóng tài nguyên
        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }
    }

    // class Program chạy chương trình
    public class Program
    {
        public static void Main(string[] args)
        {
            // Tạo và chạy FormApp
            Application.Run(new MainForm());
        }

    }
}



