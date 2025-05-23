using EventHub.Models;
using EventHub.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        public async Task<IActionResult> Index(string eventType = null, string searchQuery = null)
        {
            var events = _context.Events.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                events = events.Where(e => e.Name.Contains(searchQuery));
            }
            
            ViewBag.EventTypes = new List<string> { "Концерт", "Театр", "Спорт", "Выставка", "Фестиваль", "Семейное" };
            ViewBag.SelectedEventType = eventType;
            ViewBag.SearchQuery = searchQuery;
            
            return View(await events.ToListAsync());
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
                .OrderByDescending(e => e.Date)
                .ThenBy(e => e.Name)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout([FromBody] List<CartItem> cartItems)
        {
            _logger.LogInformation("Starting checkout process");
            
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Checkout failed: User not authenticated");
                return Json(new { success = false, message = "Необходимо войти в систему" });
            }

            if (cartItems == null || !cartItems.Any())
            {
                _logger.LogWarning("Checkout failed: Empty cart");
                return Json(new { success = false, message = "Корзина пуста" });
            }

            try
            {
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                if (emailClaim == null)
                {
                    _logger.LogWarning("Checkout failed: Email claim not found");
                    return Json(new { success = false, message = "Пользователь не найден" });
                }

                _logger.LogInformation($"Processing checkout for user email: {emailClaim.Value}");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
                if (user == null)
                {
                    _logger.LogWarning($"Checkout failed: User not found for email {emailClaim.Value}");
                    return Json(new { success = false, message = "Пользователь не найден" });
                }

                foreach (var item in cartItems)
                {
                    _logger.LogInformation($"Processing cart item: EventId={item.EventId}, Quantity={item.Quantity}, Type={item.TicketType}");

                    // Проверка существования события
                    var eventItem = await _context.Events.FindAsync(item.EventId);
                    if (eventItem == null)
                    {
                        _logger.LogWarning($"Checkout failed: Event not found with ID {item.EventId}");
                        return Json(new { success = false, message = $"Событие с ID {item.EventId} не найдено" });
                    }

                    // Проверка количества билетов
                    if (item.Quantity <= 0 || item.Quantity > 5)
                    {
                        _logger.LogWarning($"Checkout failed: Invalid quantity {item.Quantity}");
                        return Json(new { success = false, message = "Количество билетов должно быть от 1 до 5" });
                    }

                    // Проверка типа билета
                    if (string.IsNullOrEmpty(item.TicketType))
                    {
                        _logger.LogWarning("Checkout failed: Ticket type not specified");
                        return Json(new { success = false, message = "Не указан тип билета" });
                    }

                    // Проверка цены
                    decimal expectedPrice = item.TicketType switch
                    {
                        "vip" => eventItem.Price * 2,
                        "discount" => eventItem.Price / 2,
                        _ => eventItem.Price
                    };

                    if (item.Price != expectedPrice)
                    {
                        _logger.LogWarning($"Checkout failed: Price mismatch. Expected: {expectedPrice}, Got: {item.Price}");
                        return Json(new { success = false, message = "Неверная цена билета" });
                    }

                    // Проверка на максимальное количество билетов
                    var existingTickets = await _context.UserTickets
                        .Where(t => t.UserId == user.Id && t.EventId == item.EventId)
                        .SumAsync(t => t.Quantity);

                    if (existingTickets + item.Quantity > 5)
                    {
                        _logger.LogWarning($"Checkout failed: Too many tickets. Existing: {existingTickets}, Requested: {item.Quantity}");
                        return Json(new { success = false, message = "Превышено максимальное количество билетов на одно событие (5)" });
                    }

                    var userTicket = new UserTicket
                    {
                        UserId = user.Id,
                        EventId = item.EventId,
                        PurchaseDate = DateTime.Now,
                        Status = "Active",
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    };

                    _logger.LogInformation($"Adding ticket to database: UserId={user.Id}, EventId={item.EventId}, Quantity={item.Quantity}");
                    _context.UserTickets.Add(userTicket);
                }

                _logger.LogInformation("Saving changes to database");
                await _context.SaveChangesAsync();
                _logger.LogInformation("Checkout completed successfully");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ticket purchase. Details: {Message}", ex.Message);
                return Json(new { success = false, message = "Произошла ошибка при покупке билетов. Пожалуйста, попробуйте позже." });
            }
        }
    }
}
