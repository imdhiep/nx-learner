using System;
using NXOpen;

public class NXJournal
{
    public static void Main(string[] args)
    {
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.Part workPart = theSession.Parts.Work;
        NXOpen.Part displayPart = theSession.Parts.Display;

        // Bắt đầu Undo Mark, giúp có thể hoàn tác lại các thao tác nếu cần
        NXOpen.Session.UndoMarkId markId1 = theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Visible, "Unsuppress All Features");

        NXOpen.Features.Feature[] allFeatures = workPart.Features.ToArray();


        // Unsuppress tất cả các feature (bỏ qua kiểm tra supressed)
        NXOpen.Features.Feature[] suppressedFeatures = workPart.Features.UnsuppressFeatures(allFeatures);

        // Kết thúc Undo Mark
        //theSession.DeleteUndoMark(markId1, null);
    }

    public static int GetUnloadOption(string dummy) 
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
    }
}
