using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCModel.Data;
using MVCModel.Models;

namespace MVCModel.Controllers
{
    public class AccountController : Controller
    {
        private readonly MVCModelDbContext _db;

        public AccountController(MVCModelDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                // Add the user to the database
                _db.Users.Add(model);
                await _db.SaveChangesAsync();

                // Sign in the user
                await SignInUser(model);

                TempData["success"] = "User registered successfully";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && user.Password == model.Password)
                {
                    // Sign in the user
                    await SignInUser(user);

                    return RedirectToAction("Dashboard", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }


        private async Task SignInUser(User user)
        {
            // Create a claims identity for the authenticated user
            var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, "user")
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user with a cookie authentication scheme
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("CookieAuthentication");

            return RedirectToAction("Index", "Home");
        }
    }
    }