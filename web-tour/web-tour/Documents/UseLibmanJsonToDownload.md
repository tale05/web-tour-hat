# Sử dụng libman.json để tải các thư viện cho Frontend trong ASP.NET Core

## 1. Tạo và cấu hình file libman.json

- Tạo file: libman.json cùng cấp với Program.cs
- Cấu hình libman.json như sau:

```{
  "version": "1.0",
  "defaultProvider": "cdnjs",
  "libraries": [
    {
      "library": "twitter-bootstrap@3.3.7",
      "destination": "wwwroot/lib/bootstrap/dist"
    },
    {
      "library": "jquery@3.7.1",
      "destination": "wwwroot/lib/jquery/dist"
    },
    {
      "library": "jquery-validate@1.20.0",
      "destination": "wwwroot/lib/jquery-validate"
    },
    {
      "library": "jquery-validation-unobtrusive@4.0.0",
      "destination": "wwwroot/lib/jquery-validation-unobtrusive"
    },
    {
      "library": "tinymce@7.6.1",
      "destination": "wwwroot/lib/tinymce-dist"
    },
    {
      "library": "jqueryui@1.14.1",
      "destination": "wwwroot/lib/jquery-ui"
    },
    {
      "library": "elfinder@2.1.65",
      "destination": "wwwroot/lib/elfinder"
    }
  ]
}
```


## 2. Chạy các lệnh sau trong PM

- ```dotnet tool install -g Microsoft.Web.LibraryManager.Cli```

- ```libman --version```

- Copy full path file libman.json: 
  - ```D:\WebsiteTour\hosting\web-tour\web-tour\libman.json```

- Chỉ lấy đường dẫn như dưới đây
  - ```cd "D:\WebsiteTour\hosting\web-tour\web-tour"```

- libman update jqueryui

- libman update elfinder

- libman restore


## 3. Các lệnh tải thư viện khác

```libman install bootstrap@5.3.0 --destination wwwroot/lib/bootstrap```

```libman install jquery@3.6.4 --destination wwwroot/lib/jquery```

```libman install popper.js@2.11.6 --destination wwwroot/lib/popper.js```

```libman install font-awesome@6.5.2 --destination wwwroot/lib/font-awesome```

```libman install bootstrap-icons@1.10.5 --destination wwwroot/lib/bootstrap-icons```

```libman install jquery-validation@1.20.0 --destination wwwroot/lib/jquery-validation```

```libman install jquery-validation-unobtrusive@3.2.12 --destination wwwroot/lib/jquery-validation-unobtrusive```

```libman install bootstrap-datepicker@1.10.2 --destination wwwroot/lib/bootstrap-datepicker```