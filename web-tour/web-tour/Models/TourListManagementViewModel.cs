namespace web_tour.Models
{
    public class TourListManagementViewModel
    {
        public List<TourManagementViewModel> Tours { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
