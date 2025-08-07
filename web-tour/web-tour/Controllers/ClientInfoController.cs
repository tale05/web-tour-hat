using Microsoft.AspNetCore.Mvc;
using System;

using web_tour.Models;

namespace web_tour.Controllers
{
    public class ClientInfoController : Controller
    {
        [HttpGet("checkip")]
        public IActionResult Index()
        {
            var userAgent = Request.Headers["User-Agent"].ToString();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var browser = GetBrowserName(userAgent);
            var os = GetOperatingSystem(userAgent);
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            var model = new ClientInfoViewModel
            {
                IP = ip,
                Browser = browser,
                OS = os,
                UserAgent = userAgent,
                DateTimeNow = now
            };

            return View(model);
        }

        private string GetBrowserName(string userAgent)
        {
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) return "Safari";
            if (userAgent.Contains("Edge")) return "Edge";
            if (userAgent.Contains("MSIE") || userAgent.Contains("Trident")) return "Internet Explorer";
            return "Unknown";
        }

        private string GetOperatingSystem(string userAgent)
        {
            if (userAgent.Contains("Windows NT 10.0")) return "Microsoft Windows 10";
            if (userAgent.Contains("Windows NT 6.3")) return "Microsoft Windows 8.1";
            if (userAgent.Contains("Windows NT 6.1")) return "Microsoft Windows 7";
            if (userAgent.Contains("Mac OS X")) return "Mac OS X";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone")) return "iOS (iPhone)";
            return "Unknown";
        }
    }
}