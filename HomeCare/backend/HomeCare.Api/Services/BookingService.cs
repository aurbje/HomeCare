using HomeCare.Api.Data;
using HomeCare.Api.DTO;
using HomeCare.Api.Enums;
using HomeCare.Api.Services.Interfaces;
using HomeCare.Api.Models;
using HomeCare.Api.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HomeCare.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository repo, ILogger<BookingService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<Booking>> GetBookingsForUserAsync(int userId)
        {
            var all = await _repo.GetUpcomingBookingsAsync();
            return all.Where(a => a.UserId == userId && a.DateTime >= DateTime.Today)
                      .OrderBy(a => a.DateTime)
                      .ToList();
        }

        public async Task<List<UserSummaryDto>> GetAvailableCaregiverForSlotAsync(DateTime date, int? timeSlotId, int? bookingId)
        {
            if (!timeSlotId.HasValue) return new();

            var allCaregiver = await _repo.GetAvailableCaregiverByDateAsync(date);
            var bookings = await _repo.GetUpcomingBookingsAsync();
            var bookedIds = bookings
                .Where(a => a.DateTime.Date == date.Date && a.TimeSlotId == timeSlotId.Value && (!bookingId.HasValue || a.Id != bookingId.Value))
                .Select(a => a.CaregiverId)
                .Where(pid => pid.HasValue)
                .Select(pid => pid!.Value)
                .ToHashSet();

            return allCaregiver
                .Where(p => !bookedIds.Contains(p.Id))
                .Select(p => new UserSummaryDto { Id = p.Id, FullName = p.FullName })
                .ToList();
        }

        public async Task<bool> IsCaregiverBookedAsync(DateTime date, int timeSlotId, int CaregiverId, int? excludeBookingId = null)
        {
            var bookings = await _repo.GetUpcomingBookingsAsync();
            return bookings.Any(a => a.DateTime.Date == date.Date && a.TimeSlotId == timeSlotId && a.CaregiverId == CaregiverId && (!excludeBookingId.HasValue || a.Id != excludeBookingId.Value));
        }

        public bool IsBookingTimeAvailable(DateTime date, string timeString)
        {
            if (!TimeSpan.TryParse(timeString, out var time)) return false;
            var start = new TimeSpan(8, 0, 0);
            var end = new TimeSpan(15, 0, 0);
            return time >= start && time < end;
        }

        public async Task<BookingInitDto> GetBookingInitAsync(int userId)
        {
            _logger.LogInformation("Loading booking init for user {UserId}", userId);

            var availableDates = await _repo.GetAvailableDatesAsync() ?? Enumerable.Empty<AvailableDate>();
            var categories = await _repo.GetCategoriesAsync() ?? Enumerable.Empty<Category>();

            var selectedDate = DateTime.Today;
            var categoryId = categories.FirstOrDefault()?.Id ?? 0;

            var client = await _repo.GetUserByIdAsync(userId);
            var clientName = client?.FullName ?? string.Empty;
            var bookings = await GetBookingsForUserAsync(userId) ?? new List<Booking>();

            var datesDto = availableDates.Select(d => new AvailableDateDto
            {
                Id = d.Id,
                Date = d.Date,
                TimeSlots = (d.TimeSlots ?? Enumerable.Empty<TimeSlot>()).Select(ts => new TimeSlotDto
                {
                    Id = ts.Id,
                    Slot = ts.Slot,
                    IsBooked = ts.IsBooked
                }).ToList()
            }).ToList();

            var categoriesDto = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();

            var apptsDto = bookings.Select(a => new BookingDto
            {
                Id = a.Id,
                DateTime = a.DateTime,
                TimeSlotId = a.TimeSlotId,
                Category = a.Category == null ? null : new CategoryDto { Id = a.Category.Id, Name = a.Category.Name },
                Caregiver = a.Caregiver == null ? null : new UserSummaryDto { Id = a.Caregiver.Id, FullName = a.Caregiver.FullName },
                Client = a.Client == null ? null : new UserSummaryDto { Id = a.Client.Id, FullName = a.Client.FullName },
                Notes = a.Notes,
            }).ToList();

            return new BookingInitDto
            {
                Model = new BookingFormDataDto
                {
                    SelectedDate = selectedDate,
                    TimeSlotId = 0,
                    CategoryId = categoryId,
                    AvailableDates = datesDto,
                    Categories = categoriesDto,
                    AvailableCaregiver = new List<UserSummaryDto>()
                },
                ClientName = clientName,
                Bookings = apptsDto
            };
        }

        public async Task<BookingResult> CreateOrUpdateBookingAsync(BookingRequestDto model, int UserId)
        {
            _logger.LogInformation("Booking attempt for categoryId {CategoryId} on {Date}", model.CategoryId, model.SelectedDate);

            var errors = new Dictionary<string, string>();
            var selectedCategory = await _repo.GetCategoryByIdAsync(model.CategoryId);

            // Validate: Require notes if category is "Annet"
            if (selectedCategory?.Name.ToUpper() == "Annet" && string.IsNullOrWhiteSpace(model.Notes))
            {
                _logger.LogWarning("Booking validation failed: 'Other' category requires notes.");
                errors["Notes"] = "Please provide details for 'Other' category.";
            }

            // Validate time slot
            TimeSlot? selectedSlot = null;
            if (!model.TimeSlotId.HasValue)
            {
                errors["TimeSlotId"] = "Vennligst velg et tidspunkt.";
            }
            else
            {
                selectedSlot = await _repo.GetAvailableTimeSlotAsync(model.TimeSlotId.Value);
                if (selectedSlot == null)
                {
                    _logger.LogWarning("Time slot {TimeSlotId} is unavailable.", model.TimeSlotId);
                    errors["TimeSlotId"] = "Valgt tidspunkt er ikke lenger tilgjengelig.";
                }
                else if (selectedSlot.AvailableDate == null)
                {
                    _logger.LogWarning("Time slot {TimeSlotId} has no available date.", model.TimeSlotId);
                    errors["TimeSlotId"] = "Tidspunktet mangler tilknyttet dato.";
                }
            }

            // Parse start time from slot string
            TimeSpan startTime = default;
            bool validTime = false;
            if (selectedSlot != null)
            {
                var slotParts = selectedSlot.Slot.Replace("–", "-").Split('-');
                if (slotParts.Length == 2 && TimeSpan.TryParse(slotParts[0], out startTime))
                {
                    validTime = true;
                }
                else
                {
                    _logger.LogError("Invalid time slot format: {Slot}", selectedSlot.Slot);
                    errors["TimeSlotId"] = "Invalid time slot format.";
                }
            }

            // Validate Caregiver availability
            if (errors.Count == 0 && validTime && model.SelectedCaregiverId.HasValue && selectedSlot?.AvailableDate != null)
            {
                var date = selectedSlot.AvailableDate.Date.Date;
                var conflict = await IsCaregiverBookedAsync(date, selectedSlot.Id, model.SelectedCaregiverId.Value, model.BookingId == 0 ? null : model.BookingId);
                if (conflict)
                {
                    _logger.LogWarning("Caregiver {CaregiverId} already booked for date {Date} slot {SlotId}.", model.SelectedCaregiverId, date, selectedSlot.Id);
                    errors["SelectedCaregiverId"] = "Valgt ansatt er allerede booket på dette tidspunktet.";
                }
            }

            if (errors.Count > 0)
            {
                return new BookingResult { Success = false, ValidationErrors = errors, ResultType = BookingResultType.ValidationError };
            }

            // Validate Caregiver exists
            if (!model.SelectedCaregiverId.HasValue)
            {
                errors["SelectedCaregiverId"] = "Please select a Caregiver.";
                return new BookingResult { Success = false, ValidationErrors = errors, ResultType = BookingResultType.ValidationError };
            }

            var Caregiver = await _repo.GetUserByIdAsync(model.SelectedCaregiverId.Value);
            if (Caregiver == null)
            {
                _logger.LogWarning("Caregiver ID {Id} not found.", model.SelectedCaregiverId);
                errors["SelectedCaregiverId"] = "Valgt ansatt finnes ikke.";
                return new BookingResult { Success = false, ValidationErrors = errors, ResultType = BookingResultType.ValidationError };
            }

            // Update existing booking
            if (model.BookingId > 0)
            {
                var existing = await _repo.GetBookingByIdAsync(model.BookingId);
                if (existing == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found for update.", model.BookingId);
                    return new BookingResult { Success = false, ResultType = BookingResultType.NotFound, Message = "Booking not found." };
                }

                if (selectedSlot?.AvailableDate != null && selectedCategory != null)
                {
                    existing.DateTime = selectedSlot.AvailableDate.Date.Add(startTime);
                    existing.TimeSlotId = selectedSlot.Id;
                    existing.CategoryId = selectedCategory.Id;
                    existing.Notes = model.Notes;
                    existing.CaregiverId = model.SelectedCaregiverId;

                    await _repo.UpdateBookingAsync(existing);
                    _logger.LogInformation("Booking {BookingId} updated.", existing.Id);

                    return new BookingResult { Success = true, BookingId = existing.Id, Message = "Booking updated.", ResultType = BookingResultType.Success };
                }
                else
                {
                    errors["TimeSlotId"] = "Valgt tidspunkt er ikke lenger tilgjengelig.";
                    return new BookingResult { Success = false, ValidationErrors = errors, ResultType = BookingResultType.ValidationError };
                }
            }

            // Create new booking
            if (selectedSlot?.AvailableDate == null || selectedCategory == null)
            {
                errors["TimeSlotId"] = "Valgt tidspunkt er ikke lenger tilgjengelig.";
                return new BookingResult { Success = false, ValidationErrors = errors, ResultType = BookingResultType.ValidationError };
            }

            var booking = new Booking
            {
                DateTime = selectedSlot.AvailableDate.Date.Add(startTime),
                TimeSlotId = selectedSlot.Id,
                CategoryId = selectedCategory.Id,
                Notes = model.Notes,
                CaregiverId = model.SelectedCaregiverId,
                UserId = UserId
            };

            await _repo.AddBookingAsync(booking);
            _logger.LogInformation("Booking created for {DateTime} category {CategoryId}.", booking.DateTime, booking.CategoryId);

            return new BookingResult { Success = true, BookingId = booking.Id, Message = "Booking booked.", ResultType = BookingResultType.Success };
        }

        public async Task<BookingResult> CancelBookingAsync(int bookingId, int userId, bool isAdmin)
        {
            _logger.LogInformation("Cancel request for booking {BookingId} by user {UserId}.", bookingId, userId);

            var booking = await _repo.GetBookingByIdAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found.", bookingId);
                return new BookingResult { Success = false, ResultType = BookingResultType.NotFound, Message = "Booking not found." };
            }

            if (booking.UserId != userId && !isAdmin)
            {
                _logger.LogWarning("User {UserId} attempted to cancel booking {BookingId} they don't own.", userId, bookingId);
                return new BookingResult { Success = false, ResultType = BookingResultType.Forbidden, Message = "Not authorized to cancel this booking." };
            }

            await _repo.DeleteBookingAsync(bookingId);
            _logger.LogInformation("Booking {BookingId} cancelled.", bookingId);

            return new BookingResult { Success = true, ResultType = BookingResultType.Success, Message = "Booking cancelled." };
        }

        public async Task<BookingDto?> GetBookingAsync(int bookingId, int userId, bool isAdmin)
        {
            var booking = await _repo.GetBookingByIdAsync(bookingId);
            if (booking == null) return null;

            // Verify access: client, Caregiver, or admin
            if (booking.UserId != userId && booking.CaregiverId != userId && !isAdmin)
            {
                return null;
            }

            return new BookingDto
            {
                Id = booking.Id,
                DateTime = booking.DateTime,
                TimeSlotId = booking.TimeSlotId,
                Category = booking.Category == null ? null : new CategoryDto { Id = booking.Category.Id, Name = booking.Category.Name },
                Caregiver = booking.Caregiver == null ? null : new UserSummaryDto { Id = booking.Caregiver.Id, FullName = booking.Caregiver.FullName },
                Client = booking.Client == null ? null : new UserSummaryDto { Id = booking.Client.Id, FullName = booking.Client.FullName },
                Notes = booking.Notes
            };
        }
    }
}
