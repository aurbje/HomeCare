using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    [Authorize(Roles = "Caregiver")]
    public class CaregiverController : Controller
    {
        private readonly UserManager<Caregiver> _userManager;
        private readonly ICaregiverRepository _caregiverRepository;
        private readonly ILogger<CaregiverController> _logger;

        public CaregiverController(
            UserManager<Caregiver> userManager,
            ICaregiverRepository caregiverRepository,
            ILogger<CaregiverController> logger)
        {
            _userManager = userManager;
            _caregiverRepository = caregiverRepository;
            _logger = logger;
        }

        // ---------- DASHBOARD ----------

        public async Task<IActionResult> Dashboard()
        {
            var caregiver = await _userManager.GetUserAsync(User);
            if (caregiver == null)
            {
                _logger.LogWarning("Dashboard access without caregiver user");
                return RedirectToAction("Login", "CaregiverAccount");
            }

            var visitsToday = await _caregiverRepository.GetTodayVisitsAsync(caregiver.Id);
            var tasks = await _caregiverRepository.GetCareTasksAsync(caregiver.Id);

            // Viewen din har: @model (List<CareTask>, List<Visit>)
            var model = (tasks.ToList(), visitsToday.ToList());

            return View(model);
        }

        // ---------- HANDLINGER PÅ DASHBOARD ----------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteVisit(int id)
        {
            await _caregiverRepository.UpdateVisitStatusAsync(id, true);
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _caregiverRepository.GetCareTaskByIdAsync(id);
            if (task != null)
            {
                // forutsetter at CareTask har IsCompleted
                task.IsCompleted = true;
                await _caregiverRepository.UpdateCareTaskAsync(task);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        // valgfritt: detaljeside for et besøk
        public async Task<IActionResult> VisitDetails(int id)
        {
            var visit = await _caregiverRepository.GetVisitByIdAsync(id);
            if (visit == null)
                return NotFound();

            return View(visit); // da trenger du Views/Caregiver/VisitDetails.cshtml
        }
    }
}
