// using Microsoft.AspNetCore.Mvc;
// using HomeCare.Models;
// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace HomeCare.Controllers
// {
//     public class AppointmentController : Controller
//     {
//         // Midlertidig in-memory-liste (erstattes med database senere)
//         private static readonly List<Appointment> _appointments = new();

//         // === BOOKING-SKJEMA ===
//         [HttpGet]
//         public IActionResult Book()
//         {
//             return View(); // Viser Book.cshtml
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public IActionResult Book(Appointment appt)
//         {
//             if (ModelState.IsValid)
//             {
//                 appt.Id = _appointments.Count + 1;
//                 appt.DateTime = appt.DateTime == default ? DateTime.Now.AddDays(1) : appt.DateTime;

//                 _appointments.Add(appt);

//                 // TempData["SuccessMessage"] = $"Timen for {appt.ServiceType} er registrert!";
//                 return RedirectToAction("Confirm", new { id = appt.Id });
//             }

//             // Hvis validering feiler
//             TempData["ErrorMessage"] = "Vennligst fyll ut alle nødvendige felter.";
//             return View(appt);
//         }

//         // === BEKREFTELSE ===
//         [HttpGet]
//         public IActionResult Confirm(int id)
//         {
//             var appt = _appointments.FirstOrDefault(a => a.Id == id);
//             if (appt == null)
//                 return NotFound("Avtalen finnes ikke.");

//             return View(appt);
//         }

//         // === LISTE OVER ALLE BOOKINGER ===
//         [HttpGet]
//         public IActionResult ViewAll()
//         {
//             return View(_appointments);
//         }

//         // === AVLYS AVTALE ===
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public IActionResult Cancel(int id)
//         {
//             var appt = _appointments.FirstOrDefault(a => a.Id == id);
//             if (appt == null)
//                 return Json(new { success = false, message = "Avtalen ble ikke funnet." });

//             _appointments.Remove(appt);
//             return Json(new { success = true, message = "Avtalen ble avlyst. Ønsker du å bestille en ny?" });
//         }

//         // === ADMIN / MANAGE ENKELTAVTALE ===
//         [HttpGet]
//         public IActionResult Manage(int id)
//         {
//             var appt = _appointments.FirstOrDefault(a => a.Id == id);
//             if (appt == null)
//                 return NotFound();

//             return View(appt);
//         }
//     }
// }
