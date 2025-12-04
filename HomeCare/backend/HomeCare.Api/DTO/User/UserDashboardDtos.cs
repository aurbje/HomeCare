namespace HomeCare.Api.DTO.User
{
    // admin Dashboard
    public class AdminDashboardDto
    {
        public List<BookingDto> Bookings { get; set; } = new();
        public List<UserDto> Users { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<UserSummaryDto> Caregiver { get; set; } = new();
    }

    // user Dashboard
    public class UserDashboardDto
    {
        public string UserName { get; set; } = string.Empty;
        public List<ReminderDto> Reminders { get; set; } = new();
        public List<BookingSummaryDto> TodayBookings { get; set; } = new();
        public List<BookingSummaryDto> UpcomingBookings { get; set; } = new();
        public List<CalendarBookingDto> CalendarBookings { get; set; } = new();
    }
    // reminder DTO
    public class ReminderDto
    {
        public int Id { get; set; }
        public string Time { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
    // booking summary DTO
    public class CalendarBookingDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? CategoryName { get; set; }
        public string? CaregiverName { get; set; }
        public string? Notes { get; set; }
    }

    // booking Init Response (for booking page)
    public class BookingInitDto
    {
        public BookingFormDataDto Model { get; set; } = new();
        public string ClientName { get; set; } = string.Empty;
        public List<BookingDto> Bookings { get; set; } = new();
    }
    // booking form data DTO
    public class BookingFormDataDto
    {
        public DateTime SelectedDate { get; set; }
        public int TimeSlotId { get; set; }
        public int CategoryId { get; set; }
        public List<AvailableDateDto> AvailableDates { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public List<UserSummaryDto> AvailableCaregiver { get; set; } = new();
    }


}
