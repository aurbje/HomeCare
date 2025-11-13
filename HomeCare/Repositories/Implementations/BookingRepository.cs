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

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Include(b => b.User) // includes user data if needed
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AvailableDate?> GetAvailableDateByDateAsync(DateTime date)
        {
            return await _context.AvailableDates
                .Include(d => d.TimeSlots)
                .FirstOrDefaultAsync(d => d.Date == date);
        }


        public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync()
        {
            return await _context.AvailableDates
                .Include(d => d.TimeSlots)
                //.Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .Where(d => d.Date >= DateTime.Today) // show all
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
            return await _context.TimeSlots.Include(ts => ts.AvailableDate)
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId); // fixed
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

        // added after the error CS1061
        public async Task UpdateAvailableDateAsync(AvailableDate availableDate)
        {
            _context.AvailableDates.Update(availableDate);
            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
