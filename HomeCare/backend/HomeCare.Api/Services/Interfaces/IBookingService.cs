using HomeCare.Api.DTO;
using HomeCare.Api.Models;
using HomeCare.Api.Enums;

namespace HomeCare.Api.Services.Interfaces
{
    public interface IBookingService
    {
        /// Gets initial booking page data (available dates, time slots, categories, etc.)
        Task<BookingInitDto> GetBookingInitAsync(int userId);

        /// Creates or updates a booking.
        Task<BookingResult> CreateOrUpdateBookingAsync(BookingRequestDto model, int userId);


        Task<BookingResult> CancelBookingAsync(int bookingId, int userId, bool isAdmin);


        Task<BookingDto?> GetBookingAsync(int bookingId, int userId, bool isAdmin);

        Task<List<Booking>> GetBookingsForUserAsync(int userId);

        /// Gets available caregivers for a specific date and time slot.
        Task<List<UserSummaryDto>> GetAvailableCaregiverForSlotAsync(DateTime date, int? timeSlotId, int? bookingId);

        /// Checks if a caregiver is already booked.

        Task<bool> IsCaregiverBookedAsync(DateTime date, int timeSlotId, int caregiverId, int? excludeBookingId = null);
    }
}
