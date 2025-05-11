using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Translucency
{
    #region File: Program.cs
    /// <summary>
    /// Lớp Program chứa điểm khởi đầu của ứng dụng và khai báo các biến toàn cục cho NXOpen.
    /// </summary>
    public static class Program
    {
        public static Session theSession;
        public static UI theUI;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    #endregion

    #region File: Form1.cs
    /// <summary>
    /// Phần code-behind của Form1: xử lý các sự kiện và logic của Form.
    /// Lớp này là partial để kết hợp với phần thiết kế ở Form1.Design.cs.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Đăng ký sự kiện Form.Shown để khởi tạo phiên làm việc NXOpen bất đồng bộ.
            this.Shown += MainForm_Shown;

            // Đăng ký sự kiện cho nút btnWrite.
            this.btnWrite.Click += btnWrite_Click;
        }

        #region Sự kiện Form Shown - Khởi tạo NXOpen Session và UI
        /// <summary>
        /// Sự kiện Shown của Form được dùng để khởi tạo NXOpen.Session và NXOpen.UI sau khi Form hiển thị.
        /// Sau khi khởi tạo xong, nút btnWrite được hiển thị.
        /// </summary>
        private async void MainForm_Shown(object sender, EventArgs e)
        {
            // Khởi tạo NXOpen.Session và NXOpen.UI trên background thread
            await Task.Run(() =>
            {
                Program.theSession = Session.GetSession();
                Program.theUI = UI.GetUI();
            });
            // Hiển thị nút btnWrite sau khi đã khởi tạo thành công
            btnWrite.Visible = true;
        }
        #endregion

        #region Sự kiện Button Click
        /// <summary>
        /// Sự kiện click của nút Write.
        /// Xử lý các trường hợp nhập liệu từ 2 ô (upper, lower) và thực hiện gọi các hàm xử lý tương ứng.
        /// </summary>
        private void btnWrite_Click(object sender, EventArgs e)
        {
            // Lấy nội dung nhập từ 2 ô và loại bỏ khoảng trắng thừa
            string upperText = cbUpperTolerance.Text.Trim();
            string lowerText = cbLowerTolerance.Text.Trim();

            bool upperEntered = !string.IsNullOrEmpty(upperText);
            bool lowerEntered = !string.IsNullOrEmpty(lowerText);

            // Lấy work part từ phiên làm việc NXOpen (đã được khởi tạo trong Program.cs)
            NXOpen.Part workPart = Program.theSession.Parts.Work;

            // Ẩn form trong quá trình xử lý
            this.Hide();

            try
            {
                // Mở hộp thoại chọn đối tượng (giả sử NXProcess.SelectDim thực hiện chức năng này)
                NXOpen.TaggedObject selectedObject = null;
                Selection.Response response = NXProcess.SelectDim(out selectedObject);

                // Nếu cả 2 ô không có dữ liệu nhập thì gọi xử lý NoneTolerance cho từng đối tượng
                if (!upperEntered && !lowerEntered)
                {
                    while (response == Selection.Response.Ok && selectedObject != null)
                    {
                        NXProcess.NoneTolerance(Program.theSession, workPart, selectedObject);
                        response = NXProcess.SelectDim(out selectedObject);
                    }
                    return;
                }

                // *** Trường hợp: nếu cả 2 combobox có lựa chọn (SelectedItem khác null) ***
                if (cbUpperTolerance.SelectedItem != null && cbLowerTolerance.SelectedItem != null)
                {
                    // Lấy giá trị từ lựa chọn
                    string deviation = cbUpperTolerance.SelectedItem.ToString();
                    int grade = 0;
                    if (!int.TryParse(cbLowerTolerance.SelectedItem.ToString(), out grade))
                    {
                        // Nếu không ép được kiểu thì thoát 
                        return;
                    }

                    // Với mỗi đối tượng được chọn, gọi phương thức LimitsAndFits
                    while (response == Selection.Response.Ok && selectedObject != null)
                    {
                        NXProcess.LimitsAndFits(Program.theSession, workPart, selectedObject, deviation, grade);
                        response = NXProcess.SelectDim(out selectedObject);
                    }
                }

                // ----------------- Xử lý dựa trên giá trị nhập (Text) -----------------

                // Trường hợp 1: Nếu cả 2 ô đều được nhập → gọi BilateralTwoLines
                if (upperEntered && lowerEntered)
                {
                    double upperTolerance = 0.0;
                    double lowerTolerance = 0.0;
                    if (!double.TryParse(upperText, out upperTolerance) ||
                        !double.TryParse(lowerText, out lowerTolerance))
                    {
                        return;
                    }

                    while (response == Selection.Response.Ok && selectedObject != null)
                    {
                        NXProcess.BilateralTwoLines(Program.theSession, workPart, selectedObject, upperTolerance, lowerTolerance);
                        response = NXProcess.SelectDim(out selectedObject);
                    }
                }
                // Trường hợp 2 & 3: Chỉ có một trong hai ô được nhập
                else
                {
                    // Nếu chỉ có cbUpperTolerance được nhập
                    if (upperEntered)
                    {
                        double upperTolerance = 0.0;
                        if (!double.TryParse(upperText, out upperTolerance))
                        {
                            return;
                        }

                        while (response == Selection.Response.Ok && selectedObject != null)
                        {
                            // Nếu giá trị nhập bắt đầu bằng dấu '+' → gọi UnilateralAbove
                            if (upperText.StartsWith("+"))
                            {
                                NXProcess.UnilateralAbove(Program.theSession, workPart, selectedObject, upperTolerance);
                            }
                            // Ngược lại → gọi BilateralOneLine
                            else
                            {
                                NXProcess.BilateralOneLine(Program.theSession, workPart, selectedObject, upperTolerance);
                            }
                            response = NXProcess.SelectDim(out selectedObject);
                        }
                    }
                    // Nếu chỉ có cbLowerTolerance được nhập
                    else if (lowerEntered)
                    {
                        double lowerTolerance = 0.0;
                        if (!double.TryParse(lowerText, out lowerTolerance))
                        {
                            return;
                        }

                        while (response == Selection.Response.Ok && selectedObject != null)
                        {
                            NXProcess.UnilateralBelow(Program.theSession, workPart, selectedObject, lowerTolerance);
                            response = NXProcess.SelectDim(out selectedObject);
                        }
                    }
                }

                // Gọi hàm unload thư viện NXOpen (nếu cần)
                NXProcess.GetUnloadOption("");
            }
            finally
            {
                // Hiển thị lại form và xóa nội dung trong 2 ô nhập
                this.Show();
                cbUpperTolerance.Text = "";
                cbLowerTolerance.Text = "";
            }
        }
        #endregion
    }
    #endregion

    #region File: Form1.Design.cs
    /// <summary>
    /// Phần mã Designer của Form1: định nghĩa các control và giao diện ban đầu của Form.
    /// Lớp này là partial để kết hợp với phần code-behind ở Form1.cs.
    /// </summary>
    public partial class Form1 : Form
    {
        #region Designer Variables
        private System.ComponentModel.IContainer components = null;
        internal System.Windows.Forms.Label lbUpperTolerance;
        internal System.Windows.Forms.Label lbLowerTolerance;
        internal System.Windows.Forms.Button btnWrite;
        internal System.Windows.Forms.ComboBox cbLowerTolerance;
        internal System.Windows.Forms.ComboBox cbUpperTolerance;
        #endregion

        #region Designer Generated Code
        /// <summary>
        /// Phương thức cần thiết cho Designer support - không sửa đổi nội dung bên trong.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbUpperTolerance = new System.Windows.Forms.Label();
            this.lbLowerTolerance = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            this.cbLowerTolerance = new System.Windows.Forms.ComboBox();
            this.cbUpperTolerance = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lbUpperTolerance
            // 
            this.lbUpperTolerance.Location = new System.Drawing.Point(10, 12);
            this.lbUpperTolerance.Name = "lbUpperTolerance";
            this.lbUpperTolerance.Size = new System.Drawing.Size(100, 13);
            this.lbUpperTolerance.TabIndex = 2;
            this.lbUpperTolerance.Text = "Upper (Deviation)";
            // 
            // lbLowerTolerance
            // 
            this.lbLowerTolerance.Location = new System.Drawing.Point(10, 39);
            this.lbLowerTolerance.Name = "lbLowerTolerance";
            this.lbLowerTolerance.Size = new System.Drawing.Size(100, 13);
            this.lbLowerTolerance.TabIndex = 3;
            this.lbLowerTolerance.Text = "Lower (Grade)";
            // 
            // btnWrite
            // 
            this.btnWrite.BackColor = System.Drawing.Color.White;
            this.btnWrite.Location = new System.Drawing.Point(215, 9);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(53, 48);
            this.btnWrite.TabIndex = 6;
            this.btnWrite.Text = "WRITE";
            this.btnWrite.UseVisualStyleBackColor = false;
            // 
            // cbLowerTolerance
            // 
            this.cbLowerTolerance.ForeColor = System.Drawing.Color.DarkRed;
            this.cbLowerTolerance.FormattingEnabled = true;
            this.cbLowerTolerance.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.cbLowerTolerance.Location = new System.Drawing.Point(116, 36);
            this.cbLowerTolerance.Name = "cbLowerTolerance";
            this.cbLowerTolerance.Size = new System.Drawing.Size(80, 21);
            this.cbLowerTolerance.TabIndex = 5;
            // 
            // cbUpperTolerance
            // 
            this.cbUpperTolerance.ForeColor = System.Drawing.Color.DarkRed;
            this.cbUpperTolerance.FormattingEnabled = true;
            this.cbUpperTolerance.Items.AddRange(new object[] {
            "E",
            "F",
            "G",
            "H",
            "JS",
            "K",
            "M",
            "N",
            "e",
            "f",
            "g",
            "h",
            "js",
            "k",
            "m",
            "n",
            "p"});
            this.cbUpperTolerance.Location = new System.Drawing.Point(116, 9);
            this.cbUpperTolerance.Name = "cbUpperTolerance";
            this.cbUpperTolerance.Size = new System.Drawing.Size(80, 21);
            this.cbUpperTolerance.TabIndex = 4;
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(280, 64);
            this.Controls.Add(this.lbUpperTolerance);
            this.Controls.Add(this.lbLowerTolerance);
            this.Controls.Add(this.cbUpperTolerance);
            this.Controls.Add(this.cbLowerTolerance);
            this.Controls.Add(this.btnWrite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ultra Tolerance";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Sự kiện Load của Form: thiết lập vị trí ban đầu và chuyển focus.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new System.Drawing.Point(this.Location.X, this.Location.Y - 260);
            this.BeginInvoke((Action)(() => cbUpperTolerance.Focus()));
        }

        #region Dispose Method
        /// <summary>
        /// Giải phóng các tài nguyên đang được sử dụng.
        /// </summary>
        /// <param name="disposing">True nếu cần giải phóng tài nguyên.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
    #endregion

    #region File: NXProcess.cs
    /// <summary>
    /// Lớp chứa các phương thức xử lý liên quan đến đối tượng NX.
    /// </summary>
    public class NXProcess
    {
        /// <summary>
        /// Phương thức ghi dung sai dạng OneLine (±).
        /// </summary>
        public static void BilateralOneLine(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject, double upperTolerance)
        {
            Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
            // Ép kiểu selectedObject sang Dimension
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                linearDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                linearDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                radialDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                radialDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralOneLine;
                    ordinateDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId1, null);
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

        /// <summary>
        /// Phương thức ghi dung sai dạng TwoLines (+/-).
        /// </summary>
        public static void BilateralTwoLines(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject, double upperTolerance, double lowerTolerance)
        {
            Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                linearDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                linearDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                linearDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                radialDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                radialDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                radialDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.BilateralTwoLines;
                    ordinateDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId1, null);
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

        /// <summary>
        /// Phương thức ghi dung sai dạng TwoLines (+/0).
        /// </summary>
        public static void UnilateralAbove(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject, double upperTolerance)
        {
            Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralAbove;
                linearDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                linearDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralAbove;
                radialDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                radialDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralAbove;
                    ordinateDimensionBuilder.Style.DimensionStyle.UpperToleranceMetric = upperTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId1, null);
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

        /// <summary>
        /// Phương thức ghi dung sai dạng TwoLines (0/-).
        /// </summary>
        public static void UnilateralBelow(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject, double lowerTolerance)
        {
            Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralBelow;
                linearDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                linearDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralBelow;
                radialDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                radialDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.UnilateralBelow;
                    ordinateDimensionBuilder.Style.DimensionStyle.LowerToleranceMetric = lowerTolerance;
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceValuePrecision = 3;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId1, null);
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

        /// <summary>
        /// Phương thức ghi dung sai Fit Hole, Shaft
        /// </summary>
        public static void LimitsAndFits(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject, string deviation, int grade)
        {
            Session.UndoMarkId markId = theSession.SetUndoMark(Session.MarkVisibility.Visible, "LimitsAndFits");

            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId, null);
                return;
            }
            // Xử lý Linear Dimension
            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder builder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                builder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.LimitsAndFits;
                builder.Style.LetteringStyle.DimLineSpaceFactor = 0.000000000000000000001;

                if (deviation.Equals(deviation.ToUpper()))
                {
                    builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Hole;
                    builder.Style.DimensionStyle.LimitFitDeviation = deviation;
                    builder.Style.DimensionStyle.LimitFitGrade = grade;

                }
                else
                {
                    builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Shaft;
                    builder.Style.DimensionStyle.LimitFitShaftDeviation = deviation;
                    builder.Style.DimensionStyle.LimitFitShaftGrade = grade;
                }
                theSession.UpdateManager.DoUpdate(markId);
                builder.Commit();
                builder.Destroy();
            }
            // Radial Dimension
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder builder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                builder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.LimitsAndFits;
                builder.Style.LetteringStyle.DimLineSpaceFactor = 0.000000000000000000001;

                if (deviation.Equals(deviation.ToUpper()))
                {
                    builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Hole;
                    builder.Style.DimensionStyle.LimitFitDeviation = deviation;
                    builder.Style.DimensionStyle.LimitFitGrade = grade;
                }
                else
                {
                    builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Shaft;
                    builder.Style.DimensionStyle.LimitFitShaftDeviation = deviation;
                    builder.Style.DimensionStyle.LimitFitShaftGrade = grade;
                }
                theSession.UpdateManager.DoUpdate(markId);
                builder.Commit();
                builder.Destroy();
            }
            //Codinate Dimension
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder builder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    builder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.LimitsAndFits;
                    builder.Style.LetteringStyle.DimLineSpaceFactor = 0.000000000000000000001;

                    if (deviation.Equals(deviation.ToUpper()))
                    {
                        builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Hole;
                        builder.Style.DimensionStyle.LimitFitDeviation = deviation;
                        builder.Style.DimensionStyle.LimitFitGrade = grade;
                    }
                    else
                    {
                        builder.Style.DimensionStyle.LimitFitAnsiHoleType = NXOpen.Annotations.FitAnsiHoleType.Shaft;
                        builder.Style.DimensionStyle.LimitFitShaftDeviation = deviation;
                        builder.Style.DimensionStyle.LimitFitShaftGrade = grade;
                    }
                    theSession.UpdateManager.DoUpdate(markId);
                    builder.Commit();
                    builder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId, null);
                    return;
                }
            }
            else
            {
                theSession.DeleteUndoMark(markId, null);
                return;
            }
            theSession.DeleteUndoMark(markId, null);
        }

        /// <summary>
        /// Phương thức ghi dung sai dạng None
        /// </summary>
        public static void NoneTolerance(Session theSession, NXOpen.Part workPart, TaggedObject selectedObject)
        {
            Session.UndoMarkId markId1 = theSession.SetUndoMark(Session.MarkVisibility.Visible, "Start");
            // Ép kiểu selectedObject sang Dimension
            NXOpen.Annotations.Dimension typeDim = selectedObject as NXOpen.Annotations.Dimension;
            if (typeDim == null)
            {
                theSession.DeleteUndoMark(markId1, null);
                return;
            }

            if (typeDim is NXOpen.Annotations.HorizontalDimension ||
                typeDim is NXOpen.Annotations.VerticalDimension ||
                typeDim is NXOpen.Annotations.CylindricalDimension ||
                typeDim is NXOpen.Annotations.PerpendicularDimension ||
                typeDim is NXOpen.Annotations.ParallelDimension)
            {
                NXOpen.Annotations.LinearDimensionBuilder linearDimensionBuilder = workPart.Dimensions.CreateLinearDimensionBuilder(typeDim);
                linearDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.None;

                theSession.UpdateManager.DoUpdate(markId1);
                linearDimensionBuilder.Commit();
                linearDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.RadiusDimension ||
                     typeDim is NXOpen.Annotations.DiameterDimension ||
                     typeDim is NXOpen.Annotations.HoleDimension)
            {
                NXOpen.Annotations.RadialDimensionBuilder radialDimensionBuilder = workPart.Dimensions.CreateRadialDimensionBuilder(typeDim);
                radialDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.None;

                theSession.UpdateManager.DoUpdate(markId1);
                radialDimensionBuilder.Commit();
                radialDimensionBuilder.Destroy();
            }
            else if (typeDim is NXOpen.Annotations.VerticalOrdinateDimension ||
                     typeDim is NXOpen.Annotations.HorizontalOrdinateDimension)
            {
                NXOpen.Annotations.OrdinateDimension ordinateDim = typeDim as NXOpen.Annotations.OrdinateDimension;
                if (ordinateDim != null)
                {
                    NXOpen.Annotations.OrdinateDimensionBuilder ordinateDimensionBuilder = workPart.Dimensions.CreateOrdinateDimensionBuilder(ordinateDim);
                    ordinateDimensionBuilder.Style.DimensionStyle.ToleranceType = NXOpen.Annotations.ToleranceType.None;

                    theSession.UpdateManager.DoUpdate(markId1);
                    ordinateDimensionBuilder.Commit();
                    ordinateDimensionBuilder.Destroy();
                }
                else
                {
                    theSession.DeleteUndoMark(markId1, null);
                    return;
                }
            }

            theSession.DeleteUndoMark(markId1, null);
        }

        /// <summary>
        /// Phương thức lựa chọn đối tượng Dimension trong NX.
        /// </summary>
        public static Selection.Response SelectDim(out TaggedObject obj)
        {
            obj = null;
            UI ui = UI.GetUI();
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

            // Sử dụng NXOpen.Point3d (không thay đổi thành Point)
            NXOpen.Point3d cursor = new NXOpen.Point3d();

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

        /// <summary>
        /// Phương thức trả về tùy chọn unload của thư viện NXOpen.
        /// </summary>
        public static int GetUnloadOption(string dummy)
        {
            return (int)Session.LibraryUnloadOption.Immediately;
        }
    }
    #endregion

    #region File: Unload.cs
    /// <summary>
    /// Lớp Unload chứa phương thức GetUnloadOption trả về tùy chọn unload cho NXOpen.
    /// </summary>
    public class Unload
    {
        public static int GetUnloadOption(string dummy)
        {
            return (int)Session.LibraryUnloadOption.Immediately;
        }
    }
    #endregion
}
