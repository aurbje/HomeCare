using HomeCare.Data;
using HomeCare.Models;
using HomeCare.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository // booking repository implementation
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

        public async Task AddBookingAsync(Booking booking) // add new booking
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking) // update existing booking
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(int id) // delete booking by id
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync() // get available dates with unbooked time slots
        {
            return await _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync() // get all service categories
        {
            return await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync() // get upcoming appointments
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
        }

         public async Task<Appointment?> GetAppointmentByIdAsync(int id) // get appointment by id
        {
            return await _context.Appointments
                .Include(a => a.TimeSlot)
                .Include(a => a.Category)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAppointmentAsync(Appointment appointment) // add new appointment
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment) // update existing appointment
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(int id) // delete appointment by id
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId) // get available time slot by id
        {
            return await _context.TimeSlots
                .Include(ts => ts.AvailableDate)
                .FirstOrDefaultAsync(ts => ts.Id == timeSlotId && !ts.IsBooked);
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId) // category by id
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task UpdateTimeSlotAsync(TimeSlot timeSlot) // update time slot
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }


        public async Task SaveChangesAsync() // save changes to the context
        {
            await _context.SaveChangesAsync();
        }
    }
}
