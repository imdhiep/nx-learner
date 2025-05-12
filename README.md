# NX Learner

**NX Learner** là bộ sưu tập các ví dụ, template và ứng dụng mẫu giúp bạn học lập trình với NX Open API (Siemens NX) bằng C# và VB.NET.  
Dự án này dành cho những ai muốn bắt đầu phát triển plugin, macro, hoặc tự động hóa trong môi trường Siemens NX.

---

## 1. Tổng quan về NX Open

**NX Open** là bộ thư viện lập trình (API) của Siemens NX, cho phép bạn tự động hóa, mở rộng tính năng, xây dựng plugin hoặc macro cho phần mềm thiết kế Siemens NX.  
Bạn có thể sử dụng nhiều ngôn ngữ: C#, VB.NET, C++, Python.

**Các ứng dụng NX Open thường gặp:**
- Tạo macro tự động hóa thao tác lặp đi lặp lại.
- Xây dựng giao diện nhập liệu, xử lý dữ liệu CAD/CAM.
- Tạo các công cụ kiểm tra, báo cáo, xuất dữ liệu từ NX.
- Tùy biến giao diện, menu, tích hợp phần mềm khác.

---

## 2. Lộ trình học với repo này

**Gợi ý cho người mới bắt đầu:**
1. Đọc qua phần "Cấu trúc thư mục" bên dưới để hiểu từng ví dụ.
2. Bắt đầu từ các ví dụ đơn giản trong `1_Program Type/` và `BasicClassApplication1/`.
3. Làm quen với cách build DLL/EXE trong `Dll Build/`.
4. Thử tạo giao diện với WinForms (`FormBasicApplication1/`) và học cách chuyển sang BlockStyler nếu cần.
5. Nâng cao với các ví dụ về Radial Menu, ListingWindow, và các phương pháp tương tác khác trong `2_NX Method/`.
6. Tham khảo tài liệu, video trong `url-learner.txt` khi gặp khó khăn.

---

## 3. Cấu trúc thư mục & Giải thích chi tiết

