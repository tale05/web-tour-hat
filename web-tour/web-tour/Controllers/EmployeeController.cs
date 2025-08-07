using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_tour.Entities;
using web_tour.Models;
using web_tour.Models.Company;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using System.Data;
using web_tour.Controllers.Helpers;
using BCrypt.Net;
using web_tour.Filters;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace web_tour.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DulichhatComDbtravelContext _context;
        private readonly SetupImageSystemHelper _imageSystemHelper;

        public EmployeeController(DulichhatComDbtravelContext context, SetupImageSystemHelper helper)
        {
            _context = context;
            _imageSystemHelper = helper;
        }
        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("LoggedIn") == "true";
        }
        private string? GetLoggedInCompanyId()
        {
            return HttpContext.Session.GetString("CompanyId");
        }
        private string? GetLoggedInNameEmployee()
        {
            return HttpContext.Session.GetString("NameEmployee");
        }
        private string? GetLoggedInIdEmployee()
        {
            return HttpContext.Session.GetString("EmployeeId");
        }
        private bool ContainsHtmlOrScript(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Regex phát hiện thẻ HTML, CSS, hoặc JS cơ bản
            var pattern = @"(<[^>]+>|script|style|on\w+=|javascript:)";
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        [HttpPost("admin")]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ContainsHtmlOrScript(username) || ContainsHtmlOrScript(password))
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu đúng định dạng";
                return RedirectToAction("Login");
            }

            // Xác thực gRecaptcha
            var gRecaptchaResponse = Request.Form["g-recaptcha-response"];
            var secret = "6Lf6v1crAAAAABc6tI47GiTZpS-540eh2gN0uN9j";

            using var client = new HttpClient();
            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", gRecaptchaResponse)
            });

            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", postData);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            if (result.success != true)
            {
                TempData["Error"] = "Xác minh không thành công.";
                return RedirectToAction("Login");
            }

            // Các bước kiểm tra tài khoản và mật khẩu
            var accountEmployee = _context.Employees
                .FirstOrDefault(e => e.Username == username.Trim());

            // Kiểm tra tài khoản tồn tại trước
            if (accountEmployee == null)
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return RedirectToAction("Login");
            }

            // Kiểm tra mật khẩu
            bool checkLogin = BCrypt.Net.BCrypt.Verify(password.Trim(), accountEmployee.Password);
            if (!checkLogin)
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return RedirectToAction("Login");
            }

            // Nếu mật khẩu đúng thì gán session
            HttpContext.Session.SetString("LoggedIn", "true");

            if (string.IsNullOrEmpty(accountEmployee.CompanyId))
            {
                TempData["Error"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return RedirectToAction("Login");
            }

            // Lưu session
            HttpContext.Session.SetString("CompanyId", accountEmployee.CompanyId);
            HttpContext.Session.SetString("NameEmployee", $"{accountEmployee.LastNameEmployee} {accountEmployee.FirstNameEmployee}");
            HttpContext.Session.SetString("EmployeeId", accountEmployee.EmployeeId.Trim());

            return RedirectToAction("CompanyInfo");
        }

        private void SetAllPropertiesAfterLogin()
        {
            ViewBag.NameEmployee = GetLoggedInNameEmployee();
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
        private async Task<byte[]> ProcessUploadedFileAsync(IFormFile uploadedFile, string oldBase64)
        {
            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await uploadedFile.CopyToAsync(ms);
                return ms.ToArray();
            }
            else if (!string.IsNullOrEmpty(oldBase64))
            {
                return Convert.FromBase64String(oldBase64);
            }
            return null;
        }
        
        [HttpGet("admin")]
        public IActionResult Login()
        {
            if (IsLoggedIn()) return RedirectToAction("CompanyInfo");
            return View();
        }
        public IActionResult Dashboard()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> CompanyInfo()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            var companies = await _context.Companies.ToListAsync();
            if (companies == null || !companies.Any())
            {
                Console.WriteLine("Không có công ty nào.");
                return View("Error");
            }
            var firstCompany = companies.First();
            CompanyModel companyToUpdate = new CompanyModel
            {
                CompanyId = firstCompany.CompanyId,
                NameVie = firstCompany.NameVie,
                NameEng = firstCompany.NameEng,
                NameAbbr = firstCompany.NameAbbr,
                CompanyEmail = firstCompany.CompanyEmail,
                CompanyPhone = firstCompany.CompanyPhone,
                CompanyAddress = firstCompany.CompanyAddress,
                CompanyDescription = firstCompany.CompanyDescription,
                BusinessLicenseNo = firstCompany.BusinessLicenseNo,
                BusinessLicenseDate = firstCompany.BusinessLicenseDate.HasValue
                                        ? firstCompany.BusinessLicenseDate.Value.ToDateTime(TimeOnly.MinValue)
                                        : (DateTime?)null,
                IssuedBy = firstCompany.IssuedBy,
                InternationalTravelLicenseNo = firstCompany.InternationalTravelLicenseNo,
                InternationalTravelLicenseDate = firstCompany.InternationalTravelLicenseDate.HasValue
                                        ? firstCompany.InternationalTravelLicenseDate.Value.ToDateTime(TimeOnly.MinValue)
                                        : (DateTime?)null,
                FacebookUrl = firstCompany.FacebookUrl,
            };

            return View(companyToUpdate);
        }
        public async Task<IActionResult> CategoryManagement()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            var categories = await _context.Categories.ToListAsync();
            if (categories == null || !categories.Any())
            {
                Console.WriteLine("Không có danh mục nào.");
                TempData["Error"] = "Không có danh mục nào.";
                return View("Error");
            }

            var categoryModels = categories.Select(c => new CategoryModel
            {
                CategoryId = c.CategoryId,
                NameCategory = c.NameCategory,
                ImgCategory = c.ImgCategory,
                StatusCategory = c.StatusCategory
            }).ToList();

            return View(categoryModels);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeCompanyInfo(CompanyModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            if (string.IsNullOrWhiteSpace(model.CompanyPhone) || !System.Text.RegularExpressions.Regex.IsMatch(model.CompanyPhone, @"^\d{10}$"))
            {
                TempData["Error"] = "Số điện thoại không hợp lệ. Vui lòng nhập đúng 10 chữ số.";
                return RedirectToAction("CompanyInfo");
            }
            if (ModelState.IsValid)
            {
                

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyId == model.CompanyId);
                if (company != null)
                {
                    company.NameVie = model.NameVie;
                    company.NameEng = model.NameEng;
                    company.NameAbbr = model.NameAbbr;
                    company.CompanyEmail = model.CompanyEmail;
                    company.CompanyPhone = model.CompanyPhone;
                    company.CompanyAddress = model.CompanyAddress;
                    company.CompanyDescription = model.CompanyDescription;
                    company.BusinessLicenseNo = model.BusinessLicenseNo;
                    company.BusinessLicenseDate = model.BusinessLicenseDate.HasValue
                                                ? DateOnly.FromDateTime(model.BusinessLicenseDate.Value)
                                                : (DateOnly?)null;
                    company.IssuedBy = model.IssuedBy;
                    company.InternationalTravelLicenseNo = model.InternationalTravelLicenseNo;
                    company.InternationalTravelLicenseDate = model.InternationalTravelLicenseDate.HasValue
                                                ? DateOnly.FromDateTime(model.InternationalTravelLicenseDate.Value)
                                                : (DateOnly?)null;
                    company.FacebookUrl = model.FacebookUrl;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã lưu thông tin thành công!";
                    return RedirectToAction("CompanyInfo");
                }
                TempData["Error"] = "Không tìm thấy công ty để cập nhật!";
                return RedirectToAction("CompanyInfo");
            }
            var errorMessages = ModelState
                .Where(ms => ms.Value.Errors.Any())
                .Select(ms => $"{ms.Key}: {string.Join(", ", ms.Value.Errors.Select(e => e.ErrorMessage))}");

            TempData["Error"] = "Fail. Error:\n" + string.Join("\n", errorMessages);
            return View("CompanyInfo", model);
        }
        [HttpGet]
        public ActionResult EditCategory(string id)
        {
            SetAllPropertiesAfterLogin();
            if (!IsLoggedIn()) return RedirectToAction("Login");
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View("CategoryEdit", category);
        }
        [HttpPost]
        public IActionResult UpdateCategoryStatus(string categoryId, string imgCategory, bool status)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");

            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category == null)
            {
                TempData["Error"] = "Danh mục không tồn tại!";
                return RedirectToAction("CategoryManagement");
            }

            category.StatusCategory = status;
            _context.SaveChanges();
            TempData["Success"] = "Trạng thái danh mục đã được cập nhật!";

            return RedirectToAction("CategoryManagement");
        }
        [HttpPost]
        public async Task<ActionResult> UpdateCategory(Category model)
        {
            ModelState.Remove("ImageUpload");
            ModelState.Remove("OldImgCategory");
            if (ModelState.IsValid)
            {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == model.CategoryId);
                if (category != null)
                {
                    category.NameCategory = model.NameCategory;
                    category.StatusCategory = model.StatusCategory;
                    category.ImgCategory = model.ImgCategory;

                    _context.SaveChanges();
                }

                TempData["Success"] = "Đã cập nhật thông tin thành công!";
                return RedirectToAction("CategoryManagement", "Employee");
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage)
                                     .ToList();

            TempData["Error"] = "Cập nhật thất bại: " + string.Join(" | ", allErrors);
            return View("CategoryEdit", model);
        }
        [HttpGet]
        public ActionResult RemoveCategory(string id)
        {
            try
            {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category == null)
                {
                    TempData["Error"] = "Danh mục không tồn tại.";
                    return RedirectToAction("CategoryManagement");
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();

                TempData["Success"] = "Xóa danh mục thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa danh mục thất bại: " + ex.Message;
            }

            return RedirectToAction("CategoryManagement");
        }
        public IActionResult CategoryInsert()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddCategory(string NameCategory, string ImgCategory)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            if (string.IsNullOrEmpty(ImgCategory))
            {
                ViewData["Error"] = "Vui lòng chọn ảnh danh mục.";
                return View("CategoryInsert");
            }
            if (!ModelState.IsValid)
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();

                ViewData["Error"] = "Thêm danh mục thất bại: " + string.Join(" | ", allErrors);
                return View("CategoryInsert");
            }
            try
            {
                var nameParam = new SqlParameter("@NameCategory", NameCategory ?? (object)DBNull.Value);
                var imgParam = new SqlParameter("@ImgCategory", ImgCategory ?? (object)DBNull.Value);
                var statusParam = new SqlParameter("@StatusCategory", 1);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC InsertCategory @NameCategory, @ImgCategory, @StatusCategory",
                    nameParam, imgParam, statusParam
                );

                TempData["Success"] = "Đã thêm danh mục thành công!";
                return RedirectToAction("CategoryManagement", "Employee");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Lỗi khi thêm danh mục: " + ex.Message;
                return View("CategoryInsert");
            }
        }
        public async Task<IActionResult> TourManagement(int page = 1)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            var totalToursToView = await _context.Tours.CountAsync();
            ViewBag.TotalRecords = totalToursToView;

            int pageSize = 10;
            var totalTours = await _context.Tours.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalTours / pageSize);

            var tours = await (from t in _context.Tours
                               join c in _context.Categories on t.CategoryId equals c.CategoryId
                               orderby t.ToursId
                               select new TourManagementViewModel
                               {
                                   ToursId = t.ToursId,
                                   Title = t.Title,
                                   ImgTitle = t.ImgTitle,
                                   StatusTour = t.StatusTour ?? true,
                                   CategoryName = c.NameCategory
                               })
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            var model = new TourListManagementViewModel
            {
                Tours = tours,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(model);
        }
        public async Task<IActionResult> SearchTour(string keyword = "", int page = 1)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("TourManagement");
            }

            int pageSize = 10;

            var query = from t in _context.Tours
                        join c in _context.Categories on t.CategoryId equals c.CategoryId
                        where t.Title.Contains(keyword)
                        orderby t.ToursId
                        select new TourManagementViewModel
                        {
                            ToursId = t.ToursId,
                            Title = t.Title,
                            ImgTitle = t.ImgTitle,
                            StatusTour = t.StatusTour ?? true,
                            CategoryName = c.NameCategory
                        };

            ViewBag.TotalRecords = query.Count();

            var totalTours = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalTours / pageSize);

            var tours = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new TourListManagementViewModel
            {
                Tours = tours,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View("TourManagement", model);
        }

        [HttpGet]
        public IActionResult ToggleTourStatus(string id)
        {
            var tour = _context.Tours.FirstOrDefault(t => t.ToursId == id);
            if (tour == null)
            {
                TempData["Error"] = "Cập nhật bị lỗi.";
                return RedirectToAction("TourManagement");
            }

            tour.StatusTour = !tour.StatusTour;
            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thành công";
            return RedirectToAction("TourManagement");
        }
        public IActionResult TourInsert()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            var model = new TourInsertViewModel
            {
                Categories = _context.Categories
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        NameCategory = c.NameCategory
                    }).ToList()
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult AddTour(TourInsertViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.ImgProduct))
            {
                // Load lại danh sách Categories cho view
                model.Categories = _context.Categories
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        NameCategory = c.NameCategory
                    }).ToList();

                // Kiểm tra lỗi thiếu ảnh
                if (string.IsNullOrEmpty(model.ImgProduct))
                {
                    SetAllPropertiesAfterLogin();
                    ViewData["Error"] = "Vui lòng chọn ảnh cho tour.";
                }

                return View("TourInsert", model);
            }

            var parameters = new[]
            {
                new SqlParameter("@CategoryId", model.CategoryId),
                new SqlParameter("@Title", model.Title),
                new SqlParameter("@ImgTitle", model.ImgProduct),
                new SqlParameter("@StatusTour", model.StatusProduct)
            };

            _context.Database.ExecuteSqlRaw(
                "EXEC InsertTour @CategoryId, @Title, @ImgTitle, @StatusTour", parameters
            );

            TempData["Success"] = "Thêm tour thành công!";
            return RedirectToAction("TourManagement");
        }
        [HttpGet]
        public ActionResult RemoveProduct(string id)
        {
            try
            {
                var productDetail = _context.Tourdetails.FirstOrDefault(c => c.ToursId == id);
                if (productDetail != null)
                {
                    _context.Tourdetails.Remove(productDetail);
                    _context.SaveChanges();
                }
                var product = _context.Tours.FirstOrDefault(c => c.ToursId == id);
                if (product == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại.";
                    return RedirectToAction("TourManagement");
                }

                _context.Tours.Remove(product);
                _context.SaveChanges();

                TempData["Success"] = "Xóa sản phẩm thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa sản phẩm thất bại: " + ex.Message;
            }

            return RedirectToAction("TourManagement");
        }
        [HttpGet]
        public ActionResult EditProduct(string id)
        {
            SetAllPropertiesAfterLogin();
            if (!IsLoggedIn()) return RedirectToAction("Login");

            var tour = _context.Tours.FirstOrDefault(t => t.ToursId == id);
            if (tour == null)
            {
                return NotFound();
            }

            var viewModel = new TourUpdateViewModel
            {
                TourId = tour.ToursId,
                Title = tour.Title ?? "",
                CategoryId = tour.CategoryId ?? "",
                StatusProduct = tour.StatusTour ?? true,
                ImgTitle = tour.ImgTitle,
                Categories = _context.Categories.ToList()
            };

            return View("TourEdit", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTour(TourUpdateViewModel model)
        {
            SetAllPropertiesAfterLogin();
            if (!IsLoggedIn()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                model.Categories = _context.Categories.ToList();
                return View("TourEdit", model);
            }

            var tour = _context.Tours.FirstOrDefault(t => t.ToursId == model.TourId);
            if (tour == null)
            {
                return NotFound();
            }

            tour.Title = model.Title;
            tour.CategoryId = model.CategoryId;
            tour.StatusTour = model.StatusProduct;
            tour.ImgTitle = model.ImgTitle;

            _context.Update(tour);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật tour thành công.";
            return RedirectToAction("TourManagement");
        }
        public IActionResult TourDetailManager(string idTour, string title)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            ViewBag.Title = title;

            var tourDetail = (from td in _context.Tourdetails
                              join tr in _context.Tours on td.ToursId equals tr.ToursId
                              where td.ToursId == idTour
                              select new TourDetailViewModel
                              {
                                  TourDetailId = td.TourdetailId,
                                  TourId = idTour,
                                  Description = td.DescriptionTour,
                                  Content = td.ContentTour,
                                  PriceBefore = td.PriceBefore,
                                  PriceAfter = td.PriceAfter
                              }).FirstOrDefault();

            if (tourDetail == null)
            {
                tourDetail = new TourDetailViewModel
                {
                    TourId = idTour,
                    Description = "Chưa có thông tin",
                    Content = "Chưa có thông tin",
                    PriceBefore = null,
                    PriceAfter = null
                };
            }
            return View("TourDetailManager", tourDetail);
        }
        [HttpPost]
        public ActionResult InsertOrUpdateTourDetail(TourDetailViewModel model, string actionType)
        {
            if (actionType == "create")
            {
                var newTourDetail = new Tourdetail
                {
                    ToursId = model.TourId,
                    DescriptionTour = model.Description,
                    ContentTour = model.Content,
                    PriceBefore = model.PriceBefore,
                    PriceAfter = model.PriceAfter
                };

                _context.Tourdetails.Add(newTourDetail);
                _context.SaveChanges();

                TempData["Success"] = "Đã tạo mới chi tiết tour thành công.";
            }
            else if (actionType == "update")
            {
                int id = model.TourDetailId ?? 0;
                var existingTourDetail = _context.Tourdetails.FirstOrDefault(t => t.TourdetailId == id);

                if (existingTourDetail != null)
                {
                    existingTourDetail.DescriptionTour = model.Description;
                    existingTourDetail.ContentTour = model.Content;
                    existingTourDetail.PriceBefore = model.PriceBefore;
                    existingTourDetail.PriceAfter = model.PriceAfter;

                    _context.SaveChanges();

                    TempData["Success"] = "Đã cập nhật thông tin chi tiết tour thành công.";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy bản ghi cần cập nhật.";
                }
            }

            return RedirectToAction("TourManagement", "Employee");
        }
        public async Task<IActionResult> EmployeeManager()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            SetAllPropertiesAfterLogin();

            string IdEmployee = GetLoggedInIdEmployee();

            List<Employee> employees;

            if (IdEmployee == "NV002")
            {
                // Hiển thị toàn bộ danh sách nếu là admin
                employees = await _context.Employees.ToListAsync();
            }
            else
            {
                // Nếu không phải admin thì loại bỏ NV002 khỏi danh sách
                employees = await _context.Employees
                    .Where(e => e.EmployeeId != "NV002")
                    .ToListAsync();
            }

            if (employees == null || !employees.Any())
            {
                var noDataModel = new EmployeeModel
                {
                    IdEmployee = "",
                    FirstName = "Chưa có nhân viên nào",
                    LastName = "",
                    Email = "",
                    UserName = "",
                };
                return View(new List<EmployeeModel> { noDataModel });
            }

            var employeeModels = employees.Select(em => new EmployeeModel
            {
                IdEmployee = em.EmployeeId?.Trim(),
                FirstName = em.FirstNameEmployee,
                LastName = em.LastNameEmployee,
                Email = em.Email + " / " + em.Phone,
                UserName = em.Username,
            }).ToList();

            return View(employeeModels);
        }
        public IActionResult EmployeeInsert()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            return View();
        }
        [HttpPost]
        public ActionResult EmployeeInsert(EmployeeModel modelFromView)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            if (!ModelState.IsValid)
            {
                return View(modelFromView);
            }

            var companyId = GetLoggedInCompanyId();
            string username = modelFromView.UserName?.Trim();

            // ✅ Kiểm tra username đã tồn tại chưa
            bool isUsernameExist = _context.Employees.Any(e => e.Username == username);
            if (isUsernameExist)
            {
                ViewData["Error"] = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.";
                return View(modelFromView);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(modelFromView.Password);

            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@CompanyId", companyId),
                    new SqlParameter("@FirstNameEmployee", modelFromView.FirstName),
                    new SqlParameter("@LastNameEmployee", modelFromView.LastName),
                    new SqlParameter("@Email", modelFromView.Email),
                    new SqlParameter("@Phone", modelFromView.Phone),
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", hashedPassword)
                };

                _context.Database.ExecuteSqlRaw("EXEC InsertEmployee @CompanyId, @FirstNameEmployee, @LastNameEmployee, @Email, @Phone, @Username, @Password", parameters);

                TempData["Success"] = "Thêm nhân viên thành công!";
            }
            catch (Exception)
            {
                TempData["Error"] = "Thêm nhân viên thất bại!";
                return View(modelFromView);
            }

            return RedirectToAction("EmployeeManager");
        }
        public IActionResult EmployeeEdit(string id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new EmployeeModel
            {
                IdEmployee = employee.EmployeeId,
                FirstName = employee.FirstNameEmployee,
                LastName = employee.LastNameEmployee,
                Email = employee.Email,
                Phone = employee.Phone,
                UserName = employee.Username,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmployeeEdit(EmployeeModel modelFromView)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == modelFromView.IdEmployee);
            if (employee == null)
            {
                ViewData["Error"] = "Không tìm thấy nhân viên";
                return RedirectToAction("EmployeeManager");
            }

            // Kiểm tra rỗng
            if (string.IsNullOrWhiteSpace(modelFromView.FirstName) || string.IsNullOrWhiteSpace(modelFromView.LastName) ||
                string.IsNullOrWhiteSpace(modelFromView.Email) || string.IsNullOrWhiteSpace(modelFromView.Phone) ||
                string.IsNullOrWhiteSpace(modelFromView.UserName))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ thông tin.";
                return View(modelFromView);
            }

            if (string.IsNullOrWhiteSpace(modelFromView.Phone) ||
                !System.Text.RegularExpressions.Regex.IsMatch(modelFromView.Phone, @"^\d{10}$"))
            {
                ViewData["Error"] = "Số điện thoại không hợp lệ. Vui lòng nhập đúng 10 chữ số.";
                return View(modelFromView);
            }

            string username = modelFromView.UserName?.Trim();

            // ✅ Kiểm tra username đã tồn tại chưa
            bool isUsernameExist = _context.Employees.Any(e => e.Username == username);
            if (isUsernameExist)
            {
                ViewData["Error"] = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.";
                return View(modelFromView);
            }

            if (ModelState.IsValid)
            {
                employee.FirstNameEmployee = modelFromView.FirstName;
                employee.LastNameEmployee = modelFromView.LastName;
                employee.Email = modelFromView.Email;
                employee.Phone = modelFromView.Phone;
                employee.Username = modelFromView.UserName;

                if (!string.IsNullOrWhiteSpace(modelFromView.Password))
                {
                    employee.Password = BCrypt.Net.BCrypt.HashPassword(modelFromView.Password);
                }

                _context.SaveChanges();
                TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
                return RedirectToAction("EmployeeManager");
            }

            ViewData["Error"] = "Cập nhật thông tin nhân viên thất bại!";
            return View(modelFromView);
        }
        public async Task<IActionResult> NewsManager()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            var news = await _context.Newspapers.ToListAsync();
            if (news == null || !news.Any())
            {
                var noDataModel = new NewsModel
                {
                    IdNews = null,
                    IdCompany = "",
                    Title = "Chưa có bài viết nào",
                    ImgNews = null,
                    Content = "",
                    CreatedTime = null,
                };
                return View(new List<NewsModel> { noDataModel });
            }
            else
            {
                var newsModels = news.Select(n => new NewsModel
                {
                    IdNews = n.NewspaperId,
                    IdCompany = n.CompanyId,
                    Title = n.Title,
                    ImgNews = n.ImgTitle,
                    Content = n.Content,
                    CreatedTime = n.CreatedTime,
                }).ToList();
                return View(newsModels);
            }
        }
        public IActionResult NewsInsert()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> NewsInsert(NewsModel modelFromView)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            // Kiểm tra rỗng
            if (!ModelState.IsValid ||
                string.IsNullOrWhiteSpace(modelFromView.Title) ||
                string.IsNullOrWhiteSpace(modelFromView.Content))
            {
                ViewData["Error"] = "Tiêu đề và nội dung không được để trống.";
                return View(modelFromView);
            }

            if (string.IsNullOrEmpty(modelFromView.ImgNews))
            {
                SetAllPropertiesAfterLogin();
                ViewData["Error"] = "Vui lòng chọn ảnh cho tour.";
                return View(modelFromView);
            }

            try
            {
                string companyId = GetLoggedInCompanyId() ?? "";
                modelFromView.IdCompany = companyId;

                var newNews = new Newspaper
                {
                    CompanyId = modelFromView.IdCompany,
                    Title = modelFromView.Title,
                    ImgTitle = modelFromView.ImgNews,
                    Content = modelFromView.Content,
                    CreatedTime = DateTime.Now
                };

                _context.Newspapers.Add(newNews);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm bài viết thành công!";
                return RedirectToAction("NewsManager");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Lỗi khi thêm bài viết: " + ex.Message;
                return View(modelFromView);
            }
        }
        public IActionResult NewsEdit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            var news = _context.Newspapers.FirstOrDefault(e => e.NewspaperId == id);
            if (news == null)
            {
                return NotFound();
            }
            var model = new NewsModel
            {
                IdNews = news.NewspaperId,
                IdCompany = news.CompanyId,
                Title = news.Title,
                ImgNews = news.ImgTitle,
                Content = news.Content,
                CreatedTime = news.CreatedTime,
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsEdit(NewsModel modelFromView)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login");
            SetAllPropertiesAfterLogin();

            // Kiểm tra null hoặc rỗng
            if (string.IsNullOrWhiteSpace(modelFromView.Title) || string.IsNullOrWhiteSpace(modelFromView.Content))
            {
                ViewData["Error"] = "Tiêu đề và nội dung không được để trống.";
                return View(modelFromView);
            }

            // Lấy bài viết cần cập nhật
            var news = _context.Newspapers.FirstOrDefault(e => e.NewspaperId == modelFromView.IdNews);
            if (news == null)
            {
                TempData["Error"] = "Không tìm thấy bài viết cần cập nhật.";
                return RedirectToAction("NewsManager");
            }

            // Nếu hợp lệ thì cập nhật
            string companyId = GetLoggedInCompanyId() ?? "";
            modelFromView.IdCompany = companyId;
            if (ModelState.IsValid)
            {
                news.CompanyId = modelFromView.IdCompany;
                news.Title = modelFromView.Title.Trim();
                news.ImgTitle = modelFromView.ImgNews;
                news.Content = modelFromView.Content.Trim();
                news.CreatedTime = DateTime.Now;

                _context.SaveChanges();

                TempData["Success"] = "Cập nhật thông tin bài viết thành công!";
                return RedirectToAction("NewsManager");
            }

            ViewData["Error"] = "Cập nhật thông tin bài viết thất bại. Vui lòng kiểm tra lại.";
            return View(modelFromView);
        }
        [HttpGet]
        public ActionResult RemoveNews(int id)
        {
            try
            {
                var news = _context.Newspapers.FirstOrDefault(c => c.NewspaperId == id);
                if (news == null)
                {
                    TempData["Error"] = "Bài viết không tồn tại.";
                    return RedirectToAction("NewsManager");
                }

                _context.Newspapers.Remove(news);
                _context.SaveChanges();

                TempData["Success"] = "Xóa bài viết thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Xóa bài viết thất bại: " + ex.Message;
            }

            return RedirectToAction("NewsManager");
        }
    }
}