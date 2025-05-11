using System;
using System.Windows.Forms;
using NXOpen;

/// <summary>
/// Toàn bộ chương trình được đặt trong 1 phương thức Main duy nhất
/// Session và UI được khởi tạo ngay nên sẽ có độ trễ Startup overhead khi hiện Form nhưng các thao tác click liên quan đến Session,UI sẽ nhanh hơn
/// Trường hợp chỉ thực hiện ít tác vụ như bật tắt, setting tham số thì có thể khởi tạo Session, UI ngay trong sự kiện button.Click thì Form sẽ load ra ngay
/// </summary>
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.UI theUI = NXOpen.UI.GetUI();

        using (Form form = new Form
        {
            Text = "Program",
            ClientSize = new System.Drawing.Size(250, 100),
            BackColor = System.Drawing.Color.White,
            ShowIcon = false,
            TopMost = true,
            StartPosition = FormStartPosition.CenterScreen
        })
        {
            // Điều chỉnh vị trí Form hiện lên
            form.Load += (sender, e) =>
            {
                form.Location = new System.Drawing.Point(form.Location.X + 500, form.Location.Y - 200);
            }; 

            #region Message Label
            Label lblMessage = new Label
    {
        Text = "Message",
        Location = new System.Drawing.Point(5, 0),
        Size = new System.Drawing.Size(200, 23)
    }; 
            #endregion

            #region UI Controls
            // 
            Button btnOn = new Button
            {
                Text = "On",
                Location = new System.Drawing.Point(60, 40),
                Size = new System.Drawing.Size(60, 30)
            };
            // Đăng ký sự kiện Click cho btnOn
            btnOn.Click += (sender, e) =>
            {
                theUI.VisualizationVisualPreferences.Translucency = true;
                lblMessage.Text = "Translucency On";
            };

            // Khởi tạo Button "Off" bằng object initializer
            Button btnOff = new Button
            {
                Text = "Off",
                Location = new System.Drawing.Point(130, 40),
                Size = new System.Drawing.Size(60, 30)
            };
            // Đăng ký sự kiện Click cho btnOff
            btnOff.Click += (sender, e) =>
            {
                theUI.VisualizationVisualPreferences.Translucency = false;
                lblMessage.Text = "Translucency Off";
            }; 
            #endregion

            // Thêm các control vào Form
            form.Controls.Add(btnOn);
            form.Controls.Add(btnOff);
            form.Controls.Add(lblMessage);

            Application.Run(form);
        }
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
    }
}
