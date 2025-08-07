# Sử dụng LocalTunnel để tạo đường dẫn truy cập web tour từ xa

Cách tạo local tunnel cho web tour
1. Cài đặt node.js
   - Tải node.js tại: https://nodejs.org/en/download/
   - Cài đặt node.js
   - Kiểm tra node.js đã cài đặt thành công hay chưa bằng lệnh: node -v
2. Cài đặt localtunnel
   - Mở cmd (hoặc terminal) và chạy lệnh: npm install -g localtunnel
3. Chạy web tour bằng "http" 
4. Lệnh: lt --port 8080 (thay 8080 bằng port của web tour)
5. Lệnh: https://loca.lt/mytunnelpassword (để lấy ip cho password, chạy trên trình duyệt)
6. Truy cập link đã được tạo
7. Nhập password đã tạo ở bước 5
8. Muốn gỡ localtunnel thì chạy lệnh: npm uninstall -g localtunnel