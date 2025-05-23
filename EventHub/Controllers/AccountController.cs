using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using EventHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EventHub.Data;
using Microsoft.Extensions.Logging;

namespace EventHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль!");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            _logger.LogInformation("Attempting to register user: {Email}", user.Email);

            try
            {
                if (ModelState.IsValid)
                {
                    // Check if email already exists
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Registration failed: Email {Email} already exists", user.Email);
                        ModelState.AddModelError("Email", "Этот email уже зарегистрирован");
                        return View(user);
                    }

                    // Check if username already exists
                    existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Registration failed: Username {Username} already exists", user.UserName);
                        ModelState.AddModelError("UserName", "Это имя пользователя уже занято");
                        return View(user);
                    }

                    // Validate password length
                    if (string.IsNullOrEmpty(user.Password) || user.Password.Length < 6)
                    {
                        _logger.LogWarning("Registration failed: Password too short for user {Email}", user.Email);
                        ModelState.AddModelError("Password", "Пароль должен содержать минимум 6 символов");
                        return View(user);
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User {Email} registered successfully", user.Email);

                    // Automatically log in the user after registration
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties();

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {Email} logged in after registration", user.Email);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Registration failed: ModelState is invalid for user {Email}", user.Email);
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", user.Email);
                ModelState.AddModelError("", "Произошла ошибка при регистрации. Пожалуйста, попробуйте позже.");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

            if (user == null)
            {
                return NotFound();
            }

            var userTickets = await _context.UserTickets
                .Include(ut => ut.Event)
                .Where(ut => ut.UserId == user.Id)
                .OrderByDescending(ut => ut.PurchaseDate)
                .ToListAsync();

            ViewBag.UserTickets = userTickets;

            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
} 