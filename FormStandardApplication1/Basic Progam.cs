using System;
using System.Collections.Generic;
using NXOpen;
using NXOpen.Drawings;

public class Program
{
    public static void Main(string[] args)
    {
        #region ShowOnlybyPartNo
        // Lấy phiên làm việc và phần hiện hành
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.Part workPart = theSession.Parts.Work;

        // Duyệt qua các Sheet lấy tên current Sheet
        DrawingSheet currentSheet = workPart.DrawingSheets.CurrentDrawingSheet;

        string partNo = ""; // Khai báo biến partNo bên ngoài

        if (currentSheet != null)
        {
            DraftingDrawingSheet draftingCurrentSheet = currentSheet as DraftingDrawingSheet;
            if (draftingCurrentSheet != null)
            {
                string currentSheetName = draftingCurrentSheet.Name;
                partNo = currentSheetName.Contains("-")
                    ? currentSheetName.Split('-')[0].Trim()
                    : currentSheetName;
            }
        }

        //Ẩn hết các đối tượng khác ngoài Body, Hiện Curve, Drafting Object
        theSession.DisplayManager.HideByType("SHOW_HIDE_TYPE_ALL", NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
        theSession.DisplayManager.ShowByType("SHOW_HIDE_TYPE_DRAWING_OBJECTS", NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);
        theSession.DisplayManager.ShowByType("SHOW_HIDE_TYPE_CURVES", NXOpen.DisplayManager.ShowHideScope.AnyInAssembly);

        Body matchingBody = null;
        foreach (Body body in workPart.Bodies)
        {
            string bodyPrefix = body.Name;
            if (bodyPrefix.Contains("-"))
            {
                bodyPrefix = bodyPrefix.Split('-')[0].Trim();
            }
            if (!string.IsNullOrEmpty(partNo) && string.Equals(bodyPrefix, partNo, StringComparison.OrdinalIgnoreCase))
            {
                matchingBody = body;
                break;
            }
        }

        // Nếu tìm thấy body phù hợp thì hiển thị
        if (matchingBody != null)
        {
            theSession.DisplayManager.ShowObjects(
                new DisplayableObject[] { matchingBody },
                DisplayManager.LayerSetting.ChangeLayerToSelectable);
        }
        #endregion

        // Định nghĩa mask để chỉ chọn các đối tượng thuộc lớp DraftingViews
        NXOpen.Selection.MaskTriple[] maskArray = new NXOpen.Selection.MaskTriple[1];
        maskArray[0] = new NXOpen.Selection.MaskTriple(60, -1, 0);

        // Lấy đối tượng được chọn qua UI (được trả về dưới dạng TaggedObject)
        NXOpen.UI theUI = NXOpen.UI.GetUI();
        NXOpen.TaggedObject[] selectedObjects;
        NXOpen.Selection.Response response = theUI.SelectionManager.SelectTaggedObjects(
            "Select a drafting view",
            "Select a drafting view",
            NXOpen.Selection.SelectionScope.WorkPart,
            NXOpen.Selection.SelectionAction.ClearAndEnableSpecific,
            false, false, maskArray,
            out selectedObjects);

        // Nếu không có đối tượng nào được chọn, thoát sớm
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            return;
        }

        // Tách các đối tượng thành 2 nhóm
        List<NXOpen.Drawings.DraftingView> firstPhase = new List<NXOpen.Drawings.DraftingView>();
        List<NXOpen.Drawings.DraftingView> secondPhase = new List<NXOpen.Drawings.DraftingView>();

        foreach (NXOpen.TaggedObject obj in selectedObjects)
        {
            NXOpen.Drawings.DraftingView draftingView = obj as NXOpen.Drawings.DraftingView;
            if (draftingView != null)
            {
                if (draftingView is NXOpen.Drawings.ProjectedView ||
                    draftingView is NXOpen.Drawings.SectionView ||
                    draftingView is NXOpen.Drawings.BaseView ||
                    draftingView is NXOpen.Drawings.DrawingView ||
                    draftingView is NXOpen.Drawings.DetailView ||
                    draftingView is NXOpen.Drawings.ViewBreak)
                {
                    if (draftingView is NXOpen.Drawings.DetailView || draftingView is NXOpen.Drawings.ViewBreak)
                    {
                        secondPhase.Add(draftingView);
                    }
                    else
                    {
                        firstPhase.Add(draftingView);
                    }
                }
            }
        }

        // Xử lý nhóm firstPhase
        foreach (NXOpen.Drawings.DraftingView draftingView in firstPhase)
        {
            if (draftingView.Style.HiddenLines.HiddenlineFont == NXOpen.Preferences.Font.Invisible)
            {
                draftingView.Style.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Dashed;
            }
            else
            {
                draftingView.Style.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Invisible;
            }
            draftingView.Commit();
        }

        // Xử lý nhóm secondPhase (DetailView và ViewBreak)
        foreach (NXOpen.Drawings.DraftingView draftingView in secondPhase)
        {
            if (draftingView.Style.HiddenLines.HiddenlineFont == NXOpen.Preferences.Font.Invisible)
            {
                draftingView.Style.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Dashed;
            }
            else
            {
                draftingView.Style.HiddenLines.HiddenlineFont = NXOpen.Preferences.Font.Invisible;
            }
            draftingView.Commit();
        }

        // Cập nhật các thay đổi
        int nErrs = theSession.UpdateManager.DoUpdate((NXOpen.Session.UndoMarkId)0);
    }

    public static int GetUnloadOption(string dummy)
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;
    }
}
