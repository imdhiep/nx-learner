using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Drawing;
using NXOpen;
using NXOpen.UF;

namespace PartList
{

    #region File: NXProcess.cs : Khởi tạo Session, Body Assign, Capture, Selection
    public class NXProcess
    {
        public static Session theSession;
        private static UFSession theUfSession;
        public static NXOpen.Part workPart;
        private readonly ILoggingService _logger;

        public NXProcess(ILoggingService logger)
        {
            _logger = logger;
            try
            {
                theSession = Session.GetSession();
                theUfSession = UFSession.GetUFSession();
                workPart = theSession.Parts.Work;
                _logger.Log("Session Already... ");
            }
            catch (Exception ex)
            {
                _logger.Log("Lỗi khởi tạo NX Session: " + ex.Message);
            }
        }

        /// <summary>
        /// BodyAssign: Đổi tên các body được chọn theo chuỗi PartNo-PartName,
        /// tính bounding box và trả về chuỗi kích thước dạng "LxWxH".
        /// </summary>
        public string BodyAssign(string partNo, string partName)
        {
            try
            {
                UI ui = UI.GetUI();
                NXObject[] selectedObjects = SelectBody(ui);
                if (selectedObjects != null && selectedObjects.Length > 0)
                {
                    string baseName = partNo + "-" + partName;
                    int index = 1;
                    foreach (NXObject obj in selectedObjects)
                    {
                        ObjectGeneralPropertiesBuilder opBuilder =
                            workPart.PropertiesManager.CreateObjectGeneralPropertiesBuilder(new NXObject[] { obj });
                        opBuilder.Name = baseName + " [" + index.ToString() + "]";
                        opBuilder.Commit();
                        opBuilder.Destroy();
                        index++;
                    }

                    // Lấy bounding box của đối tượng đầu tiên
                    UFSession ufSession = UFSession.GetUFSession();
                    NXOpen.Tag csys = NXOpen.Tag.Null;
                    double[] min_corner = new double[3];
                    double[,] directions = new double[3, 3];
                    double[] distances = new double[3];

                    ufSession.Csys.AskWcs(out csys);
                    ufSession.Modl.AskBoundingBoxExact(selectedObjects[0].Tag, csys, min_corner, directions, distances);

                    Array.Sort(distances);
                    Array.Reverse(distances);
                    string sizeFormatted = String.Format("{0:F1}x{1:F1}x{2:F1}", distances[0], distances[1], distances[2]);
                    _logger.Log("BodyAssign thành công. Kích thước: " + sizeFormatted);
                    return sizeFormatted;
                }
                else
                {
                    _logger.Log("No body selected!");
                    return "";
                }
            }
            catch (Exception ex)
            {
                _logger.Log("Body Assign Failed: " + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// CaptureImage: Chụp ảnh màn hình của phần được hiển thị và lưu vào thư mục "Pic".
        /// Trả về đường dẫn ảnh thông qua out parameter.
        /// </summary>
        public void CaptureImage(string partNo, string partName, string size, out string imagePath)
        {
            imagePath = "";
            try
            {
                if (partNo == "Part No" || partNo == "New Part No")
                {
                    _logger.Log("Capture Fail: Nhập PartNo !");
                    return;
                }

                string fullFilePath = workPart.FullPath;
                string folderPath = Path.GetDirectoryName(fullFilePath);
                string picFolderPath = Path.Combine(folderPath, "Pic");

                if (!Directory.Exists(picFolderPath))
                {
                    Directory.CreateDirectory(picFolderPath);
                }

                string imageFileName = partNo;
                if (!string.IsNullOrEmpty(partName))
                {
                    imageFileName += "-" + partName;
                }
                imageFileName += "-" + DateTime.Now.ToString("yyMMdd_HH-mm-ss") + ".jpg";
                imagePath = Path.Combine(folderPath, "Pic", imageFileName);

                UFSession ufSession = UFSession.GetUFSession();
                ufSession.Disp.CreateImage(imagePath, UFDisp.ImageFormat.Jpeg, UFDisp.BackgroundColor.White);

                _logger.Log("Capture Successful!");
            }
            catch (Exception ex)
            {
                _logger.Log("Capture Fail: " + ex.Message);
            }
        }

        /// <summary>
        /// SelectBody: Cho phép người dùng chọn body từ giao diện NX.
        /// </summary>
        private static NXObject[] SelectBody(UI ui)
        {
            NXObject[] objects = null;
            Selection.MaskTriple[] maskArray = new Selection.MaskTriple[1];
            maskArray[0].Type = NXOpen.UF.UFConstants.UF_solid_type;
            maskArray[0].Subtype = 0;
            maskArray[0].SolidBodySubtype = NXOpen.UF.UFConstants.UF_UI_SEL_FEATURE_SOLID_BODY;

            Selection.Response response = ui.SelectionManager.SelectObjects(
                "Select body",
                "Select a Body",
                Selection.SelectionScope.AnyInAssembly,
                Selection.SelectionAction.ClearAndEnableSpecific,
                false,
                false,
                maskArray,
                out objects);

            if (response == Selection.Response.Ok)
                return objects;
            return null;
        }
    }
    #endregion


}

