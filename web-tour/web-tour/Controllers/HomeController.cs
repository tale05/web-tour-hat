using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using System.Globalization;
using System.IO;
using System.Linq;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Models;
using web_tour.Filters;

namespace web_tour.Controllers
{
    public class HomeController : Controller
    {
        DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;

        public HomeController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
        {
            _context = context;
            _imageSystemHelper = helper;
        }

        public IActionResult Index()
        {
            try
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

                var tours = (from t in _context.Tours
                             join c in _context.Categories on t.CategoryId equals c.CategoryId
                             join td in _context.Tourdetails on t.ToursId equals td.ToursId
                             where t.StatusTour == true && c.StatusCategory == true
                             select new TourForIndex
                             {
                                 IdTour = t.ToursId,
                                 NameTour = t.Title,
                                 ImgTour = t.ImgTitle,
                                 PriceAfter = td.PriceAfter.HasValue ? td.PriceAfter.Value.ToString("#,0", new CultureInfo("vi-VN")) + " ₫" : "Liên hệ",
                                 PriceBefore = td.PriceBefore.HasValue ? td.PriceBefore.Value.ToString("#,0", new CultureInfo("vi-VN")) + " ₫" : string.Empty,
                             }).Take(8).ToList();

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

                var listNews = (from n in _context.Newspapers
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
                               }).Take(8).ToList();

                var viewModel = new AllListViewModelsForIndexPage
                {
                    ListNameAndImageCategories = categories.Take(4).ToList(),
                    SliderImages = _imageSystemHelper.GetImageForSlider(),
                    ListTourForIndex = tours,
                    ListNameAndImageCategoriesForNavbar = categories,
                    CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
                    ListNewsForIndex = listNews,
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
