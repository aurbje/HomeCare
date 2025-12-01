using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using System.Security.Claims;
using HomeCare.Api.DTO.Shared;

namespace HomeCare.Api.Services
{
    public class BookingService
    {
        private readonly IBookingRepository _repo;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository repo, ILogger<BookingService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // ------------------------------
        // USER BOOKING MANAGEMENT
        // ------------------------------
        public async Task<ServiceResponse<IEnumerable<AvailableDate>>> GetAvailableDatesAsync()
        {
            try
            {
                var dates = await _repo.GetAvailableDatesAsync();
                return ServiceResponse<IEnumerable<AvailableDate>>.SuccessResponse(dates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available dates");
                return ServiceResponse<IEnumerable<AvailableDate>>.FailResponse("Failed to load available dates");
            }
        }

        public async Task<ServiceResponse<object>> CreateBookingAsync(Booking dto, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                    return ServiceResponse<object>.FailResponse("User not authenticated");

                var timeSlot = await _repo.GetAvailableTimeSlotAsync(dto.TimeSlotId);
                if (timeSlot == null || timeSlot.IsBooked)
                    return ServiceResponse<object>.FailResponse("Time slot not available");

                // create booking
                var booking = new Booking
                {
                    Date = timeSlot.AvailableDate.Date,
                    Time = timeSlot.Slot,
                    TimeSlotId = timeSlot.Id,
                    CategoryId = dto.CategoryId,
                    Notes = dto.Notes,
                    UserId = int.Parse(userId),
                    Status = "Booked"
                };

                await _repo.AddBookingAsync(booking);

                // mark slot as booked
                timeSlot.IsBooked = true;
                await _repo.UpdateTimeSlotAsync(timeSlot);

                _logger.LogInformation("Created booking for user {UserId} at {Date} {Time}", userId, booking.Date, booking.Time);

                return ServiceResponse<object>.SuccessResponse(new
                {
                    message = "Booking created successfully",
                    booking
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return ServiceResponse<object>.FailResponse("Unexpected error while creating booking");
            }
        }

        public async Task<ServiceResponse<object>> UpdateBookingStatusAsync(int id, string status)
        {
            try
            {
                var booking = await _repo.GetBookingByIdAsync(id);
                if (booking == null)
                    return ServiceResponse<object>.FailResponse("Booking not found");

                booking.Status = status;
                await _repo.UpdateBookingAsync(booking);

                _logger.LogInformation("Updated booking {Id} status to {Status}", id, status);

                return ServiceResponse<object>.SuccessResponse(new { message = "Status updated", booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status for {Id}", id);
                return ServiceResponse<object>.FailResponse("Error updating booking status");
            }
        }

        public async Task<ServiceResponse<object>> CompleteVisitAsync(int bookingId)
        {
            try
            {
                var booking = await _repo.GetBookingByIdAsync(bookingId);
                if (booking == null)
                    return ServiceResponse<object>.FailResponse("Booking not found");

                booking.Status = "Completed";
                await _repo.UpdateBookingAsync(booking);

                _logger.LogInformation("Marked booking {BookingId} as completed", bookingId);

                return ServiceResponse<object>.SuccessResponse(new { message = "Visit marked as completed", booking });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing visit for booking {BookingId}", bookingId);
                return ServiceResponse<object>.FailResponse("Failed to complete visit");
            }
        }
    }
}
