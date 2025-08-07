using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
namespace web_tour.Controllers;

public class FileManagerController : Controller
{
    private readonly DulichhatComDbtravelContext _context;
    private readonly SetupImageSystemHelper _imageSystemHelper;

    public FileManagerController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
    {
        _context = context;
        _imageSystemHelper = helper;
    }

    [Route("/file-manager")]
    public IActionResult Index()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        SetCompanyIconToViewBag();
        return View();
    }
    public IActionResult SelectImage()
    {
        return View();
    }
    private bool IsLoggedIn()
    {
        return HttpContext.Session.GetString("LoggedIn") == "true";
    }

    private void SetCompanyIconToViewBag()
    {
        string logoPath = _imageSystemHelper.GetLogoPath();
        if (!string.IsNullOrEmpty(logoPath))
        {
            ViewBag.CompanyIcon = logoPath;
        }
        else
        {
            ViewBag.CompanyIcon = Url.Content("~/images/logo_HAT.jpg");
        }
    }
}