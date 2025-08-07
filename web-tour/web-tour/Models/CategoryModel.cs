using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace web_tour.Models
{
    public class CategoryModel
    {
        [Display(Name = "Mã danh mục")]
        public string? CategoryId { get; set; }

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(255, ErrorMessage = "Tên danh mục không được vượt quá 255 ký tự.")]
        public string? NameCategory { get; set; }

        [Display(Name = "Hình ảnh danh mục")]
        public string? ImgCategory { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? StatusCategory { get; set; }
    }
}