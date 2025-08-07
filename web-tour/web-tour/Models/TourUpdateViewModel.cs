using System.ComponentModel.DataAnnotations;
using web_tour.Entities;

namespace web_tour.Models
{
    public class TourUpdateViewModel
    {
        [Required]
        public string? TourId { get; set; }

        [Required(ErrorMessage = "Tiêu đề tour là bắt buộc.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public string? CategoryId { get; set; }

        public bool StatusProduct { get; set; }

        public string? ImgTitle { get; set; }

        public List<Category>? Categories { get; set; }
    }
}
