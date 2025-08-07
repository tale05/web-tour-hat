using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using web_tour.Controllers.Helpers;
using web_tour.Entities;
using web_tour.Filters;
using web_tour.Helpers;
using web_tour.Models;

namespace web_tour.Controllers
{
    public class TourController : Controller
    {
        private readonly DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;

        public TourController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
        {
            _context = context;
            _imageSystemHelper = helper;
        }

        [HttpGet("tour")]
        public IActionResult Index(int page = 1)
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

            var allTours = (from t in _context.Tours
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

            int pageSize = 9;
            int totalTours = allTours.Count();
            int totalPages = (int)Math.Ceiling((double)totalTours / pageSize);

            // Lấy danh sách tour theo trang
            var toursOnPage = allTours
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            // --- Bổ sung lấy danh sách tour đã xem gần đây từ cookie ---
            //var viewedToursCookie = Request.Cookies["viewedTours"];
            //var listTourRecentlyViewed = new List<TourForIndex>();

            //if (!string.IsNullOrEmpty(viewedToursCookie))
            //{
            //    var viewedTourIds = viewedToursCookie.Split(',', StringSplitOptions.RemoveEmptyEntries);

            //    // Lấy thông tin các tour đã xem gần đây (có thể giới hạn số lượng, ví dụ 5)
            //    listTourRecentlyViewed = allTours
            //        .Where(t => viewedTourIds.Contains(t.IdTour))
            //        .Take(5)
            //        .ToList();
            //}

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            var viewModel = new AllListViewModelsForIndexPage
            {
                SliderImages = _imageSystemHelper.GetImageForSlider(),
                ListNameAndImageCategories = categories,
                ListTourForIndex = toursOnPage,
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
                //ListTourRecentlyViewed = listTourRecentlyViewed
            };

            return View(viewModel);
        }

        // Tham số bắt buộc phải đứng trước

        [HttpGet("tour/tim-kiem-theo-danh-muc/{slug}")]
        public IActionResult GetListTourByCategory(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Index"); // hoặc trả về lỗi
            }

            var category = _context.Categories
                .Include(c => c.Tours)
                .AsEnumerable()
                .FirstOrDefault(c => SlugHelper.GenerateSlug(c.NameCategory) == slug);


            if (category == null)
            {
                return RedirectToAction("Index", "Tour");
            }

            ViewBag.LogoFile = _imageSystemHelper.GetLogoPath();
            ViewBag.BusinessLicenses = _imageSystemHelper.GetBusinessLicensePath();
            // Dùng CategoryId để Phân trang
            ViewBag.CategoryId = category.CategoryId.Trim().ToLower();

            var categories = _context.Categories
                .Where(ct => ct.StatusCategory == true)
                .Select(ct => new NameAndImageCategoryForIndex
                {
                    IdCategory = ct.CategoryId,
                    NameCategory = ct.NameCategory,
                    ImgCategory = ct.ImgCategory
                }).ToList();

            var culture = new CultureInfo("vi-VN");
            var allTours = (from t in _context.Tours
                            join c in _context.Categories on t.CategoryId equals c.CategoryId
                            join td in _context.Tourdetails on t.ToursId equals td.ToursId
                            where t.StatusTour == true
                            && t.CategoryId == category.CategoryId
                            && c.StatusCategory == true
                            select new TourForIndex
                            {
                                IdTour = t.ToursId,
                                NameTour = t.Title,
                                ImgTour = t.ImgTitle,
                                PriceAfter = td.PriceAfter.HasValue ? td.PriceAfter.Value.ToString("#,0", culture) + " ₫" : "Liên hệ",
                                PriceBefore = td.PriceBefore.HasValue ? td.PriceBefore.Value.ToString("#,0", culture) + " ₫" : string.Empty,
                            }).ToList();

            int pageSize = 9;
            int totalPages = (int)Math.Ceiling((double)allTours.Count / pageSize);
            var toursOnPage = allTours.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // --- Bổ sung lấy danh sách tour đã xem gần đây từ cookie ---
            //var viewedToursCookie = Request.Cookies["viewedTours"];
            //var listTourRecentlyViewed = new List<TourForIndex>();

            //if (!string.IsNullOrEmpty(viewedToursCookie))
            //{
            //    var viewedTourIds = viewedToursCookie.Split(',', StringSplitOptions.RemoveEmptyEntries);

            //    // Lấy thông tin các tour đã xem gần đây (có thể giới hạn số lượng, ví dụ 5)
            //    listTourRecentlyViewed = _context.Tours
            //    .Join(_context.Tourdetails, t => t.ToursId, td => td.ToursId, (t, td) => new { t, td })
            //    .Where(joined => viewedTourIds.Contains(joined.t.ToursId) && joined.t.StatusTour == true)
            //    .Select(joined => new TourForIndex
            //    {
            //        IdTour = joined.t.ToursId,
            //        NameTour = joined.t.Title,
            //        ImgTour = joined.t.ImgTitle,
            //        PriceAfter = joined.td.PriceAfter.HasValue ? joined.td.PriceAfter.Value.ToString("#,0", culture) + " ₫" : "Liên hệ",
            //        PriceBefore = joined.td.PriceBefore.HasValue ? joined.td.PriceBefore.Value.ToString("#,0", culture) + " ₫" : string.Empty,
            //    })
            //    .Take(5)
            //    .ToList();
            //}

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

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
                ListNameAndImageCategories = categories,
                ListTourForIndex = toursOnPage,
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
                //ListTourRecentlyViewed = listTourRecentlyViewed
            };

