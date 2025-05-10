using NXOpen;
using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        // Khởi tạo phiên làm việc NX và phần làm việc (work part)
        Session theSession = Session.GetSession();  // Lấy phiên làm việc NX hiện tại
        Part workPart = theSession.Parts.Work;      // Lấy phần làm việc hiện tại

        // Gọi phương thức để đặt tên các đặc tính cha dựa trên tên của các body
        NameParentFeaturesAfterBody(workPart);
    }

    // Phương thức này dùng để đặt tên cho các đặc tính cha dựa trên tên của các body
    public static void NameParentFeaturesAfterBody(Part workPart)
    {
        List<Body> partBodies = new List<Body>();  // Khai báo danh sách các body trong phần làm việc

        // Duyệt qua tất cả các body trong phần làm việc
        foreach (Body tempBody in workPart.Bodies)
        {
            try
            {
                if (tempBody.IsSolidBody)  // Kiểm tra nếu body là một solid body (khối rắn)
                {
                    partBodies.Add(tempBody);  // Thêm body vào danh sách partBodies
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi xử lý body: " + ex.Message);  // In ra thông báo lỗi nếu có sự cố
            }
        }

        // Duyệt qua tất cả các body trong danh sách
        foreach (Body tempBody in partBodies)
        {
            try
            {
                if (!string.IsNullOrEmpty(tempBody.Name))  // Kiểm tra xem body có tên hay không
                {
                    NXOpen.Features.Feature[] parentFeatures = tempBody.GetFeatures();  // Lấy các đặc tính cha của body

                    // Nếu body có ít nhất một đặc tính cha, đổi tên đặc tính cha đầu tiên thành tên của body
                    if (parentFeatures.Length > 0)
                    {
                        parentFeatures[0].SetName(tempBody.Name);  // Đặt tên cho đặc tính cha đầu tiên
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi đổi tên đặc tính cha: " + ex.Message);  // In ra thông báo lỗi nếu có sự cố
            }
        }
    }

    // Phương thức này xác định khi nào sẽ unload (hủy tải) phiên làm việc NX
    public static int GetUnloadOption(string dummy) 
    {
        return (int)NXOpen.Session.LibraryUnloadOption.Immediately;  // Xác định unload ngay lập tức
    }
}