| Thư mục/Tệp                         | Mô tả chi tiết                                                                                  |
|-------------------------------------|------------------------------------------------------------------------------------------------|
| **1_Program Type/**                 | Các ví dụ về các loại chương trình NX Open: Journal, DLL, EXE, Add-in.                         |
| **2_NX Method/**                    | Các phương pháp tương tác với NX Open: WinForms, BlockStyler, ListingWindow, Radial Menu, ...  |
| **BasicClassApplication1/**         | Ứng dụng mẫu sử dụng class cơ bản, minh họa cách tổ chức code hướng đối tượng trong NX Open.   |
| **Dll Build/**                      | Hướng dẫn và ví dụ build DLL cho NX (bao gồm cấu hình project, script build, v.v.).            |
| **FormBasicApplication1/**          | Ứng dụng WinForms đơn giản, ví dụ về giao diện cơ bản kết hợp với NX Open.                     |
| **FormProfessionalApplication1/**   | Ứng dụng WinForms nâng cao, tích hợp nhiều tính năng, giao diện chuyên nghiệp hơn.              |
| **FormStandardApplication1/**       | Ứng dụng WinForms theo chuẩn, minh họa cấu trúc và best practice khi xây dựng plugin NX.        |
| **Radial Menu/**                    | Ví dụ về tạo và tùy biến Radial Menu trong NX (menu dạng hình tròn, thuận tiện thao tác nhanh). |
| **Program - v2.cs**                 | File mã nguồn chính, ví dụ nâng cao về xử lý logic hoặc giao diện với NX Open.                  |
| **url-learner.txt**                 | Tổng hợp link tài liệu, video, blog về NX Open và lập trình NX.                                 |

---

### 3.1. 1_Program Type/

**Mục đích:**  
Giúp bạn hiểu các kiểu chương trình có thể phát triển với NX Open.

**Bao gồm:**
- **Journal:**  
  - Script nhỏ, chạy trực tiếp trong NX (Alt+F11).  
  - Thường dùng để thử nghiệm nhanh, không cần build DLL.
- **DLL Plugin:**  
  - Thư viện động, build từ Visual Studio, nạp vào NX để mở rộng tính năng.
- **EXE:**  
  - Ứng dụng độc lập, có thể giao tiếp với NX qua API.
- **Add-in:**  
  - Plugin dạng mở rộng giao diện NX, có thể thêm menu, nút bấm.

**Cách chạy:**  
- Với Journal: Copy code vào NX Journal Editor (Alt+F11) và chạy.
- Với DLL: Build ra file .dll, nạp vào NX qua Tools > Journal > Play hoặc Customer Defaults.
- Với EXE: Build ra file .exe, chạy ngoài hoặc từ NX.

---

### 3.2. 2_NX Method/

**Mục đích:**  
Giới thiệu các phương pháp tương tác với NX Open.

**Bao gồm:**
- **WinForms:**  
  - Giao diện truyền thống của .NET (Form, Button, DataGridView).
  - Lưu ý: Không phải lúc nào cũng chạy được trong NX, chỉ dùng để học hoặc chạy ngoài.
- **BlockStyler:**  
  - Giao diện native của NX, khuyến nghị dùng cho plugin thực tế.
  - Tạo form, nhập liệu, bảng dữ liệu, v.v.
- **ListingWindow:**  
  - Cửa sổ log của NX, thường dùng để xuất thông báo, debug.
- **Radial Menu:**  
  - Menu dạng vòng tròn, thao tác nhanh trong NX.
- **Khác:**  
  - Ví dụ về automation, xuất file, đọc dữ liệu, v.v.

**Cách chạy:**  
- Đọc README hoặc hướng dẫn trong từng thư mục con để biết cách build và nạp code.

---

### 3.3. BasicClassApplication1/

**Mục đích:**  
Giúp bạn hiểu cách tổ chức code hướng đối tượng trong NX Open.

**Nội dung:**
- Tạo class quản lý session, part, UI.
- Tách biệt logic xử lý và giao diện.
- Ví dụ về thao tác với đối tượng hình học, thuộc tính part, v.v.

**Cách chạy:**  
- Mở project bằng Visual Studio, build, và nạp vào NX như hướng dẫn ở trên.

---

### 3.4. Dll Build/

**Mục đích:**  
Hướng dẫn build DLL cho NX Open.

**Nội dung:**
- Cấu hình project Visual Studio:  
  - Thêm reference đến NXOpen.dll, NXOpen.UF.dll.
  - Chọn đúng version .NET Framework.
- Script build tự động (nếu có).
- Hướng dẫn nạp DLL vào NX.

**Cách chạy:**  
- Làm theo hướng dẫn trong file README hoặc file hướng dẫn trong thư mục này.

---

### 3.5. FormBasicApplication1/

**Mục đích:**  
Ví dụ về giao diện WinForms cơ bản kết hợp NX Open.

**Nội dung:**
- Tạo form, button, textbox, xử lý sự kiện.
- Kết nối với NX Open để thao tác dữ liệu.

**Lưu ý:**  
- WinForms không phải lúc nào cũng chạy được trong NX, nên dùng để học hoặc chạy ngoài.

---

### 3.6. FormProfessionalApplication1/

**Mục đích:**  
Ví dụ về ứng dụng WinForms nâng cao.

**Nội dung:**
- Giao diện nhiều tab, quản lý dữ liệu phức tạp.
- Tích hợp nhiều tính năng, ví dụ thực tế cho dự án lớn.

---

### 3.7. FormStandardApplication1/

**Mục đích:**  
Ví dụ về ứng dụng WinForms chuẩn hóa.

**Nội dung:**
- Cấu trúc project rõ ràng: UI, logic, data.
- Dễ dàng mở rộng, bảo trì.

---

### 3.8. Radial Menu/

**Mục đích:**  
Ví dụ về tạo Radial Menu (menu hình tròn) trong NX.

**Nội dung:**
- Tạo menu, gán chức năng cho từng nút.
- Tùy biến giao diện, thao tác nhanh trong NX.

---

### 3.9. Program - v2.cs

**Mục đích:**  
File mã nguồn nâng cao, có thể chứa:
- Logic xử lý đặc biệt (automation, thao tác dữ liệu lớn).
- Case study thực tế.
- Các hàm tiện ích, class dùng chung.

---

### 3.10. url-learner.txt

**Mục đích:**  
Tổng hợp link tài liệu, video, blog về NX Open, giúp bạn tự học và tra cứu khi cần.

---

## 4. Hướng dẫn cài đặt và chạy ví dụ

### Bước 1: Chuẩn bị môi trường

- Cài đặt **Siemens NX** (khuyến nghị từ NX 1926 trở lên).
- Cài đặt **Visual Studio** (2019 hoặc mới hơn).
- Đảm bảo đã cài **NX Open API** (có sẵn khi cài NX).
- Thêm reference đến `NXOpen.dll`, `NXOpen.UF.dll` trong project.

### Bước 2: Clone repo về máy

