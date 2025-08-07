using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
namespace web_tour.Controllers.Helpers
{
    public class SetupImageSystemHelper
    {
        private readonly IWebHostEnvironment _env;

        public SetupImageSystemHelper(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string GetLogoPath()
        {
            var wwwRoot = _env.WebRootPath;
            var logoDir = Path.Combine(wwwRoot, "system", "icon");

            if (Directory.Exists(logoDir))
            {
                var extensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg" };
                var files = Directory.GetFiles(logoDir)
                                     .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                     .OrderBy(f => f)
                                     .ToList();

                if (files.Any())
                {
                    var fileName = Path.GetFileName(files.First());
                    return Path.Combine("/system/icon/", fileName).Replace("\\", "/");
                }
            }

            return null;
        }

        public List<string> GetBusinessLicensePath()
        {
            var result = new List<string>();
            var wwwRoot = _env.WebRootPath;
            var imgDir = Path.Combine(wwwRoot, "system", "giayphepkinhdoanh");

            if (Directory.Exists(imgDir))
            {
                var extensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg" };

                var files = Directory.GetFiles(imgDir)
                                     .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                     .OrderBy(f => f);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var virtualPath = Path.Combine("/system/giayphepkinhdoanh/", fileName).Replace("\\", "/");
                    result.Add(virtualPath);
                }
            }

            return result;
        }

        public List<string> GetImageForSlider()
        {
            var result = new List<string>();
            var wwwRoot = _env.WebRootPath;
            var imgDir = Path.Combine(wwwRoot, "system", "slide");

            if (Directory.Exists(imgDir))
            {
                var extensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".svg" };

                var files = Directory.GetFiles(imgDir)
                                     .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                                     .OrderBy(f => f);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var virtualPath = Path.Combine("/system/slide/", fileName).Replace("\\", "/");
                    result.Add(virtualPath);
                }
            }

            return result;
        }
    }
}
