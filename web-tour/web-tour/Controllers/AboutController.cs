using Microsoft.AspNetCore.Mvc;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Filters;
using web_tour.Models;

namespace web_tour.Controllers
{
    public class AboutController : Controller
    {
        private readonly DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;

        public AboutController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
        {
            _context = context;
            _imageSystemHelper = helper;
        }

        [HttpGet("gioi-thieu")]
        public IActionResult Index()
        {
            ViewBag.LogoFile = _imageSystemHelper.GetLogoPath();
            ViewBag.BusinessLicenses = _imageSystemHelper.GetBusinessLicensePath();

            var categories = (from ct in _context.Categories
                              where ct.StatusCategory == true
                              select new NameAndImageCategoryForIndex
                              {
                                  IdCategory = ct.CategoryId,
                                  NameCategory = ct.NameCategory,
                                  ImgCategory = ct.ImgCategory
                              }).ToList();

            var ci = _context.Companies.FirstOrDefault();
            if (ci == null)
            {
                return NotFound();
            }
            var companyInfo = new CompanyForIndexAbout
            {
                NameVie = ci.NameVie,
                NameEng = ci.NameEng,
                NameAbbr = ci.NameAbbr,
                CompanyEmail = ci.CompanyEmail,
                CompanyPhone = ci.CompanyPhone,
                CompanyAddress = ci.CompanyAddress,
                CompanyDescription = ci.CompanyDescription,
                BusinessLicenseNo = ci.BusinessLicenseNo,
                BusinessLicenseDate = ci.BusinessLicenseDate?.ToString("dd/MM/yyyy"),
                IssuedBy = ci.IssuedBy,
                InternationalTravelLicenseNo = ci.InternationalTravelLicenseNo,
                InternationalTravelLicenseDate = ci.InternationalTravelLicenseDate?.ToString("dd/MM/yyyy"),
                FacebookUrl = ci.FacebookUrl
            };


            var viewModel = new AllListViewModelsForIndexPage
            {
                SliderImages = _imageSystemHelper.GetImageForSlider(),
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfo,
            };

            return View(viewModel);
        }
    }
}
