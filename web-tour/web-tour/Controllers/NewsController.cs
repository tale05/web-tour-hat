using Microsoft.AspNetCore.Mvc;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Filters;
using web_tour.Helpers;
using web_tour.Models;

namespace web_tour.Controllers
{
    public class NewsController : Controller
    {
        private readonly DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;
        private readonly HashIdHelper _hashIdHelper;

        public NewsController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper, HashIdHelper hashIdHelper)
        {
            _context = context;
            _imageSystemHelper = helper;
            _hashIdHelper = hashIdHelper;
        }

        
        [HttpGet("bai-viet")]
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

            var allNews = (from n in _context.Newspapers
                           join c in _context.Companies on n.CompanyId equals c.CompanyId
                           orderby n.CreatedTime descending
                           select new NewsForIndex
                           {
                               IdNews = n.NewspaperId,
                               Company = c.NameVie,
                               ImgNews = n.ImgTitle,
                               Title = n.Title,
                               Content = n.Content,
                               CreatedTime = n.CreatedTime.HasValue ? n.CreatedTime.Value.ToString("dd/MM/yyyy") : null
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
                ListNewsForIndex = allNews,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
            };

            return View(viewModel);
        }

        // Xem docs về HashIdHelper để hiểu cách mã hóa và giải mã ID (Documents/HashIdHelper.txt)
        
        [HttpGet("bai-viet/chi-tiet-bai-viet/{encodedId}")]
        public IActionResult NewsDetail(string encodedId)
        {
            int id = _hashIdHelper.DecodeId(encodedId);
            if (id == 0)
                return NotFound();

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

            var newsDetail = (from n in _context.Newspapers
                              join c in _context.Companies on n.CompanyId equals c.CompanyId
                              where n.NewspaperId == id
                              select new NewsForIndex
                              {
                                  IdNews = n.NewspaperId,
                                  Company = c.NameVie,
                                  ImgNews = n.ImgTitle,
                                  Title = n.Title,
                                  Content = n.Content,
                                  CreatedTime = n.CreatedTime.HasValue ? n.CreatedTime.Value.ToString("dd/MM/yyyy") : null
                              }).FirstOrDefault();

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
                NewsForIndex = newsDetail ?? new NewsForIndex(),
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
            };

            return View(viewModel);
        }
    }
}
