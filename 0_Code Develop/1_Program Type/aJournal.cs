using System;
using System.IO;
using System.Diagnostics;
using NXOpen;

public class Program
{
    public static void Main(string[] args)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();

        string partFullPath = thesession.Part.Work.FullPath;
        string folderPath = Path.GetDirectoryName(partFullPath);
        Process.Start("explorer.exe", folderPath);
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)Session.LibraryUnloadOption.Immediately;
    }
}


