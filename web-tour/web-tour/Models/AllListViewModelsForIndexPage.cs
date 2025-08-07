namespace web_tour.Models
{
    public class NameAndImageCategoryForIndex
    {
        public string? IdCategory { get; set; }
        public string? NameCategory { get; set; }
        public string? ImgCategory { get; set; }
    }

    public class TourForIndex
    {
        public string? IdTour { get; set; }
        public string? NameTour { get; set; }
        public string? ImgTour { get; set; }
        public string? PriceAfter { get; set; }
        public string? PriceBefore { get; set; }
    }

    public class TourDetailFromIndexViewModel
    {
        public string? Title { get; set; }
        public string? ImgTitle { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? PriceAfter { get; set; }
        public string? PriceBefore { get; set; }
    }

    public class CompanyForIndexAbout
    {
        public string? NameVie { get; set; }
        public string? NameEng { get; set; }
        public string? NameAbbr { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyDescription { get; set; }
        public string? BusinessLicenseNo { get; set; }
        public string? BusinessLicenseDate { get; set; }
        public string? IssuedBy { get; set; }
        public string? InternationalTravelLicenseNo { get; set; }
        public string? InternationalTravelLicenseDate { get; set; }
        public string? FacebookUrl { get; set; }
    }

    public class NewsForIndex
    {
        public int? IdNews { get; set; }
        public string? Company { get; set; }
        public string? ImgNews { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? CreatedTime { get; set; }
    }

    public class AllListViewModelsForIndexPage
    {
        public List<NameAndImageCategoryForIndex> ListNameAndImageCategories { get; set; }
        public List<TourForIndex> ListTourForIndex { get; set; }
        public List<string> SliderImages { get; set; }
        public TourDetailFromIndexViewModel TourDetailFromIndexViewModel { get; set; }
        public List<NameAndImageCategoryForIndex> ListNameAndImageCategoriesForNavbar { get; set; }
        public CompanyForIndexAbout CompanyForIndexAbout { get; set; }
        public List<NewsForIndex> ListNewsForIndex { get; set; }
        public NewsForIndex NewsForIndex { get; set; }
        public List<string> ListDocuments { get; set; }
        public List<TourForIndex> ListTourRecentlyViewed { get; set; }

        public AllListViewModelsForIndexPage()
        {
            ListNameAndImageCategories = new List<NameAndImageCategoryForIndex>();
            ListTourForIndex = new List<TourForIndex>();
            SliderImages = new List<string>();
            TourDetailFromIndexViewModel = new TourDetailFromIndexViewModel();
            ListNameAndImageCategoriesForNavbar = new List<NameAndImageCategoryForIndex>();
            CompanyForIndexAbout = new CompanyForIndexAbout();
            ListNewsForIndex = new List<NewsForIndex>();
            NewsForIndex = new NewsForIndex();
            ListDocuments = new List<string>();
            ListTourRecentlyViewed = new List<TourForIndex>();
        }
    }
}