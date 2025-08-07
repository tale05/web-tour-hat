using Microsoft.AspNetCore.Mvc;
using System.IO;
using Markdig;
using web_tour.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq;
using web_tour.Filters;
using web_tour.Models;
using Microsoft.Extensions.Options;

namespace web_tour.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly string _documentsPath;
        private readonly List<AccountConfig> _accounts;

        public DocumentsController(IOptions<List<AccountConfig>> accountsOptions, IWebHostEnvironment env)
        {
            _accounts = accountsOptions.Value;
            _documentsPath = Path.Combine(env.WebRootPath, "Documents");
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("LoggedIn") == "true";
        }

        [HttpGet("tai-lieu/dang-nhap")]
        public IActionResult Login()
        {
            if (IsLoggedIn())
                return RedirectToAction("Index", "Documents");
            else
                return View();
        }

        [HttpPost("tai-lieu/dang-nhap")]
        public IActionResult Login(string username, string password)
        {
            var matched = _accounts.FirstOrDefault(acc =>
                acc.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && acc.Password == password);

            if (matched == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return View();
            }

            HttpContext.Session.SetString("LoggedIn", "true");
            HttpContext.Session.SetString("Username", matched.Username);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("tai-lieu")]
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");

            var files = Directory.GetFiles(_documentsPath, "*.md")
                                 .Concat(Directory.GetFiles(_documentsPath, "*.txt"))
                                 .Select(Path.GetFileName)
                                 .ToList();

            return View(files);
        }

        [HttpGet("tai-lieu/chi-tiet-tai-lieu/{fileName}")]
        public IActionResult ViewDoc(string fileName)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");

            if (string.IsNullOrEmpty(fileName)) return NotFound();

            var fullPath = Path.Combine(_documentsPath, fileName);
            if (!System.IO.File.Exists(fullPath)) return NotFound();

            var markdown = System.IO.File.ReadAllText(fullPath);
            var html = Markdown.ToHtml(markdown);

            ViewBag.FileName = fileName;
            ViewBag.HtmlContent = html;
            return View();
        }
    }
}