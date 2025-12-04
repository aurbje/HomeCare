using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using HomeCare.Api.DTO.Shared;
using HomeCare.Api.DTO.Admin;

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

        // users
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
// delete user with checks
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
        // Get user by ID
        public async Task<ServiceResponse<User>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _adminRepo.GetUserByIdAsync(id);
                if (user == null)
                {
                    return ServiceResponse<User>.FailResponse("Bruker ikke funnet");
                }
                return ServiceResponse<User>.SuccessResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID {UserId}", id);
                return ServiceResponse<User>.FailResponse("Feil ved henting av bruker");
            }
        }
        // Update user
        public async Task<ServiceResponse<string>> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            try
            {
                var existingUser = await _adminRepo.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return ServiceResponse<string>.FailResponse("Bruker ikke funnet");
                }

                // map properties from your DTO to the existing user entity
                existingUser.FullName = userDto.FullName;
                existingUser.Email = userDto.Email;
                existingUser.TlfNumber = userDto.TlfNumber;
                existingUser.Address = userDto.Address;

                var success = await _adminRepo.UpdateUserAsync(existingUser);
                if (!success)
                {
                    return ServiceResponse<string>.FailResponse("Kunne ikke oppdatere bruker");
                }

                _logger.LogInformation("Updated user {UserId}", id);
                return ServiceResponse<string>.SuccessResponse("Bruker ble oppdatert");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved oppdatering av bruker");
            }
        }
        // caregivers
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
// Get caregiver by ID with role check
        public async Task<ServiceResponse<User>> GetCaregiverByIdAsync(int id)
        {
            try
            {
                var user = await _adminRepo.GetUserByIdAsync(id);
                // CORRECTED LOGIC: Check if the user's role IS "Caregiver" or "Admin"
                if (user == null || (user.Role?.ToLower() != "caregiver" && user.Role?.ToLower() != "admin"))
                {
                    return ServiceResponse<User>.FailResponse("Ansatt ikke funnet");
                }
                return ServiceResponse<User>.SuccessResponse(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching caregiver by ID {UserId}", id);
                return ServiceResponse<User>.FailResponse("Feil ved henting av ansatt");
            }
        }
// Update caregiver with role check
        public async Task<ServiceResponse<string>> UpdateCaregiverAsync(int id, UpdateCaregiverDto caregiverDto)
        {
            try
            {
                var existingUser = await _adminRepo.GetUserByIdAsync(id);
                if (existingUser == null || (existingUser.Role?.ToLower() != "caregiver" && existingUser.Role?.ToLower() != "admin"))
                {
                    return ServiceResponse<string>.FailResponse("Ansatt ikke funnet");
                }

                existingUser.FullName = caregiverDto.FullName;
                existingUser.Email = caregiverDto.Email;
                existingUser.TlfNumber = caregiverDto.TlfNumber;
                existingUser.Address = caregiverDto.Address;

                var success = await _adminRepo.UpdateUserAsync(existingUser);
                if (!success)
                {
                    return ServiceResponse<string>.FailResponse("Kunne ikke oppdatere ansatt");
                }

                _logger.LogInformation("Updated caregiver {UserId}", id);
                return ServiceResponse<string>.SuccessResponse("Ansatt ble oppdatert");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating caregiver {UserId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved oppdatering av ansatt");
            }
        }
// delete caregiver with checks
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

        //bookings
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
// delete booking
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
        // Get booking by ID
        public async Task<ServiceResponse<Booking>> GetBookingByIdAsync(int id)
        {
            try
            {
                var booking = await _adminRepo.GetBookingByIdAsync(id);
                if (booking == null)
                {
                    return ServiceResponse<Booking>.FailResponse("Booking ikke funnet");
                }
                return ServiceResponse<Booking>.SuccessResponse(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking by ID {BookingId}", id);
                return ServiceResponse<Booking>.FailResponse("Feil ved henting av booking");
            }
        }
        // Update booking
        public async Task<ServiceResponse<string>> UpdateBookingAsync(int id, UpdateBookingDto bookingDto)
        {
            try
            {
                var existingBooking = await _adminRepo.GetBookingByIdAsync(id);
                if (existingBooking == null)
                {
                    return ServiceResponse<string>.FailResponse("Booking ikke funnet");
                }

                // Map properties from DTO to the existing booking entity
                existingBooking.UserId = bookingDto.UserId;
                existingBooking.CaregiverId = bookingDto.CaregiverId;
                existingBooking.DateTime = bookingDto.DateTime;
                existingBooking.TimeSlotId = bookingDto.TimeSlotId;
                existingBooking.CategoryId = bookingDto.CategoryId;
                existingBooking.Notes = bookingDto.Notes;

                var success = await _adminRepo.UpdateBookingAsync(existingBooking);
                if (!success)
                {
                    return ServiceResponse<string>.FailResponse("Kunne ikke oppdatere booking");
                }

                _logger.LogInformation("Updated booking {BookingId}", id);
                return ServiceResponse<string>.SuccessResponse("Booking ble oppdatert");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId}", id);
                return ServiceResponse<string>.FailResponse("Feil ved oppdatering av booking"); // error message
            }
        }
    }
}