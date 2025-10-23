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
    }
}
