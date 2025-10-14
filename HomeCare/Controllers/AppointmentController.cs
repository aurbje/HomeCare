using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using HomeCare.Models;


public class AppointmentController : Controller
{
    private static List<Appointment> _appointments = new();

    public IActionResult Book() => View();

    [HttpPost]
    public IActionResult Book(Appointment appt)
    {
        appt.Id = _appointments.Count + 1;
        _appointments.Add(appt);
        return RedirectToAction("Confirm");
    }

    public IActionResult Confirm() => Content("Appointment booked!");

    public IActionResult ViewAll() => Json(_appointments);

    [HttpPost]
    public IActionResult Cancel(int id)
    {
        var appt = _appointments.FirstOrDefault(a => a.Id == id);
        if (appt != null) _appointments.Remove(appt);
        return Json(new { message = "Canceled. Want to book a new one?" });
    }

    public IActionResult Manage(int id) => View(_appointments.FirstOrDefault(a => a.Id == id));
}