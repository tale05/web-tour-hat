namespace web_tour.Models
{
    public class CategoryViewModel
    {
        public string? CategoryId { get; set; }
        public string? NameCategory { get; set; }
    }
    public class TourInsertViewModel
    {
        public List<CategoryViewModel>? Categories { get; set; }
        public string? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? ImgProduct { get; set; }
        public bool StatusProduct { get; set; }
    }
}
