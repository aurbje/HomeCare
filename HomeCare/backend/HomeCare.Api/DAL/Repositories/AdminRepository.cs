using HomeCare.Api.Data;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeCare.Api.DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        // --- ILogger has been removed from the constructor ---
        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Users

        public async Task<IEnumerable<User>> GetUsersAsync(string? searchTerm)
        {
            // Base query excludes admins and caregivers. Handles cases where Role might be null.
            IQueryable<User> query = _context.Users
                .Where(u => u.Role != null && u.Role.ToLower() != "caregiver" && u.Role.ToLower() != "admin");

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                // This search is now null-safe.
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.Address != null && u.Address.ToLower().Contains(term)) ||
                    (u.TlfNumber != null && u.TlfNumber.ToLower().Contains(term))
                );
            }
            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            if (user.Role != null && user.Role.ToLower() == "admin" && await CountAdminsAsync() <= 1)
            {
                return false; // Block deleting last admin
            }

            if (await HasClientBookingsAsync(id) || await HasCaregiverBookingsAsync(id))
            {
                return false; // Block deleting user with bookings
            }

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Caregivers

        public async Task<IEnumerable<User>> GetCaregiversAsync(string? searchTerm)
        {
            IQueryable<User> query = _context.Users
                .Where(u => u.Role != null && (u.Role.ToLower() == "caregiver" || u.Role.ToLower() == "admin"));
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)));
            }
            return await query.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<bool> DeleteCaregiverAsync(int id)
        {
            var caregiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role != null && u.Role == "Caregiver");
            if (caregiver == null || await HasCaregiverBookingsAsync(id)) return false;

            _context.Users.Remove(caregiver);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Bookings

        public async Task<IEnumerable<Booking>> GetBookingsAsync(string? searchTerm)
        {
            IQueryable<Booking> query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.TimeSlot)
                .Include(b => b.Category);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(b =>
                    (b.Category != null && b.Category.Name != null && b.Category.Name.ToLower().Contains(term)) ||
                    (b.TimeSlot != null && b.TimeSlot.Slot != null && b.TimeSlot.Slot.ToLower().Contains(term)) ||
                    (b.User != null && (
                        (b.User.FullName != null && b.User.FullName.ToLower().Contains(term)) ||
                        (b.User.Email != null && b.User.Email.ToLower().Contains(term))
                    )));
            }

            return await query.OrderByDescending(b => b.DateTime).ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.TimeSlot)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;
            _context.Bookings.Remove(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Helpers

        public async Task<int> CountAdminsAsync()
        {
            return await _context.Users.CountAsync(u => u.Role != null && u.Role.ToLower() == "admin");
        }

        public async Task<bool> HasClientBookingsAsync(int userId)
        {
            return await _context.Bookings.AnyAsync(b => b.UserId == userId);
        }

        public async Task<bool> HasCaregiverBookingsAsync(int caregiverId)
        {
            return await _context.Bookings.AnyAsync(b => b.CaregiverId == caregiverId);
        }
       
        #endregion
    }
}