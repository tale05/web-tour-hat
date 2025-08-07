using Microsoft.AspNetCore.Mvc;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Filters;
using web_tour.Models;

namespace web_tour.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly string _imageFolder = "wwwroot/Files/Feedback";
        private readonly DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;

        public FeedbackController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
        {
            _context = context;
            _imageSystemHelper = helper;
        }

        
        [HttpGet("thu-muc-anh-phan-hoi")]
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

            var companyInfoForFooter = (from c in _context.Companies
                                        select new CompanyForIndexAbout
                                        {
                                            NameVie = c.NameVie,
                                            NameEng = c.NameEng,
                                            NameAbbr = c.NameAbbr,
                                            CompanyEmail = c.CompanyEmail,
                                            CompanyPhone = c.CompanyPhone,
                                            CompanyAddress = c.CompanyAddress,
                                            CompanyDescription = c.CompanyDescription,
                                            BusinessLicenseNo = c.BusinessLicenseNo,
                                            BusinessLicenseDate = c.BusinessLicenseDate.HasValue ? c.BusinessLicenseDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                                            IssuedBy = c.IssuedBy,
                                            InternationalTravelLicenseNo = c.InternationalTravelLicenseNo,
                                            InternationalTravelLicenseDate = c.InternationalTravelLicenseDate.HasValue ? c.InternationalTravelLicenseDate.Value.ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy"),
                                            FacebookUrl = c.FacebookUrl
                                        }).FirstOrDefault();

            var viewModel = new AllListViewModelsForIndexPage
            {
                SliderImages = _imageSystemHelper.GetImageForSlider(),
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
            };
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult LoadImages(int page = 1, int pageSize = 25)
        {
            var files = new DirectoryInfo(_imageFolder)
                .GetFiles()
                .OrderByDescending(f => f.CreationTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => f.Name)
                .ToList();

            return Json(files);
        }
    }
}