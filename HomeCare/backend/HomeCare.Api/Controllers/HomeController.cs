using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeCare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // GET: /api/home
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Home endpoint hit.");

            return Ok(new
            {
                title = "HomeCare API",
                message = "Welcome to the HomeCare backend service.",
                version = "v1.0",
                status = "running"
            });
        }

        // GET: /api/home/about
        [HttpGet("about")]
        public IActionResult About()
        {
            _logger.LogInformation("About endpoint hit.");

            return Ok(new
            {
                title = "About HomeCare",
                message = "This project was built as part of the ITPE3200 course at OsloMet.",
                contributors = new[] { "Alexander", "Aurora", "Ulrik", "Yu" },
                repository = "https://github.com/your-org/homecare" // optional
            });
        }

        // GET: /api/home/contact
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            _logger.LogInformation("Contact endpoint hit.");

            return Ok(new
            {
                title = "Contact Support",
                email = "support@homecare.no",
                phone = "+47 123 45 678",
                hours = "Mon–Fri 09:00–17:00"
            });
        }

        // Optional: GET /api
        [HttpGet("/api")]
        public IActionResult ApiRoot()
        {
            _logger.LogInformation("API root accessed.");

            return Ok(new
            {
                message = "Welcome to the HomeCare API root. Try /api/home, /api/home/about, or /api/home/contact."
            });
        }
    }
}
