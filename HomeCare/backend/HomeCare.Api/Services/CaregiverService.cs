using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using System.Security.Claims;
using HomeCare.Api.DTO.Shared;

namespace HomeCare.Api.Services
{
    public class CaregiverService
    {
        private readonly ICaregiverRepository _caregiverRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly ILogger<CaregiverService> _logger;

        public CaregiverService(ICaregiverRepository caregiverRepo, IBookingRepository bookingRepo ,ILogger<CaregiverService> logger)
        {
            _caregiverRepo = caregiverRepo;
            _bookingRepo = bookingRepo;
            _logger = logger;
        }

        // ------------------------------
        // GET CLIENTS FOR CAREGIVER
        // ------------------------------
        public async Task<ServiceResponse<IEnumerable<User>>> GetClientsAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<User>>.FailResponse("User not authenticated");

                var clients = await _caregiverRepo.GetClientsForCaregiverAsync(caregiverId);
                _logger.LogInformation("Loaded {Count} clients for caregiver {CaregiverId}", clients.Count(), caregiverId);

                return ServiceResponse<IEnumerable<User>>.SuccessResponse(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching clients for caregiver");
                return ServiceResponse<IEnumerable<User>>.FailResponse("Failed to load clients for caregiver");
            }
        }

        // ------------------------------
        // GET CAREGIVER SCHEDULE
        // ------------------------------
        public async Task<ServiceResponse<IEnumerable<Booking>>> GetScheduleAsync(ClaimsPrincipal user)
        {
            try
            {
                var caregiverId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (caregiverId == null)
                    return ServiceResponse<IEnumerable<Booking>>.FailResponse("User not authenticated");

                var schedule = await _caregiverRepo.GetBookingsForCaregiverAsync(caregiverId);
                _logger.LogInformation("Loaded {Count} bookings for caregiver {CaregiverId}", schedule.Count(), caregiverId);

                return ServiceResponse<IEnumerable<Booking>>.SuccessResponse(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching schedule for caregiver");
                return ServiceResponse<IEnumerable<Booking>>.FailResponse("Failed to load caregiver schedule");
            }
        }

        public async Task<ServiceResponse<object>> CompleteVisitAsync(int bookingId)
        {
            try
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(bookingId);
                if (booking == null)
                    return ServiceResponse<object>.FailResponse("Booking not found");

                booking.Status = "Completed";
                await _bookingRepo.UpdateBookingAsync(booking);

                _logger.LogInformation("Marked booking {BookingId} as completed", bookingId);

                return ServiceResponse<object>.SuccessResponse(new
                {
                    message = "Visit marked as completed",
                    booking
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing visit for booking {BookingId}", bookingId);
                return ServiceResponse<object>.FailResponse("Failed to complete visit");
            }
        }


    }

}
