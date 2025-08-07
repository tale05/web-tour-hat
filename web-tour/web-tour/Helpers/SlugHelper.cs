using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace web_tour.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string title)
        {
            string normalized = title.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            string slug = sb.ToString().Normalize(NormalizationForm.FormC)
                .ToLowerInvariant()
                .Replace("đ", "d");

            // Loại bỏ ký tự không hợp lệ và thay bằng dấu gạch ngang
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }
    }
}
