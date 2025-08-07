namespace web_tour.Models
{
    public class TourDetailViewModel
    {
        public int? TourDetailId { get; set; }
        public string? TourId { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public decimal? PriceBefore { get; set; }
        public decimal? PriceAfter { get; set; }
    }
}
