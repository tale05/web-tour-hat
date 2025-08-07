# Entity Framework

## 1. Tải các thư viện cần thiết
- Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.4
- Install-Package Microsoft.EntityFrameworkCore.Tools

## 2. Thêm mới database
- Câu lệnh như sau:

```Scaffold-DbContext "Data Source=LAPTOP-1CN76TIN\SQLEXPRESS;Initial Catalog=db_travel;Persist Security Info=True;User ID=sa;Password=123;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Entities```

## 3. Generate lại database
- Câu lệnh như sau:

```Scaffold-DbContext "Data Source=LAPTOP-1CN76TIN\SQLEXPRESS;Initial Catalog=db_travel;Persist Security Info=True;User ID=sa;Password=123;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Entities -Force```

## 3. Generate database for hosting
- Câu lệnh như sau:

```Scaffold-DbContext "Data Source=apple-rds.maychudns.net,1441;Initial Catalog=dulichhat_com_dbtravel;Persist Security Info=True;User ID=dulichhat;Password=adminWeb@0110;Trust Server Certificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Entities -Force```