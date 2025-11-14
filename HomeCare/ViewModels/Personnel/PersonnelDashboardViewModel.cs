

namespace HomeCare.ViewModels.Personnel
{
    public class PersonnelDashboardViewModel
    {
        public string PersonnelName { get; set; }
        public List<DateTime> AvailableDates { get; set; }
        public List<VisitInfo> TodayVisits { get; set; }
        public List<CalendarEvent> CalendarEvents { get; set; }

    }

    public class VisitInfo
    {
        public DateTime Time { get; set; }
        public string ClientName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public List<string> Tasks { get; set; } = new();
    }

    public class CalendarEvent
    {
        public DateTime StartTime { get; set; }
        public string Title { get; set; }
        public string ClientName { get; set; }
        public string CategoryName { get; set; }
    }
}