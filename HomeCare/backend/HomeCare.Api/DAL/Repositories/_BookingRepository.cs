/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in BookingRepository.cs
 * This file kept for reference purposes
 * ============================================================

using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.DAL.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingRepository> _logger;

        public BookingRepository(AppDbContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ------------------------------
        // BOOKINGS CRUD
        // ------------------------------
        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.TimeSlot)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all bookings");
                throw;
            }
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.TimeSlot)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking by ID {Id}", id);
                throw;
            }
        }

        public async Task AddBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding booking");
                throw;
            }
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Update(booking);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {Id}", booking.Id);
                return false;
            }
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null) return false;

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {Id}", id);
                return false;
            }
        }

        // ------------------------------
        // DATES, TIMESLOTS, CATEGORIES
        // ------------------------------
        public async Task<IEnumerable<AvailableDate>> GetAvailableDatesAsync()
        {
            try
            {
                return await _context.AvailableDates
                    .Include(d => d.TimeSlots)
                    .Where(d => d.Date >= DateTime.Today && d.TimeSlots.Any(ts => !ts.IsBooked))
                    .OrderBy(d => d.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available dates");
                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                throw;
            }
        }

        public async Task<TimeSlot?> GetAvailableTimeSlotAsync(int timeSlotId)
        {
            try
            {
                return await _context.TimeSlots
                    .Include(ts => ts.AvailableDate)
                    .FirstOrDefaultAsync(ts => ts.Id == timeSlotId && !ts.IsBooked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available time slot {Id}", timeSlotId);
                throw;
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                return await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category {Id}", categoryId);
                throw;
            }
        }

        public async Task<bool> UpdateTimeSlotAsync(TimeSlot timeSlot)
        {
            try
            {
                _context.TimeSlots.Update(timeSlot);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating time slot {Id}", timeSlot.Id);
                return false;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

*/