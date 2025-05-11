using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using NXOpen;

/// <summary>
/// Toàn bộ chương trình được đặt trong 1 phương thức Main duy nhất
/// Session, UI được khai báo trước và khởi tạo theo sau khi Form.Shown nên Form sẽ hiện ra ngay
/// Dùng để điều khiển các tác vụ đơn giản, click nhiều lần khi mở form
/// </summary>
public class Program
{
    public static NXOpen.Session theSession;
    public static NXOpen.UI theUI;

    [STAThread]
    public static void Main(string[] args)
    {
        using (Form form = new Form())
        {
            #region Message Label
            Label lblMessage = new Label();
            lblMessage.Text = "Đang khởi tạo Session...";
            lblMessage.Location = new System.Drawing.Point(5, 0);
            lblMessage.Size = new System.Drawing.Size(200, 23);
            #endregion

            #region Form properties
            form.Text = "Program";
            form.ClientSize = new System.Drawing.Size(250, 100);
            form.BackColor = System.Drawing.Color.White;
            form.ShowIcon = false;
            form.TopMost = true;
            form.StartPosition = FormStartPosition.CenterScreen;

            form.Load += (s, e) =>
            {
                form.Location = new System.Drawing.Point(form.Location.X + 500, form.Location.Y - 200);
            };

            form.Shown += async (s, e) =>
            {
                await Task.Run(() =>
                {
                    theSession = NXOpen.Session.GetSession();
                    theUI = NXOpen.UI.GetUI();
                });
                form.Invoke((Action)(() =>
                {
                    lblMessage.Text = "Session Already!";
                }));
            };

            #endregion

            #region UI Controls
            // ButtonOn
            Button btnOn = new Button();
            btnOn.Text = "On";
            btnOn.Location = new System.Drawing.Point(60, 40);
            btnOn.Size = new System.Drawing.Size(60, 30);
            btnOn.Click += (s, e) =>
            {
                if (theUI != null)
                {
                    theUI.VisualizationVisualPreferences.Translucency = true;
                    lblMessage.Text = "Translucency On";
                }
                else
                {
                    lblMessage.Text = "Session chưa được khởi tạo!";
                }
            };

            // ButtonOff
            Button btnOff = new Button();
            btnOff.Text = "Off";
            btnOff.Location = new System.Drawing.Point(130, 40);
            btnOff.Size = new System.Drawing.Size(60, 30);
            btnOff.Click += (s, e) =>
            {
                if (theUI != null)
                {
                    theUI.VisualizationVisualPreferences.Translucency = false;
                    lblMessage.Text = "Translucency Off";
                }
                else
                {
                    lblMessage.Text = "Session chưa được khởi tạo!";
                }
            };
            
            #endregion

            // Add Controls to Form
            form.Controls.Add(lblMessage);
            form.Controls.Add(btnOn);
            form.Controls.Add(btnOff);

            Application.Run(form);
        } 
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
    }
}
