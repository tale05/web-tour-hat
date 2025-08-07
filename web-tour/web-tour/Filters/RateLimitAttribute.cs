using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace web_tour.Filters
{
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        private const int MaxRequests = 5;
        private const int SecondsWindow = 5;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            var controllerName = context.Controller.GetType().Name;

            if (FilterConfig.IgnoredControllers.Contains(controllerName))
            {
                await next();
                return;
            }

            if (string.IsNullOrEmpty(ip))
            {
                await next();
                return;
            }

            // Lấy info truy cập trong 10 giây qua
            var accessInfo = _cache.Get<RateLimitInfo>(ip);

            if (accessInfo == null || DateTime.Now > accessInfo.ExpiresAt)
            {
                accessInfo = new RateLimitInfo
                {
                    Count = 1,
                    ExpiresAt = DateTime.Now.AddSeconds(SecondsWindow)
                };
            }
            else
            {
                accessInfo.Count++;
            }

            _cache.Set(ip, accessInfo, TimeSpan.FromSeconds(SecondsWindow));

            if (accessInfo.Count > MaxRequests)
            {
                // Khi vượt quá số lần cho phép → xoá xác thực & trả về 404
                session.Remove("BotVerified");
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }

        private class RateLimitInfo
        {
            public int Count { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
}