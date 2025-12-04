using HomeCare.Api.DTO.User;
using HomeCare.Api.Models;
using HomeCare.Api.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Api.Services.Interfaces
{
    // Service interface for booking operations.
    public interface IBookingService
    {
        // Gets initial booking page data (available dates, time slots, categories, etc.)
        Task<BookingInitDto> GetBookingInitAsync(int userId);

        // Creates or updates a booking.
        
        Task<BookingResultDto> CreateOrUpdateBookingAsync(BookingRequestDto model, int userId);

        // Cancels a booking.

        Task<BookingResultDto> CancelBookingAsync(int bookingId, int userId, bool isAdmin);

        // Gets a specific booking.
        Task<BookingDto?> GetBookingAsync(int bookingId, int userId, bool isAdmin);

        // Gets bookings for a specific user.
        
        Task<List<Booking>> GetBookingsForUserAsync(int userId);

        // Gets available caregivers for a specific date and time slot.
        
        Task<List<UserSummaryDto>> GetAvailableCaregiverForSlotAsync(DateTime date, int? timeSlotId, int? bookingId);

        // Checks if a caregiver is already booked.
        
        Task<bool> IsCaregiverBookedAsync(DateTime date, int timeSlotId, int caregiverId, int? excludeBookingId = null);
    }
}