            return View("Index", viewModel);
        }

        // Tham số bắt buộc phải đứng trước
        // Thanh tìm kiếm trên navbar

        [HttpGet("tour/tim-kiem-theo-tour/")]
        public IActionResult GetTourByKeyword(string keyword, int page = 1)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index"); // hoặc trả về lỗi
            }

            ViewBag.LogoFile = _imageSystemHelper.GetLogoPath();
            ViewBag.BusinessLicenses = _imageSystemHelper.GetBusinessLicensePath();

            var categories = _context.Categories
                .Where(ct => ct.StatusCategory == true)
                .Select(ct => new NameAndImageCategoryForIndex
                {
                    IdCategory = ct.CategoryId,
                    NameCategory = ct.NameCategory,
                    ImgCategory = ct.ImgCategory
                }).ToList();

            var culture = new CultureInfo("vi-VN");
            var allTours = (from t in _context.Tours
                            join c in _context.Categories on t.CategoryId equals c.CategoryId
                            join td in _context.Tourdetails on t.ToursId equals td.ToursId
                            where t.StatusTour == true
                            && t.Title != null
                            && t.Title.Contains(keyword)
                            && c.StatusCategory == true
                            select new TourForIndex
                            {
                                IdTour = t.ToursId,
                                NameTour = t.Title,
                                ImgTour = t.ImgTitle,
                                PriceAfter = td.PriceAfter.HasValue ? td.PriceAfter.Value.ToString("#,0", culture) + " ₫" : "Liên hệ",
                                PriceBefore = td.PriceBefore.HasValue ? td.PriceBefore.Value.ToString("#,0", culture) + " ₫" : string.Empty,
                            }).ToList();

            int pageSize = 9;
            int totalPages = (int)Math.Ceiling((double)allTours.Count / pageSize);
            var toursOnPage = allTours.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

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
                ListNameAndImageCategories = categories,
                ListTourForIndex = toursOnPage,
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
            };

            return View("Index", viewModel);
        }


        [HttpGet("tour/chi-tiet-tour/{slug}")]
        public IActionResult TourDetail(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction("Index", "Tour");
            }

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

            // Duyệt toàn bộ tour và tạo slug từ title để tìm tour phù hợp
            var tour = _context.Tours
                .AsEnumerable()
                .FirstOrDefault(t => SlugHelper.GenerateSlug(t.Title) == slug);

            if (tour == null)
            {
                return RedirectToAction("Index", "Tour");
            }

            //// --- Bắt đầu phần thêm cookie lưu tour đã xem ---
            //var viewedTours = Request.Cookies["viewedTours"];
            //var listViewed = new List<string>();

            //if (!string.IsNullOrEmpty(viewedTours))
            //{
            //    listViewed = viewedTours.Split(',').ToList();
            //}

            //if (!listViewed.Contains(tour.ToursId))
            //{
            //    listViewed.Add(tour.ToursId);

            //    // Giới hạn số lượng tour đã xem lưu trong cookie tối đa 10
            //    if (listViewed.Count > 10)
            //    {
            //        listViewed.RemoveAt(0);
            //    }
            //}

            //var cookieOptions = new CookieOptions
            //{
            //    HttpOnly = true,
            //    IsEssential = true
            //};

            //Response.Cookies.Append("viewedTours", string.Join(",", listViewed), cookieOptions);
            //// --- Kết thúc phần thêm cookie ---

            var tourDetail = (from td in _context.Tourdetails
                              where td.ToursId == tour.ToursId
                              select new TourDetailFromIndexViewModel
                              {
                                  Title = tour.Title,
                                  ImgTitle = tour.ImgTitle,
                                  Description = td.DescriptionTour,
                                  Content = td.ContentTour,
                                  PriceAfter = td.PriceAfter.HasValue ? td.PriceAfter.Value.ToString("#,0", new CultureInfo("vi-VN")) + " ₫" : "Liên hệ",
                                  PriceBefore = td.PriceBefore.HasValue ? td.PriceBefore.Value.ToString("#,0", new CultureInfo("vi-VN")) + " ₫" : string.Empty,
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

            if (tourDetail == null)
            {
                return RedirectToAction("Index", "Tour");
            }

            var viewModel = new AllListViewModelsForIndexPage
            {
                SliderImages = _imageSystemHelper.GetImageForSlider(),
                TourDetailFromIndexViewModel = tourDetail,
                ListNameAndImageCategoriesForNavbar = categories,
                CompanyForIndexAbout = companyInfoForFooter ?? new CompanyForIndexAbout(),
            };

            return View(viewModel);
        }
    }
}