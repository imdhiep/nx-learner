using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;

namespace BasicFormApplication1
{

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