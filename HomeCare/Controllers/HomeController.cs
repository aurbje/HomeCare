using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeCare.Controllers
{
    // api controller for basic public info endpoints
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // just giving the frontend some basic app info
            _logger.LogInformation("home endpoint hit");
            return Ok(new
            {
                title = "Home",
                message = "welcome to the HomeCare API"
            });
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            // simple about message to show on the frontend
            _logger.LogInformation("about endpoint hit");
            return Ok(new
            {
                title = "About Us",
                message = "this project was made as part of the ITPE3200 course"
            });
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            // returning contact info the frontend might show
            _logger.LogInformation("contact endpoint hit");
            return Ok(new
            {
                email = "support@homecare.no",
                phone = "+47 123 45 678"
            });
        }
    }
}
