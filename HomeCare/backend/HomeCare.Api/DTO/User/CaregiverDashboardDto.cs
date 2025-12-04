namespace HomeCare.Api.DTO.User
{
    public class CaregiverDashboardDto   // DTO for Caregiver Dashboard
    {
        public string CaregiverName { get; set; } = string.Empty;
        public int CaregiverId { get; set; }
        public List<DateTime> AvailableDates { get; set; } = new();
        public List<VisitInfoDto> TodayVisits { get; set; } = new();
        public List<CalendarEventDto> CalendarEvents { get; set; } = new();
        public List<BookingSummaryDto> UpcomingBookings { get; set; } = new();
        public List<BookingSummaryDto> PastBookings { get; set; } = new();
    }

    public class VisitInfoDto // DTO for Visit Information
    {
        public DateTime Time { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> Tasks { get; set; } = new();
    }

    public class CalendarEventDto // DTO for Booking Summary
    {
        public DateTime StartTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
