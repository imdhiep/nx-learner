using System;
using NXOpen;

public class Program
{
    public static void Main(string[] args)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.UI theUI = NXOpen.UI.GetUI();

        theUI.VisualizationVisualPreferences.Translucency = true;

        NXOpen.ListingWindow lw = theSession.ListingWindow;
        lw.Open();
        lw.WriteLine("Translucency On!");
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)Session.LibraryUnloadOption.Immediately;
    }
}
