using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Data;
using HomeCare.Models;


namespace HomeCare.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Login
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
             if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "E-post og passord må fylles ut");
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Ugyldig e-post eller passord");
                return View();
            }

            // TODO: Sett opp autentisering/cookies/session
            return RedirectToAction("Index", "Home");
        }
    }
}