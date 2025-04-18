using EventHub.Models;
using Microsoft.AspNetCore.Components;
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
                _configuration.GetValue<int>("Event:Id"),
                _configuration.GetValue<string>("Event:Name") ?? "Default Name",
                _configuration.GetValue<string>("Event:Description") ?? "Default Description",
                _configuration.GetValue<string>("Event:Date") ?? DateTime.Now.ToString("yyyy-MM-dd"),
                _configuration.GetValue<string>("Event:Country") ?? "Default Country",
                _configuration.GetValue<string>("Event:Place") ?? "Default Place",
                _configuration.GetValue<int>("Event:NeedableAge"),
                _configuration.GetSection("Event:MinPrice").Get<float[]>() ?? new float[] { 0.0f }
            );
            Event[] ar = new Event[] { ev, ev };

            return View(ar);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Admin()
        {
            User us = new User("fk8ght", "1234", "admin", "fk8ghtt@gmail.com");
            return View(us);
        }

        public IActionResult About(int eventId)
        {
            //Логика поиска в бд

            if (eventId == 1)
            {
                Event ev = new Event(
                   _configuration.GetValue<int>("Event:Id"),
                   _configuration.GetValue<string>("Event:Name"),
                   _configuration.GetValue<string>("Event:Description"),
                   _configuration.GetValue<string>("Event:Date"),
                   _configuration.GetValue<string>("Event:Country"),
                   _configuration.GetValue<string>("Event:Place"),
                   _configuration.GetValue<int>("Event:NeedableAge"),
                   _configuration.GetSection("Event:MinPrice").Get<float[]>() ?? new float[] { 0.0f }
                );

                AboutEvent ac = new AboutEvent(ev, [ev, ev, ev]);

                return View(ac);
            }
            else
            {
                AboutEvent ab = new AboutEvent(null, [new Event(1, "1", "1", "1", "1", "1", 1, [1, 1])]); ;
                return View(ab);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
