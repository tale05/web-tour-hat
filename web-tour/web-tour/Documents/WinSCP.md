# Sử dụng WinSCP để kết nối với máy chủ từ xa

## 1. Cài đặt WinSCP

- Tải xuống WinSCP từ trang web chính thức: [WinSCP Download](https://winscp.net/eng/download.php).
- Cài đặt WinSCP với các tùy chọn mặc định.
- Mở WinSCP sau khi cài đặt xong.

## 2. Tạo tài khoản FTP trên hosting

- Đăng nhập vào trang quản lý hosting của bạn.
- Tìm phần quản lý FTP hoặc Tài khoản FTP.
- Tạo một tài khoản FTP mới với các thông tin sau:
  - Tên người dùng (username)
  - Mật khẩu (password)
  - Thư mục gốc httpdocs hoặc thư mục khác
  - Chọn permission: Read, Write, Delete (nếu có)

## 3. Thay đổi permission của folder đã chọn

- Vào mục Files, thư mục httpdocs
- Thay đổi permission thư mục httpdocs
- Vào thư mục Advanced (Nâng cao)
  - Tìm tài khoản FTP đã tạo
  - Chọn full permission

## 4. Kết nối với máy chủ từ xa bằng WinSCP
- Mở WinSCP.
- Chọn giao thức kết nối (FTP, SFTP, SCP).
- Nhập thông tin kết nối:
  - Hostname: Địa chỉ máy chủ (ví dụ: ftp.example.com)
  - Port number: Số cổng (thường là 21 cho FTP, 22 cho SFTP)
  - Username: Tên người dùng FTP đã tạo
  - Password: Mật khẩu của tài khoản FTP