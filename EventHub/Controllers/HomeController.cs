using EventHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace EventHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var adminName = _configuration.GetValue<string>("Admin:Name");

            Event ev = new Event(
                _configuration.GetValue<string>("Event:Name"),
                _configuration.GetValue<string>("Event:Description"),
                _configuration.GetValue<string>("Event:Date"),
                _configuration.GetValue<string>("Event:Country"),
                _configuration.GetValue<string>("Event:Place"),
                _configuration.GetValue<int>("Event:NeedableAge"),
                _configuration.GetValue<float>("Event:MinPrice")
            );

            // var  = _configuration.GetValue<string>("Admin:Name");

            return View(ev);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacts()
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
