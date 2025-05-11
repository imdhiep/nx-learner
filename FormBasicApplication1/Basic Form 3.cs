using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;

namespace BasicFormApplication1
{

    #region  File: Program.cs
    public class Program
    {
        public static NXOpen.Session theSession;
        public static NXOpen.UI theUI;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static int GetUnloadOption(string dummy)
        {
            return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
        }
    }

    #endregion

    #region File: Form.Desinger.cs
    public partial class Form1 : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Khai báo các control
        internal Label lblMessage;
        internal Button btnOn;
        internal Button btnOff;

        /// <summary>
        /// Required method for Designer support - không sửa đổi nội dung bên trong phương thức này bằng trình soạn thảo code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOn = new System.Windows.Forms.Button();
            this.btnOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(5, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(200, 23);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Đang khởi tạo Session...";
            // 
            // btnOn
            // 
            this.btnOn.Location = new System.Drawing.Point(60, 24);
            this.btnOn.Name = "btnOn";
            this.btnOn.Size = new System.Drawing.Size(60, 30);
            this.btnOn.TabIndex = 1;
            this.btnOn.Text = "On";
            this.btnOn.UseVisualStyleBackColor = true;
            this.btnOn.Click += new System.EventHandler(this.btnOn_Click);
            // 
            // btnOff
            // 
            this.btnOff.Location = new System.Drawing.Point(145, 24);
            this.btnOff.Name = "btnOff";
            this.btnOff.Size = new System.Drawing.Size(60, 30);
            this.btnOff.TabIndex = 2;
            this.btnOff.Text = "Off";
            this.btnOff.UseVisualStyleBackColor = true;
            this.btnOff.Click += new System.EventHandler(this.btnOff_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(250, 66);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnOn);
            this.Controls.Add(this.btnOff);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    #endregion

    #region File: Form.cs
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Sự kiện Shown: khởi tạo Session và UI bất đồng bộ
            this.Shown += async (s, e) =>
            {
                await Task.Run(() =>
                {
                    Program.theSession = NXOpen.Session.GetSession();
                    Program.theUI = NXOpen.UI.GetUI();
                });
                this.Invoke((Action)(() =>
                {
                    lblMessage.Text = "Session Already!";
                }));
            };
        }

        // Sự kiện Load: điều chỉnh vị trí của Form khi load
        private void Form1_Load(object sender, EventArgs e)
        {
            // Điều chỉnh vị trí của Form sau khi load
            this.Location = new System.Drawing.Point(this.Location.X + 500, this.Location.Y - 200);
        }

        // Sự kiện Click cho button On
        private void btnOn_Click(object sender, EventArgs e)
        {
            if (Program.theUI != null)
            {
                Program.theUI.VisualizationVisualPreferences.Translucency = true;
                lblMessage.Text = "Translucency On";
            }
            else
            {
                lblMessage.Text = "Session chưa được khởi tạo!";
            }
        }

        // Sự kiện Click cho button Off
        private void btnOff_Click(object sender, EventArgs e)
        {
            if (Program.theUI != null)
            {
                Program.theUI.VisualizationVisualPreferences.Translucency = false;
                lblMessage.Text = "Translucency Off";
            }
            else
            {
                lblMessage.Text = "Session chưa được khởi tạo!";
            }
        }
    }
    #endregion

}