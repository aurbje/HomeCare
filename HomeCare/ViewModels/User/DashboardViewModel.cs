using System;
using System.Collections.Generic;
using HomeCare.Models;

namespace HomeCare.ViewModels.User
{
    public class DashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public DateTime LastLogin { get; set; }
        public List<string> Notifications { get; set; } = new();
        public List<Reminder> Reminders { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
    }
}
