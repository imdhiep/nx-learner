using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;

public class Journal
{
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);

        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dllFolder = System.IO.Path.Combine(localAppData, "Siemens", "Journal", "dll");
        string dllPath = System.IO.Path.Combine(dllFolder, "FormWriteTabular.dll");

        Assembly formAssembly = null;
        if (File.Exists(dllPath))
        {
            formAssembly = Assembly.LoadFile(dllPath);
        }
        else
        {
            MessageBox.Show(".dll can't find: " + dllPath);
            return;
        }

        Type formType = formAssembly.GetType("FormWriteTabular.Form1");
        if (formType == null)
        {
            MessageBox.Show("Can't find FormWriteTabular.Form1 trong DLL.");
            return;
        }

        Form formInstance = (Form)Activator.CreateInstance(formType);
        Session theSession = Session.GetSession();
        Application.Run(formInstance);
    }

    private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
    {
        AssemblyName requestedAssembly = new AssemblyName(args.Name);
        if (requestedAssembly.Name.Equals("FormWriteTabular", StringComparison.OrdinalIgnoreCase))
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dllFolder = System.IO.Path.Combine(localAppData, "Siemens", "Journal", "dll");
            string assemblyPath = System.IO.Path.Combine(dllFolder, "FormWriteTabular.dll");
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFile(assemblyPath);
            }
        }
        return null;
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)Session.LibraryUnloadOption.Immediately;
    }
}
