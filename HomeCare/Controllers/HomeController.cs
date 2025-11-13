using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View("About");
    }

    public IActionResult Contact()
    {
        return View();
    }
}
