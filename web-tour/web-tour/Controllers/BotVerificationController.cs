using Microsoft.AspNetCore.Mvc;

namespace web_tour.Controllers
{
    public class BotVerificationController : Controller
    {
        [HttpGet("verify-bot")]
        public IActionResult VerifyBot()
        {
            return View();
        }

        [HttpPost("verify-bot")]
        public async Task<IActionResult> VerifyBot(string gRecaptchaResponse)
        {
            var secret = "6Ld1flcrAAAAALOmN-JSHwaPNhBniX-v7LYc7W_2";

            using var client = new HttpClient();

            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}", null);

            var json = await response.Content.ReadAsStringAsync();

            dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            if (jsonData.success == true && jsonData.score >= 0.5)
            {
                HttpContext.Session.SetString("BotVerified", "true");

                var returnUrl = HttpContext.Session.GetString("ReturnUrlAfterVerify");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    HttpContext.Session.Remove("ReturnUrlAfterVerify");
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index");
            }

            ViewBag.Error = "Xác minh thất bại hoặc điểm thấp. Vui lòng thử lại.";
            return View();
        }
    }
}
