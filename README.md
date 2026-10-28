# BÁO CÁO BÀI TẬP THỰC HÀNH LẬP TRÌNH C# WINFORMS

**Bài 5.1: Nâng cấp Form đăng nhập (Bài 4.1) bằng việc lưu trữ vào CSDL.**

## I. THÔNG TIN SINH VIÊN

- **Họ và tên:** Phạm Tuấn Thành
- **Mã số sinh viên:** 24810320264
- **Lớp học phần:** D19QTANM1
- **Link Repository:** [BT5.1-LoginForm](https://github.com/phmthnh/BT5.1-LoginForm)

---

## II. DANH SÁCH BÀI TẬP HOÀN THÀNH & TỰ ĐÁNH GIÁ

| Bài tập | Tiêu đề bài tập | Trạng thái | Mức độ hoàn thành (%) |
| :---: | :--- | :---: | :---: |
| **Bài 5.1** | Nâng cấp Form đăng nhập (Bài 4.1) bằng CSDL SQLite | Hoàn thành | 100% |

---

## III. MINH HỌA GIAO DIỆN & KỸ THUẬT NỔI BẬT

### 1. Form đăng nhập (Bài 4.1)

**Kỹ thuật áp dụng:** Windows Forms, `ErrorProvider`, truy vấn SQL có tham số với `Microsoft.Data.Sqlite`, lưu mật khẩu bằng PBKDF2-SHA256 và salt ngẫu nhiên.

**Chức năng:** Kiểm tra tên đăng nhập và mật khẩu không để trống; xác thực tài khoản từ CSDL; thông báo khi đăng nhập thành công hoặc nhập sai thông tin. CheckBox cho phép ẩn/hiện mật khẩu, Enter để đăng nhập và Esc để thoát.

![Giao diện Bài 5.1](docs/login-sqlite.png)

*Mô tả: Form hiển thị thông báo đăng nhập thành công sau khi xác thực tài khoản trong CSDL SQLite.*

### 2. Lưu trữ dữ liệu

- Bảng `Users` được tự tạo từ [Database/Schema.sql](Database/Schema.sql) khi mở ứng dụng lần đầu.
- Tài khoản thực hành: **`admin` / `Admin@123`**. Khởi tạo lại không ghi đè mật khẩu đã lưu.
- Mật khẩu được lưu dưới dạng hash, kèm salt và số vòng lặp; không lưu mật khẩu dạng văn bản thuần.
- File CSDL: `%LOCALAPPDATA%\LTNET\BT51-LoginForm\login.db`. Dữ liệu được giữ lại sau khi đóng và mở ứng dụng.

---

## IV. HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN

### 1. Môi trường

- Windows, .NET 10.0 SDK.
- Visual Studio 2026 (18.0 trở lên), workload **.NET desktop development**.
- NuGet `Microsoft.Data.Sqlite` được khôi phục khi build; bản SQLite không cần dịch vụ SQL Server.

### 2. Clone và chạy bằng Visual Studio

```bash
git clone https://github.com/phmthnh/BT5.1-LoginForm.git
cd BT5.1-LoginForm
```

1. Mở `BT5_1_LoginForm.slnx` bằng Visual Studio.
2. Nhấn **F5** hoặc chọn **Start** để biên dịch và chạy.
3. Nhập tài khoản thực hành ở mục III, nhấn **Đăng Nhập**.
4. Xem giao diện thiết kế: trong Solution Explorer, nhấp phải `LoginForm.cs` → **View Designer**, hoặc nhấn **Shift+F7**.

### 3. Chạy bằng terminal

Trong thư mục repository:

```bash
dotnet run --project BT5_1_LoginForm.csproj
```

Có thể đặt biến môi trường `BT51_DATABASE_PATH` để chọn đường dẫn file CSDL khác.
