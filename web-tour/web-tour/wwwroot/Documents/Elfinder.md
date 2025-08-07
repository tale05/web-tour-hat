# Hướng dẫn sử dụng Elfinder trong ASP.NET
## 📦 Cài đặt thư viện
- Muốn sử dụng elfinder phải có thư viện jqueryui và elfinder
- Làm các bước trong UseLibmanJsonToDownload.txt để tải jqueryui và elfinder
- Tải thêm thư viện elFinder.NetCore
  - dotnet add package elFinder.NetCore
  - Install-Package elFinder.NetCore -Version 1.4.0 --> Framework 8.0


## 1. Tạo FileSystemController.cs trong Controllers
- Cấu hình như sau:
```
using System;
using System.IO;
using System.Threading.Tasks;
using elFinder.NetCore;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace web_tour.Controllers
{
    // [Authorize] - bỏ comment user phải đăng nhập mới dùng được
    [Route("el-finder-file-system")]
    public class FileSystemController : Controller
    {
        IWebHostEnvironment _env;
        public FileSystemController(IWebHostEnvironment env) => _env = env;

        // Url để client-side kết nối đến backend
        // /el-finder-file-system/connector
        [Route("connector")]
        public async Task<IActionResult> Connector()
        {
            var connector = GetConnector();
            return await connector.ProcessAsync(Request);
        }

        // Địa chỉ để truy vấn thumbnail
        // /el-finder-file-system/thumb
        [Route("thumb/{hash}")]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }

        private Connector GetConnector()
        {
            // Thư mục gốc lưu trữ là wwwwroot/files (đảm bảo có tạo thư mục này)
            string pathroot = "files";

            var driver = new FileSystemDriver();

            string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
            var uri = new Uri(absoluteUrl);

            // .. ... wwww/files
            string rootDirectory = Path.Combine(_env.WebRootPath, pathroot);

            // https://localhost:5001/files/
            string url = $"{uri.Scheme}://{uri.Authority}/{pathroot}/";
            string urlthumb = $"{uri.Scheme}://{uri.Authority}/el-finder-file-system/thumb/";


            var root = new RootVolume(rootDirectory, url, urlthumb)
            {
                //IsReadOnly = !User.IsInRole("Administrators")
                IsReadOnly = false, // Can be readonly according to user's membership permission
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "Files", // Beautiful name given to the root/home folder
                //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
                //LockedFolders = new List<string>(new string[] { "Folder1" }
                ThumbnailSize = 100,
            };


            driver.AddRoot(root);

            // Thư mục "System"
            string pathRootSystem = "system";
            string rootDirectorySystem = Path.Combine(_env.WebRootPath, pathRootSystem);
            string urlSystem = $"{uri.Scheme}://{uri.Authority}/{pathRootSystem}/";
            string urlThumbSystem = $"{uri.Scheme}://{uri.Authority}/el-finder-file-system/thumb/";

            var rootSystem = new RootVolume(rootDirectorySystem, urlSystem, urlThumbSystem)
            {
                IsReadOnly = false,  // Chỉ đọc (nếu bạn muốn ngăn chỉnh sửa)
                IsLocked = false,    // Khóa thư mục
                Alias = "File_System",   // Tên hiển thị của root
                ThumbnailSize = 100 // Kích thước thumbnail
            };

            // Thêm root "System" vào driver
            driver.AddRoot(rootSystem);

            return new Connector(driver)
            {
                // This allows support for the "onlyMimes" option on the client.
                MimeDetect = MimeDetectOption.Internal
            };
        }
    }
}
```

## 2. Tạo 1 folder trong wwwroot tên là Files
## 3. Tạo 1 folder trong Views tên là FileManager/Index.cshtml
- Cấu hình như sau:
```
@{
    Layout = "~/Views/Shared/_DashboardLayout.cshtml";
}
<link href="~/css/filemanager/filemanager.css" rel="stylesheet" />

<div class="container-for-elfinder-custom">
    <h1>Quản lý tệp tin</h1>
    <div id="elfinder"></div>
</div>

@section Scripts {
    <link rel="stylesheet" href="~/lib/jquery-ui/themes/base/theme.css" />
    <link rel="stylesheet" href="~/lib/jquery-ui/themes/base/jquery-ui.css" />
    <link rel="stylesheet" href="~/lib/elfinder/css/elfinder.full.css" />
    <link rel="stylesheet" href="~/lib/elfinder/css/theme.min.css" />

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="/lib/jquery-ui/jquery-ui.min.js"></script>
    <script src="~/lib/elfinder/js/elfinder.min.js"></script>

    <script type="text/javascript">
        // Documentation for client options:
        // https://github.com/Studio-42/elFinder/wiki/Client-configuration-options
        $(document).ready(function () {
            var myCommands = elFinder.prototype._options.commands;

            // Not yet implemented commands in elFinder.NetCore
            var disabled = ['callback', 'chmod', 'editor', 'netmount', 'ping', 'search', 'zipdl', 'help'];
            elFinder.prototype.i18.en.messages.TextArea = "Edit";

            $.each(disabled, function (i, cmd) {
                (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
            });

            var options = {
                baseUrl: "@Url.Content("~/lib/elfinder/")",
                url: "/el-finder-file-system/connector",
                rememberLastDir: false,
                commands: myCommands,
                lang: 'vi',
                uiOptions: {
                    toolbar: [
                        ['back', 'forward'],
                        ['reload'],
                        ['home', 'up'],
                        ['mkdir', 'mkfile', 'upload'],
                        ['open', 'download'],
                        ['undo', 'redo'],
                        ['info'],
                        ['quicklook'],
                        ['copy', 'cut', 'paste'],
                        ['rm'],
                        ['duplicate', 'rename', 'edit'],
                        ['selectall', 'selectnone', 'selectinvert'],
                        ['view', 'sort']
                    ]
                },
                //onlyMimes: ["image", "text/plain"] // Get files of requested mime types only
                lang: 'vi',
            };
            $('#elfinder').elfinder(options).elfinder('instance');
        });
    </script>
}
```

- Nếu muốn tạo 1 view để chọn ảnh trả về đường link ảnh
- Tạo 1 view trong folder FileManager tên là SelectImage.cshtml
- Cấu hình như sau:
```
@{
    Layout = null;
}
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Chọn ảnh</title>
    <link rel="stylesheet" href="~/lib/jquery-ui/themes/base/theme.css" />
    <link rel="stylesheet" href="~/lib/jquery-ui/themes/base/jquery-ui.css" />
    <link rel="stylesheet" href="~/lib/elfinder/css/elfinder.full.css" />
    <link rel="stylesheet" href="~/lib/elfinder/css/theme.min.css" />

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/jquery-ui/jquery-ui.min.js"></script>
    <script src="~/lib/elfinder/js/elfinder.min.js"></script>
</head>
<body>
    <div id="elfinder"></div>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#elfinder').elfinder({
                baseUrl: '@Url.Content("~/lib/elfinder/")',
                url: '/el-finder-file-system/connector',
                lang: 'vi',
                getFileCallback: function (file) {
                    if (window.opener && typeof window.opener.SetImageUrl === "function") {
                        window.opener.SetImageUrl(file.url);
                        window.close();
                    } else {
                        alert("Không thể truyền đường dẫn về trang cha.");
                    }
                }
            });
        });
    </script>
</body>
</html>
```