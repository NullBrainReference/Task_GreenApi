using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Task_GreenApi.Models;

namespace Task_GreenApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private HomeViewModel model;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            //var model = new HomeViewModel();
            _httpClient = httpClientFactory.CreateClient("GreenClient");
            Console.WriteLine(_httpClient.BaseAddress);
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings(HomeViewModel model)
        {
            var resp = await _httpClient.GetAsync($"/waInstance{model.IdInstance}/getSettings/{model.ApiToken}");

            model.Log += resp.StatusCode + "\n";
            model.Log += await resp.Content.ReadAsStringAsync() + "\n";
            this.model = model;

            return RedirectToAction("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStateInstance(HomeViewModel model)
        {
            var resp = await _httpClient.GetAsync($"/waInstance{model.IdInstance}/getStateInstance/{model.ApiToken}");

            Console.WriteLine(resp);

            model.Log += resp.StatusCode + "\n";
            model.Log += await resp.Content.ReadAsStringAsync() + "\n";
            this.model = model;

            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(HomeViewModel model)
        {
            var content = new StringContent($"{{\r\n\t\"chatId\": \"{model.Number}@c.us\",\r\n\t\"message\": \"{model.Message}\"\r\n}}");

            var resp = await _httpClient.PostAsync($"/waInstance{model.IdInstance}/sendMessage/{model.ApiToken}/", content);
            
            Console.WriteLine(content);
            
            model.Log += resp.StatusCode + "\n";
            model.Log += await resp.Content.ReadAsStringAsync() + "\n";
            this.model = model;

            return RedirectToAction("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> SendFile(HomeViewModel model)
        {
            string fileName = Path.GetFileName(model.FileUrl);

            var content = new StringContent(
                $"{{\r\n\t\"chatId\": \"{model.Number}@c.us\"," +
                $"\r\n\t\"urlFile\": \"{model.FileUrl}\"," +
                $"\r\n\t\"fileName\": \"{fileName}\"\r\n}}");

            var resp = await _httpClient.PostAsync($"/waInstance{model.IdInstance}/sendFileByUrl/{model.ApiToken}/", content);

            Console.WriteLine(content);

            model.Log += resp.StatusCode + "\n";
            model.Log += await resp.Content.ReadAsStringAsync() + "\n";
            this.model = model;

            return RedirectToAction("Index", model);
        }

        public IActionResult Index(HomeViewModel model)
        {
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}