using HomeCare.Models;
using HomeCare.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    public class CaregiverAccountController : Controller
    {
        private readonly SignInManager<Caregiver> _signInManager;
        private readonly UserManager<Caregiver> _userManager;
        private readonly ILogger<CaregiverAccountController> _logger;

        public CaregiverAccountController(
            SignInManager<Caregiver> signInManager,
            UserManager<Caregiver> userManager,
            ILogger<CaregiverAccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // login already existing caregiver

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                userName: model.Email,            // email as username
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Invalid caregiver login attempt for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Ugyldig e-post eller passord.");
                return View(model);
            }

            _logger.LogInformation("Caregiver {Email} logged in", model.Email);
            return RedirectToAction("Dashboard", "Caregiver");
        }

        // register new caregiver (admin only)

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var caregiver = new Caregiver
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(caregiver, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            // assign Caregiver role
            await _userManager.AddToRoleAsync(caregiver, "Caregiver");

            _logger.LogInformation("New caregiver {Email} registered", model.Email);

            return RedirectToAction(nameof(Login));
        }

        // logout caregiver

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Caregiver logged out");
            return RedirectToAction(nameof(Login));
        }

        // denied access view

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
