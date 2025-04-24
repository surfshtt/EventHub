using EventHub.Models;
using EventHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            return View(events);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public async Task<IActionResult> Admin()
        {
            var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "admin");
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = "admin",
                    Password = "1234", // В реальном приложении пароль должен быть захэширован
                    Role = "admin",
                    Email = "admin@eventhub.com"
                };
            }
            return View(adminUser);
        }

        public async Task<IActionResult> About(int eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);

            var relatedEvents = await _context.Events
                .Where(e => e.Id != eventId)
                .Take(3)
                .ToListAsync();

            var aboutEvent = new AboutEvent
            {
                Event = eventItem,
                RelatedEvents = relatedEvents
            };

            return View(aboutEvent);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
