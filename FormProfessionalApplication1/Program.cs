using System;
using NXOpen;
using System.Windows.Forms;
//using NXOpen.UF;

namespace PartList
{

    #region Program.cs
    public static class Program
    {
        //public static NXOpen.Session theSession;
        //public static NXOpen.Part workPart;
        //public static NXOpen.Part displayPart;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
    #endregion

}



