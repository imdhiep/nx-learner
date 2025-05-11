using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;

namespace Application1
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
