namespace web_tour.Models
{
    public class NewsModel
    {
        public int? IdNews { get; set; }
        public string? IdCompany { get; set; }
        public string? Title { get; set; }
        public string? ImgNews { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
