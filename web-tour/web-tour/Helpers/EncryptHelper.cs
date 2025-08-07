using System;
using System.Text;
using System.Security.Cryptography;

namespace web_tour.Helpers
{
    public static class EncryptHelper
    {
        private static readonly string key = "your-secret-key"; // lưu ở appsettings.json

        public static string EncodeId(int id)
        {
            var plainText = (id + key).ToString();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainBytes)
                          .Replace("=", "") // xóa dấu "=" cho gọn
                          .Replace("+", "-") // tránh ký tự URL không hợp lệ
                          .Replace("/", "_");
        }

        public static int DecodeId(string encoded)
        {
            encoded = encoded.Replace("-", "+").Replace("_", "/");

            // Thêm lại dấu "=" nếu thiếu (base64 yêu cầu độ dài chia hết cho 4)
            while (encoded.Length % 4 != 0)
                encoded += "=";

            var base64Bytes = Convert.FromBase64String(encoded);
            var result = Encoding.UTF8.GetString(base64Bytes);
            if (result.EndsWith(key))
            {
                var idPart = result.Replace(key, "");
                return int.TryParse(idPart, out int id) ? id : 0;
            }
            return 0;
        }
    }
}
