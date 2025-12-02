using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.Shared;

namespace HomeCare.Api.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepo;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository adminRepo, ILogger<AdminService> logger)
        {
            _adminRepo = adminRepo;
            _logger = logger;
        }

        //Users 
        public async Task<ServiceResponse<IEnumerable<User>>> GetUsersAsync(string? searchTerm)
        {
            try
            {
                var users = await _adminRepo.GetUsersAsync(searchTerm);
                _logger.LogInformation("Loaded {Count} users with search term: {SearchTerm}", users.Count(), searchTerm ?? "none");
                return ServiceResponse<IEnumerable<User>>.SuccessResponse(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return ServiceResponse<IEnumerable<User>>.FailResponse("Failed to load users");
            }
        }

        public async Task<ServiceResponse<string>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _adminRepo.GetUserByIdAsync(id);
                if (user == null)
                    return ServiceResponse<string>.FailResponse("Bruker ikke funnet");

                if (user.Role == "Admin" && await _adminRepo.CountAdminsAsync() <= 1)
                    return ServiceResponse<string>.FailResponse("Kan ikke slette den siste admin-brukeren");

                if (await _adminRepo.HasClientBookingsAsync(id))
                    return ServiceResponse<string>.FailResponse("Kan ikke slette bruker med aktive bookinger");

                var success = await _adminRepo.DeleteUserAsync(id);
                if (!success)
                    return ServiceResponse<string>.FailResponse("Kunne ikke slette bruker");

                _logger.LogInformation("Deleted user {UserId} - {UserName}", id, user.FullName);
                return ServiceResponse<string>.SuccessResponse($"Bruker {user.FullName} ble slettet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved sletting av bruker");
            }
        }

        //Caregivers
        public async Task<ServiceResponse<IEnumerable<User>>> GetCaregiversAsync(string? searchTerm)
        {
            try
            {
                var caregivers = await _adminRepo.GetCaregiversAsync(searchTerm);
                _logger.LogInformation("Loaded {Count} caregivers", caregivers.Count());
                return ServiceResponse<IEnumerable<User>>.SuccessResponse(caregivers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching caregivers");
                return ServiceResponse<IEnumerable<User>>.FailResponse("Failed to load caregivers");
            }
        }

        public async Task<ServiceResponse<string>> DeleteCaregiverAsync(int id)
        {
            try
            {
                var caregiver = await _adminRepo.GetUserByIdAsync(id);
                if (caregiver == null || caregiver.Role != "Caregiver")
                    return ServiceResponse<string>.FailResponse("Caregiver ikke funnet");

                if (await _adminRepo.HasCaregiverBookingsAsync(id))
                    return ServiceResponse<string>.FailResponse("Kan ikke slette caregiver med aktive bookinger");

                var success = await _adminRepo.DeleteCaregiverAsync(id);
                if (!success)
                    return ServiceResponse<string>.FailResponse("Kunne ikke slette caregiver");

                _logger.LogInformation("Deleted caregiver {CaregiverId} - {CaregiverName}", id, caregiver.FullName);
                return ServiceResponse<string>.SuccessResponse($"Caregiver {caregiver.FullName} ble slettet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting caregiver {CaregiverId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved sletting av caregiver");
            }
        }

        //Bookings
        public async Task<ServiceResponse<IEnumerable<Booking>>> GetBookingsAsync(string? searchTerm)
        {
            try
            {
                var bookings = await _adminRepo.GetBookingsAsync(searchTerm);
                _logger.LogInformation("Loaded {Count} bookings", bookings.Count());
                return ServiceResponse<IEnumerable<Booking>>.SuccessResponse(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings");
                return ServiceResponse<IEnumerable<Booking>>.FailResponse("Failed to load bookings");
            }
        }

        public async Task<ServiceResponse<string>> DeleteBookingAsync(int id)
        {
            try
            {
                var booking = await _adminRepo.GetBookingByIdAsync(id);
                if (booking == null)
                    return ServiceResponse<string>.FailResponse("Booking ikke funnet");

                var success = await _adminRepo.DeleteBookingAsync(id);
                if (!success)
                    return ServiceResponse<string>.FailResponse("Kunne ikke slette booking");

                _logger.LogInformation("Deleted booking {BookingId}", id);
                return ServiceResponse<string>.SuccessResponse($"Booking {id} ble slettet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {BookingId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved sletting av booking");
            }
        }
    }
}