using System;
using NXOpen;

public class ShowOnly
{
    public static void Main(string[] args)
    {
        // Lấy đối tượng Session từ NX
        NXOpen.Session theSession = NXOpen.Session.GetSession();
        NXOpen.UI ui = NXOpen.UI.GetUI();
        NXOpen.Part workPart = theSession.Parts.Work;

        // Lấy tất cả các Body trong Part hiện tại
        NXOpen.Body[] allBodies = workPart.Bodies.ToArray();

        // Ẩn tất cả các Body trong mảng allBodies
        theSession.DisplayManager.BlankObjects(allBodies);

        // Mảng lưu trữ các Body được chọn
        NXOpen.NXObject[] selectedBodies = SelectBody(ui);
        
        // Hiển thị các Body được chọn trong mảng selectedBodies
        if (selectedBodies != null && selectedBodies.Length > 0)
        {
            NXOpen.DisplayableObject[] selectedBodyObjects = new NXOpen.DisplayableObject[selectedBodies.Length];
            for (int i = 0; i < selectedBodies.Length; i++)
            {
                selectedBodyObjects[i] = (NXOpen.DisplayableObject)selectedBodies[i];
            }
            theSession.DisplayManager.ShowObjects(selectedBodyObjects, NXOpen.DisplayManager.LayerSetting.ChangeLayerToSelectable);
        }
    }

    // Phương thức để thực hiện thao tác chọn Body
    private static NXOpen.NXObject[] SelectBody(NXOpen.UI ui)
    {
        NXOpen.NXObject[] objects = null; // Đối tượng 

        NXOpen.Selection.MaskTriple[] maskArray = new NXOpen.Selection.MaskTriple[1];
        maskArray[0].Type = NXOpen.UF.UFConstants.UF_solid_type;
        maskArray[0].Subtype = 0;
        maskArray[0].SolidBodySubtype = NXOpen.UF.UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY;

        NXOpen.Selection.Response response = ui.SelectionManager.SelectObjects(
            "Select body", // message
            "Select a Body", // title
            NXOpen.Selection.SelectionScope.AnyInAssembly, // scope
            NXOpen.Selection.SelectionAction.ClearAndEnableSpecific, // action
            false,  // includeFeatures: cần là false vì không ép kiểu đối tượng feature được
            false, // keepHighlighted: không giữ highlighted
            maskArray, // mảng MaskTriple
            out objects // truyền dữ liệu vào objects
        );

        if (response == NXOpen.Selection.Response.Ok)
        {
            return objects; // Trả về mảng các đối tượng được chọn
        }

        return null; // Trả về null nếu không có đối tượng được chọn
    }

    // Hàm này trả về lựa chọn tải thư viện khi cần thiết
    public static int GetUnloadOption(string dummy)
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately; // Thư viện sẽ được tải ngay lập nhật sau khi sử dụng
    }
}