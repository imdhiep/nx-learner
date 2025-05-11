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

}