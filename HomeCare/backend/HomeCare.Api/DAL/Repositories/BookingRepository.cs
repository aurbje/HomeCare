using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByFullNameAsync(string fullName) =>
            await _context.Users.FirstOrDefaultAsync(u => u.FullName == fullName);

        public async Task AddCaregiverAvailabilityAsync(int CaregiverId, DateTime date)
        {
            var exists = await _context.CaregiverAvailabilities
                .AnyAsync(a => a.CaregiverId == CaregiverId && a.Date.Date == date.Date);

            if (!exists)
            {
                var availability = new CaregiverAvailability
                {
                    CaregiverId = CaregiverId,
                    Date = date
                };

                _context.CaregiverAvailabilities.Add(availability);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AvailableDate?> GetAvailableDateByDateAsync(DateTime date) =>
            await _context.AvailableDates
                .Include(d => d.TimeSlots)
                .FirstOrDefaultAsync(d => d.Date == date);

        public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync() =>
            await _context.AvailableDates
                .Include(d => d.TimeSlots)
                .Where(d => d.Date >= DateTime.Today)
                .OrderBy(d => d.Date)
                .ToListAsync();

        public async Task<IEnumerable<Category>> GetCategoriesAsync() =>
            await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync() =>
            await _context.Bookings
                .Include(a => a.TimeSlot)
                    .ThenInclude(ts => ts.AvailableDate)
                .Include(a => a.Category)
                .Include(a => a.User)
                .Include(a => a.Caregiver)
                .Where(a => a.DateTime >= DateTime.Today)
                .OrderBy(a => a.DateTime)
                .ToListAsync();

        public async Task<Booking?> GetBookingByIdAsync(int id) =>
            await _context.Bookings
                .Include(a => a.TimeSlot)
                    .ThenInclude(ts => ts.AvailableDate)
                .Include(a => a.Category)
                .Include(a => a.User)
                .Include(a => a.Caregiver)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            var existing = await _context.Bookings.FirstOrDefaultAsync(a => a.Id == booking.Id);
            if (existing == null)
                throw new InvalidOperationException($"Booking {booking.Id} not found.");

            existing.TimeSlotId = booking.TimeSlotId;
            existing.DateTime = booking.DateTime;
            existing.CaregiverId = booking.CaregiverId;
            existing.CategoryId = booking.CategoryId;
            existing.Notes = booking.Notes;

            _context.Bookings.Update(existing);
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

        public async Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId) =>
            await _context.TimeSlots.Include(ts => ts.AvailableDate)
            .FirstOrDefaultAsync(ts => ts.Id == timeSlotId);

        public async Task<Category?> GetCategoryByIdAsync(int categoryId) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

        public async Task UpdateTimeSlotAsync(TimeSlot timeSlot)
        {
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAvailableDateAsync(AvailableDate availableDate)
        {
            _context.AvailableDates.Update(availableDate);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<User>> GetAvailableCaregiverByDateAsync(DateTime date) =>
            await _context.CaregiverAvailabilities
                .Where(a => a.Date.Date == date.Date)
                .Select(a => a.Caregiver)
                .Distinct()
                .ToListAsync();

        public async Task<User?> GetUserByIdAsync(int selectedCaregiverId) =>
            await _context.Users.FindAsync(selectedCaregiverId);
    }
}
