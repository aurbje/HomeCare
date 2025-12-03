using HomeCare.Api.DTO;
using HomeCare.Api.Models;
using HomeCare.Api.Enums;

namespace HomeCare.Api.Services.Interfaces
{
    /// <summary>
    /// Service interface for booking operations.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Gets initial booking page data (available dates, time slots, categories, etc.)
        /// </summary>
        Task<BookingInitDto> GetBookingInitAsync(int userId);

        /// <summary>
        /// Creates or updates a booking.
        /// </summary>
        Task<BookingResult> CreateOrUpdateBookingAsync(BookingRequestDto model, int userId);

        /// <summary>
        /// Cancels a booking.
        /// </summary>
        Task<BookingResult> CancelBookingAsync(int bookingId, int userId, bool isAdmin);

        /// <summary>
        /// Gets a specific booking.
        /// </summary>
        Task<BookingDto?> GetBookingAsync(int bookingId, int userId, bool isAdmin);

        /// <summary>
        /// Gets bookings for a specific user.
        /// </summary>
        Task<List<Booking>> GetBookingsForUserAsync(int userId);

        /// <summary>
        /// Gets available caregivers for a specific date and time slot.
        /// </summary>
        Task<List<UserSummaryDto>> GetAvailableCaregiverForSlotAsync(DateTime date, int? timeSlotId, int? bookingId);

        /// <summary>
        /// Checks if a caregiver is already booked.
        /// </summary>
        Task<bool> IsCaregiverBookedAsync(DateTime date, int timeSlotId, int caregiverId, int? excludeBookingId = null);
    }
}
