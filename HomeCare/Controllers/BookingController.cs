using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.Data;
using HomeCare.ViewModels;
using System.Threading.Tasks;
using HomeCare.Repositories.Interfaces;

namespace HomeCare.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepo;

        public BookingController(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        // GET: Booking page with available dates and categories
        public async Task<IActionResult> Booking()
        {
            var availableDates = await _bookingRepo.GetAvailableDatesAsync();
            var categories = await _bookingRepo.GetCategoriesAsync();

            var model = new BookingViewModel
            {
                SelectedDate = DateTime.Today,
                TimeSlotId = 0,
                CategoryId = categories.FirstOrDefault()?.Id ?? 0,
                AvailableDates = availableDates.ToList(),
                Categories = categories.ToList()
            };

            ViewBag.Appointments = await _bookingRepo.GetUpcomingAppointmentsAsync();
                

            return View(model); // Views/Booking/Booking.cshtml
        }

        // POST: Handle booking submission
        [HttpPost]
        public async Task<IActionResult> Booking(BookingViewModel model)
        {
            var selectedCategory = await _bookingRepo.GetCategoryByIdAsync(model.CategoryId);

            // Require notes if category is "OTHER"
            if (selectedCategory?.Name.ToUpper() == "OTHER" && string.IsNullOrWhiteSpace(model.Notes))
            {
                ModelState.AddModelError("Notes", "Please provide details for 'Other' category.");
            }

            // Retrieve selected time slot and ensure it's not already booked
            var selectedSlot = await _bookingRepo.GetAvailableTimeSlotAsync(model.TimeSlotId);

            if (selectedSlot == null)
            {
                ModelState.AddModelError("TimeSlotId", "Selected time slot is no longer available.");
            }

            // Attempt to parse start time from slot string
            TimeSpan startTime = default;
            bool validTime = false;

            if (selectedSlot != null)
            {
                var slotParts = selectedSlot.Slot.Replace("–", "-").Split('-');
                if (slotParts.Length == 2 && TimeSpan.TryParse(slotParts[0], out startTime))
                {
                    validTime = true;
                }
                else
                {
                    ModelState.AddModelError("TimeSlotId", "Invalid time slot format.");
                }
            }

            // Save or update appointment if all validations pass
            if (ModelState.IsValid && validTime)
            {
                if (model.AppointmentId > 0)
                {
                    // Editing existing appointment
                    var existing = await _bookingRepo.GetAppointmentByIdAsync(model.AppointmentId);
                    if (existing != null)
                    {
                        // Update appointment details
                        existing.DateTime = selectedSlot!.AvailableDate.Date.Add(startTime);
                        existing.TimeSlotId = selectedSlot.Id;
                        existing.CategoryId = selectedCategory!.Id;
                        existing.Notes = model.Notes;

                        selectedSlot.IsBooked = true;
                        await _bookingRepo.UpdateTimeSlotAsync(selectedSlot);

                        await _bookingRepo.UpdateAppointmentAsync(existing);
                        TempData["BookingSuccess"] = $"Appointment has changed to {existing.DateTime:yyyy-MM-dd} {selectedSlot.Slot}!";
                        return RedirectToAction("Booking");
                    }
                }
                else
                {
                    // Creating new appointment
                    var appointment = new Appointment
                    {
                        DateTime = selectedSlot.AvailableDate.Date.Add(startTime),
                        TimeSlotId = selectedSlot.Id,
                        CategoryId = selectedCategory!.Id,
                        Notes = model.Notes
                    };

                    selectedSlot.IsBooked = true;

                    await _bookingRepo.AddAppointmentAsync(appointment);
                    TempData["BookingSuccess"] = $"Appointment booked for {model.SelectedDate:yyyy-MM-dd} {selectedSlot.Slot}!";
                    return RedirectToAction("Booking");
                }
            }

            // Reload view model if validation fails
             model.AvailableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList();
            model.Categories = (await _bookingRepo.GetCategoriesAsync()).ToList();
            ViewBag.Appointments = await _bookingRepo.GetUpcomingAppointmentsAsync();

            return View(model);
        }

        // Cancel appointment
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                var slot = await _bookingRepo.GetAvailableTimeSlotAsync(appointment.TimeSlotId);
                if (slot != null)
                {
                    slot.IsBooked = false;
                    await _bookingRepo.UpdateTimeSlotAsync(slot);
                }

                await _bookingRepo.DeleteAppointmentAsync(id);
                TempData["BookingSuccess"] = "Appointment has been cancelled.";
            }
            return RedirectToAction("Booking");
        }
        // Edit appointment
        public async Task<IActionResult> EditAppointment(int id)
        {
            var appointment = await _bookingRepo.GetAppointmentByIdAsync(id);

            if (appointment == null) return NotFound();

            var model = new BookingViewModel
            {
                SelectedDate = appointment.DateTime.Date,
                TimeSlotId = appointment.TimeSlotId,
                CategoryId = appointment.CategoryId,
                Notes = appointment.Notes,
                AvailableDates = (await _bookingRepo.GetAvailableDatesAsync()).ToList(),
                Categories = (await _bookingRepo.GetCategoriesAsync()).ToList(),
                AppointmentId = appointment.Id // Add this to your ViewModel
            };

            // List of booked appointments
            ViewBag.Appointments = await _bookingRepo.GetUpcomingAppointmentsAsync();
            ViewBag.EditingId = appointment.Id;

            return View("Booking", model);
        }


        // Convert HH:mm string to hour as double (e.g. "14:30" => 14.0)
        private double TimeToHour(string time)
        {
            return double.Parse(time.Split(':')[0]);
        }
    }

}
