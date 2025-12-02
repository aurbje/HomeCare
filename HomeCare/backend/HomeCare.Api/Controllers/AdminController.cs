using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeCare.Api.Data;
using Microsoft.EntityFrameworkCore;
using HomeCare.Api.Models;
using HomeCare.Api.DTO;
using System.Linq;

namespace HomeCare.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View("Admin"); 
        }
        
    private readonly AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Users(string? q)
    {
        IQueryable<User> query = _context.AppUsers;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FullName.ToLower().Contains(term));
        }

        var users = await query
            .OrderBy(u => u.Id)
            .Take(500) // safety cap
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("Users", users);
    }
    
    [HttpGet]
public async Task<IActionResult> EditUser(int id)
{
    var user = await _context.AppUsers.FindAsync(id);
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
    var user = await _context.AppUsers.FindAsync(id);
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
    var user = await _context.AppUsers.FindAsync(id);
    if (user == null)
    {
        TempData["Error"] = "Bruker ikke funnet.";
        return RedirectToAction("Users");
    }

    // safetyguard - Do not delete the last admin
    if (user.Role == "Admin" && await _context.AppUsers.CountAsync(u => u.Role == "Admin") <= 1)
    {
        TempData["Error"] = "Kan ikke slette den siste admin-brukeren.";
        return RedirectToAction("Users");
    }

    // check references
    var hasBookings = await _context.Bookings.AnyAsync(b => b.UserId == id);

    if (hasBookings)
    {
        TempData["Error"] = "Kan ikke slette bruker som er knyttet til bookinger.";
        return RedirectToAction("Users");
    }

    _context.AppUsers.Remove(user);
    await _context.SaveChangesAsync();

    TempData["Success"] = $"Bruker {user.FullName} ble slettet.";
    return RedirectToAction("Users");
    }
        [HttpGet]
    public async Task<IActionResult> Personnel(string? q)
    {
        IQueryable<User> query = _context.AppUsers.Where(u => u.Role == "Caregiver");

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                u.FullName.ToLower().Contains(term));
        }

            var users = await query
            .OrderBy(u => u.Id)
            .Take(500)
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("Personnel", users);
    }

    [HttpGet]
    public async Task<IActionResult> EditPersonnel(int id)
    {
        // Add debugging
        Console.WriteLine($"EditPersonnel GET called with id: {id}");
        
        var user = await _context.AppUsers.FindAsync(id);
        
        // More detailed debugging
        Console.WriteLine($"User found: {user != null}");
        if (user != null)
        {
            Console.WriteLine($"User role: {user.Role}");
        }
        
        if (user == null || user.Role != "Caregiver")
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
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null || user.Role != "Caregiver")
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
        var user = await _context.AppUsers.FindAsync(id);
        if (user == null)
        {
            TempData["Error"] = "Personell ikke funnet.";
            return RedirectToAction("Personnel");
        }

        // Checks references - CaregiverId is a string containing the user ID
        var hasBookings = await _context.Bookings.AnyAsync(b => b.CaregiverId == id.ToString());

        if (hasBookings)
        {
            TempData["Error"] = "Kan ikke slette personell som er knyttet til bookinger.";
            return RedirectToAction("Personnel");
        }

        _context.AppUsers.Remove(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Personell {user.FullName} ble slettet.";
        return RedirectToAction("Personnel");
    }

        [HttpGet]
    public async Task<IActionResult> Bookings(string? q)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.User);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(b =>
                (b.User != null && (
                    b.User.FullName.ToLower().Contains(term) ||
                    b.User.Email.ToLower().Contains(term))) ||
                (b.ServiceType != null && b.ServiceType.ToLower().Contains(term)) ||
                (b.CaregiverId != null && b.CaregiverId.Contains(term)));
        }

        var bookings = await query
            .OrderByDescending(b => b.Date)
            .ThenBy(b => b.Time)
            .Take(500)
            .ToListAsync();

        ViewBag.SearchTerm = q ?? string.Empty;
        return View("Bookings", bookings);
    }

    [HttpGet]
    public async Task<IActionResult> EditBooking(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            TempData["Error"] = "Booking ikke funnet.";
            return RedirectToAction("Bookings");
        }

        // Get all users and caregivers for dropdowns
        ViewBag.Clients = await _context.AppUsers.Where(u => u.Role == "User").ToListAsync();
        ViewBag.Personnel = await _context.AppUsers.Where(u => u.Role == "Caregiver").ToListAsync();

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
        booking.CaregiverId = model.CaregiverId;

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