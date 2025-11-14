using HomeCare.Constants;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeCare.Controllers
{
    //[Authorize(Roles = UserRoles.Personnel)]
    public class PersonnelController : Controller
    {
        private readonly IPersonnelRepository _repo;

        public PersonnelController(IPersonnelRepository repo)
        {
            _repo = repo;
        }

        // show dashboard
        public async Task<IActionResult> PersonnelDashboard(int? year, int? month)
        {
            var userId = GetCurrentUserId();

            int calendarYear = year ?? DateTime.Today.Year;
            int calendarMonth = month ?? DateTime.Today.Month;

            var model = await _repo.GetDashboardViewModelAsync(userId);
            ViewBag.CalendarYear = calendarYear;
            ViewBag.CalendarMonth = calendarMonth;
            ViewBag.AvailableDates = model.AvailableDates.OrderBy(d => d.Date).ToList(); // show days in order
            return View(model); // PersonnelDashboard.cshtml
        }

        // register working days
        [HttpPost]
        public async Task<IActionResult> RegisterAvailability(DateTime AvailableDate)
        {

            // validation
            if (AvailableDate.Date < DateTime.Today)
            {
                TempData["Error"] = "Du kan ikke registrere en dato i fortiden.";
                return RedirectToAction("PersonnelDashboard");
            }

            // register
            var userId = GetCurrentUserId();
            await _repo.AddAvailabilityAsync(userId, AvailableDate);
            return RedirectToAction("PersonnelDashboard");
        }


        // RegisterMultipleAvailability
        [HttpPost]
        public async Task<IActionResult> RegisterMultipleAvailability(List<DateTime> SelectedDates)
        {
            var userId = GetCurrentUserId();
            foreach (var date in SelectedDates)
            {
                if (date.Date >= DateTime.Today)
                {
                    await _repo.AddAvailabilityAsync(userId, date);
                }
            }
            return RedirectToAction("PersonnelDashboard");
        }


        // get userID
        private int GetCurrentUserId()
        {
            return 1; // temporary ID
            // var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            // if (claim == null)
            // {
            //     throw new Exception("User is not authenticated.");
            // }
            // return int.Parse(claim.Value);
        }

        // delete working days
        [HttpPost]
        public async Task<IActionResult> DeleteAvailability(DateTime dateToDelete)
        {
            var userId = GetCurrentUserId();
            await _repo.DeleteAvailabilityAsync(userId, dateToDelete.Date);
            return RedirectToAction("PersonnelDashboard");
        }

    }
}
