namespace HomeCare.Api.DTO
{
    // DTO for Caregiver Dashboard
    public class CaregiverDashboardDto
    {
        public string CaregiverName { get; set; } = string.Empty;
        public int CaregiverId { get; set; }
        public List<DateTime> AvailableDates { get; set; } = new();
        public List<VisitInfoDto> TodayVisits { get; set; } = new();
        public List<CalendarEventDto> CalendarEvents { get; set; } = new();
        public List<BookingSummaryDto> UpcomingBookings { get; set; } = new();
    }
// DTO for Visit Information
    public class VisitInfoDto
    {
        public DateTime Time { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> Tasks { get; set; } = new();
    }
// DTO for Booking Summary
    public class CalendarEventDto
    {
        public DateTime StartTime { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
