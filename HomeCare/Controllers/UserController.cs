using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.ViewModels.Account; // der SignInViewModel og SignUpViewModel ligger
using HomeCare.Models; 
using HomeCare.Data;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
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
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Ugyldig e-post eller passord");
                return View(model);
            }

            // if login success, sent to homepage, can change this later
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
            Console.WriteLine("SignUp POST reached");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("Validation errors: " + string.Join(", ", errors));
                return View(model);
            }

            // Debug: logg verdiene fra modellen
            Console.WriteLine($"FullName: {model.FullName}, Email: {model.Email}, Tlf: {model.TlfNumber}, Address: {model.Address}");

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
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

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("SignIn");
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
