using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace web_tour.Filters
{
    public class BotVerificationFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            if (FilterConfig.IgnoredControllers.Contains(controllerName))
            {
                return;
            }

            var httpContext = context.HttpContext;
            var path = httpContext.Request.Path.ToString().ToLower();
            var session = httpContext.Session;

            var ignorePaths = new[] {
                "/verify-bot",
                "/favicon.ico",
                "/robots.txt",
                "/sitemap.xml",
                "/checkip",
            };

            var isStaticFile = path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".png") ||
                               path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".svg") ||
                               path.EndsWith(".webp") || path.EndsWith(".woff2") || path.EndsWith(".eot");

            if (ignorePaths.Any(p => path.Contains(p)) || isStaticFile)
            {
                return;
            }

            var isVerified = session.GetString("BotVerified");

            // Nếu chưa xác minh thì chuyển hướng tới trang xác minh
            if (string.IsNullOrEmpty(isVerified) || isVerified != "true")
            {
                // Lưu URL gốc vào session để redirect lại sau khi xác minh
                session.SetString("ReturnUrlAfterVerify", httpContext.Request.Path + httpContext.Request.QueryString);

                context.Result = new RedirectToActionResult("VerifyBot", "BotVerification", null);
                //context.Result = new RedirectResult("/verify-bot");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}