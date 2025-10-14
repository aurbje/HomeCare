using Microsoft.AspNetCore.Mvc;
using HomeCare.ViewModels.Account; // der SignInViewModel og SignUpViewModel ligger
using HomeCare.Models; 

namespace HomeCare.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Her legger du inn logikk for å sjekke bruker
                // F.eks. sjekk i database (User.cs-modell)
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Her legger du inn logikk for å opprette ny bruker
                // F.eks. lagre til database via User.cs
                return RedirectToAction("SignIn");
            }

            return View(model);
        }
    }
}
