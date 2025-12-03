using HomeCare.Api.Enums;

namespace HomeCare.Api.DTO.User
{
    public class BookingDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int? TimeSlotId { get; set; }
        public CategoryDto? Category { get; set; }
        public UserSummaryDto? Caregiver { get; set; }
        public UserSummaryDto? User { get; set; }
        public string? Notes { get; set; }
        public BookingStatus Status { get; set; }

        // Alias property for backward compatibility with code using "Client" instead of "User"
        public UserSummaryDto? Client
        {
            get => User;
            set => User = value;
        }
    }

    public class BookingSummaryDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? CategoryName { get; set; }
        public string? CaregiverName { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? TlfNumber { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateBookingDto
    {
        public DateTime? DateTime { get; set; }
        public string? Notes { get; set; }
        public int? CaregiverId { get; set; }
        public int CategoryId { get; set; }
        public BookingStatus Status { get; set; }
    }
}
