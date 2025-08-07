using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace web_tour.Helpers
{
    public class RecaptchaHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string RecaptchaSecretKey = "6Ld1flcrAAAAALOmN-JSHwaPNhBniX-v7LYc7W_2";

        public static async Task<bool> VerifyTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var response = await _httpClient.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={RecaptchaSecretKey}&response={token}",
                null
            );

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RecaptchaResponse>(content);

            return result != null && result.success && result.score >= 0.5;
        }

        private class RecaptchaResponse
        {
            public bool success { get; set; }
            public double score { get; set; } = 0.9;
            public string hostname { get; set; }
            public string challenge_ts { get; set; }
        }
    }
}