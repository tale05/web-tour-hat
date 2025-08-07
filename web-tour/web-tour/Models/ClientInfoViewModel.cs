namespace web_tour.Models
{
    public class ClientInfoViewModel
    {
        public string IP { get; set; }
        public string Browser { get; set; }
        public string OS { get; set; }
        public string UserAgent { get; set; }
        public string ScreenResolution { get; set; } // Không dùng được từ server, nên có thể để trống hoặc bỏ
        public string DateTimeNow { get; set; }
    }
}