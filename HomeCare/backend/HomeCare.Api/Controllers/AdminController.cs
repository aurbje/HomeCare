using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Data;
using Microsoft.EntityFrameworkCore;
using HomeCare.Models;
using HomeCare.ViewModels.Admin;
using System.Linq;

namespace HomeCare.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View("Admin"); // Admin.cshtml
        }
        
    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Users(string? q)
    {
        IQueryable<User> query = _context.Users;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FullName.ToLower().Contains(term) ||
                u.UserName.ToLower().Contains(term));
        }

        var users = await query
            .OrderBy(u => u.Id)
            .Take(500) // safety cap
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("_Users", users);
    }
    
    [HttpGet]
public async Task<IActionResult> EditUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        TempData["Error"] = "Bruker ikke funnet.";
        return RedirectToAction("Users");
    }
    
    return View("EditUser", user);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditUser(int id, User model)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        TempData["Error"] = "Bruker ikke funnet.";
        return RedirectToAction("Users");
    }

    // Edit user profile
    user.FullName = model.FullName;
    user.Email = model.Email;
    user.TlfNumber = model.TlfNumber;
    user.Address = model.Address;
    user.Role = model.Role;

    await _context.SaveChangesAsync();
    TempData["Success"] = $"Bruker {user.FullName} ble oppdatert.";
    return RedirectToAction("Users");
}
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        TempData["Error"] = "Bruker ikke funnet.";
        return RedirectToAction("Users");
    }

    // safetyguard - Do not delete the last admin
    if (user.Role == "admin" && await _context.Users.CountAsync(u => u.Role == "admin") <= 1)
    {
        TempData["Error"] = "Kan ikke slette den siste admin-brukeren.";
        return RedirectToAction("Users");
    }

    // check references
    var hasClientBookings = await _context.Bookings.AnyAsync(b => b.ClientId == id);
    var hasPersonnelBookings = await _context.Bookings.AnyAsync(b => b.PersonnelId == id);
    var hasAppointments = await _context.PersonnelAppointments.AnyAsync(pa => pa.ClientId == id || pa.PersonnelId == id);

    if (hasClientBookings || hasPersonnelBookings || hasAppointments)
    {
        TempData["Error"] = "Kan ikke slette bruker som er knyttet til bookinger/avtaler.";
        return RedirectToAction("Users");
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    TempData["Success"] = $"Bruker {user.FullName} ble slettet.";
    return RedirectToAction("Users");
    }
        [HttpGet]
    public async Task<IActionResult> Personnel(string? q)
    {
        IQueryable<User> query = _context.Users.Where(u => u.Role == "Ansatt");

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FullName.ToLower().Contains(term) ||
                u.UserName.ToLower().Contains(term) ||
                (u.PersonnelId.HasValue && u.PersonnelId.Value.ToString().Contains(term)));
        }

            var users = await query
            .OrderBy(u => u.Id)
            .Take(500)
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("_Personnel", users);
    }

    [HttpGet]
    public async Task<IActionResult> EditPersonnel(int id)
    {
        // Add debugging
        Console.WriteLine($"EditPersonnel GET called with id: {id}");
        
        var user = await _context.Users.FindAsync(id);
        
        // More detailed debugging
        Console.WriteLine($"User found: {user != null}");
        if (user != null)
        {
            Console.WriteLine($"User role: {user.Role}");
        }
        
        if (user == null || user.Role != "Ansatt")
        {
            TempData["Error"] = "Personell ikke funnet.";
            return RedirectToAction("Personnel");
        }

        return View("EditPersonnel", user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPersonnel(int id, User model)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.Role != "Ansatt")
        {
            TempData["Error"] = "Personell ikke funnet.";
            return RedirectToAction("Personnel");
        }

        // Update personnel details
        user.FullName = model.FullName;
        user.Email = model.Email;
        user.TlfNumber = model.TlfNumber;
        user.Address = model.Address;

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Personell {user.FullName} ble oppdatert.";
        return RedirectToAction("Personnel");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePersonnel(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Personell ikke funnet.";
            return RedirectToAction("Personnel");
        }

        // Checks references
        var hasPersonnelBookings = await _context.Bookings.AnyAsync(b => b.PersonnelId == id);
        var hasAppointments = await _context.PersonnelAppointments.AnyAsync(pa => pa.PersonnelId == id);

        if (hasPersonnelBookings || hasAppointments)
        {
            TempData["Error"] = "Kan ikke slette personell som er knyttet til bookinger/avtaler.";
            return RedirectToAction("Personnel");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Personell {user.FullName} ble slettet.";
        return RedirectToAction("Personnel");
    }

        [HttpGet]
    public async Task<IActionResult> Bookings(string? q)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Personnel);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(b =>
                (b.Client != null && (
                    b.Client.FullName.ToLower().Contains(term) ||
                    b.Client.Email.ToLower().Contains(term))) ||
                (b.Personnel != null && (
                    b.Personnel.FullName.ToLower().Contains(term) ||
                    b.Personnel.Email.ToLower().Contains(term))) ||
                (b.ServiceType != null && b.ServiceType.ToLower().Contains(term)));
        }

        var bookings = await query
            .OrderByDescending(b => b.Date)
            .ThenBy(b => b.Time)
            .Take(500)
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("_Bookings", bookings);
    }

    [HttpGet]
    public async Task<IActionResult> EditBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Client)
            .Include(b => b.Personnel)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            TempData["Error"] = "Booking ikke funnet.";
            return RedirectToAction("Bookings");
        }

        // Get all users and personnel for dropdowns
        ViewBag.Clients = await _context.Users.Where(u => u.Role == "User").ToListAsync();
        ViewBag.Personnel = await _context.Users.Where(u => u.Role == "Ansatt").ToListAsync();

        return View("EditBookings", booking);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBooking(int id, Booking model)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            TempData["Error"] = "Booking ikke funnet.";
            return RedirectToAction("Bookings");
        }

        // Update booking details
        booking.Date = model.Date;
        booking.Time = model.Time;
        booking.ServiceType = model.ServiceType;
        booking.Notes = model.Notes;
        booking.PersonnelId = model.PersonnelId;

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Booking {booking.Id} ble oppdatert.";
        return RedirectToAction("Bookings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            TempData["Error"] = "Booking ikke funnet.";
            return RedirectToAction("Bookings");
        }

        // Optional safety: block deletion of past bookings
        if (booking.Date.Date < DateTime.Today)
        {
            TempData["Error"] = "Kan ikke slette tidligere booking.";
            return RedirectToAction("Bookings");
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Booking {booking.Id} ble slettet.";
        return RedirectToAction("Bookings");
    }
}
}