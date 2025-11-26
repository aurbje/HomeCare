using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Repositories.Implementations;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync()
    {
        return await _context.AvailableDates
            .OrderBy(d => d.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeSlot>> GetTimeSlotsForDateAsync(int dateId)
    {
        return await _context.TimeSlots
            .Where(ts => ts.AvailableDateId == dateId)
            .OrderBy(ts => ts.Slot)
            .ToListAsync();
    }

    public async Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId)
    {
        return await _context.TimeSlots
            .Include(ts => ts.AvailableDate)
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);
    }

    public async Task UpdateTimeSlotAsync(TimeSlot slot)
    {
        _context.TimeSlots.Update(slot);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int categoryId)
    {
        return await _context.Categories.FindAsync(categoryId);
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id)
    {
        return await _context.Appointments
            .Include(a => a.TimeSlot)
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAppointmentAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAppointmentAsync(int appointmentId)
    {
        var appt = await _context.Appointments.FindAsync(appointmentId);
        if (appt != null)
        {
            _context.Appointments.Remove(appt);
            await _context.SaveChangesAsync();
        }
    }
}
