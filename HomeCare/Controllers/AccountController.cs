using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.ViewModels.Account;
using HomeCare.Models; 
using HomeCare.Data;
using System.Threading.Tasks;
using HomeCare.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HomeCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserRepository userRepo, ILogger<AccountController> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login attempt failed validation for email {Email}", model.Email);
                return View(model);
            }

            try
            {
                var user = await _userRepo.GetByEmailAsync(model.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    ModelState.AddModelError("", "Ugyldig e-post eller passord");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)  // "Admin", "Caregiver", "User" etc.
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,            // remember after browser close (optional)
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)

                    }
                );

                _logger.LogInformation("User {Email} logged in successfully", model.Email);

                return RedirectToAction("Dashboard", "User");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected error during login for email {Email}", model.Email);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);

                _logger.LogWarning("Signup validation failed for email {Email}. Errors: {Errors}", model.Email, errors);

                return View(model);
            }

            try
            {
                if (await _userRepo.EmailExistsAsync(model.Email))
                {
                    _logger.LogInformation("Signup attempt with already registered email {Email}", model.Email);
                    ModelState.AddModelError("Email", "E-postadressen er allerede registrert");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Role = "user",
                    TlfNumber = model.TlfNumber,
                    Address = model.Address
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                _logger.LogInformation("New user registered with email {Email}", model.Email);

                return RedirectToAction("SignIn");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while registering new user {Email}", model.Email);
                return View("Error");
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
