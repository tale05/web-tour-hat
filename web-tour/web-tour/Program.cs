using Microsoft.EntityFrameworkCore;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Filters;
using web_tour.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add<RateLimitAttribute>();
//    options.Filters.Add<BotVerificationFilter>();
//});

builder.Services.AddDbContext<DulichhatComDbtravelContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<SetupImageSystemHelper>();
builder.Services.AddSingleton<web_tour.Helpers.HashIdHelper>();
builder.Services.Configure<List<AccountConfig>>(builder.Configuration.GetSection("Accounts"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
    }
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tour-detail",
    pattern: "tour/chi-tiet-tour/{slug}",
    defaults: new { controller = "Tour", action = "TourDetail" });

app.MapControllerRoute(
    name: "category-slug",
    pattern: "tour/tim-kiem-theo-danh-muc/{slug}",
    defaults: new { controller = "Tour", action = "GetListTourByCategory" });

app.Run();