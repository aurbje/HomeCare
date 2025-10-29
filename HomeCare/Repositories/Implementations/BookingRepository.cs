using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all appointments (bookings)
        public async Task<IEnumerable<Appointment>> GetAllBookingsAsync()
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)   // Include related timeslot info
                .Include(a => a.Category)   // Include category info
                .ToListAsync();
        }

        // Get a single appointment by ID
        public async Task<Appointment?> GetBookingByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Add a new appointment
        public async Task AddBookingAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        // Update an existing appointment
        public async Task UpdateBookingAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        // Delete an appointment by ID
        public async Task DeleteBookingAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync()
        {
            return await _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
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

        public async Task DeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId)
        {
            return await _context.TimeSlots
                .Include(ts => ts.AvailableDate)
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId && !ts.IsBooked);
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task UpdateTimeSlotAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
