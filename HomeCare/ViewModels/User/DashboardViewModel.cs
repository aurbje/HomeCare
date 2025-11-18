using System;
using System.Collections.Generic;
using HomeCare.Models;

namespace HomeCare.ViewModels.User
{
    public class DashboardViewModel // ViewModel for user dashboard
    {
        public string FullName { get; set; } = string.Empty; // name
        public string Email { get; set; } = string.Empty; // email
        public int UpcomingBookings { get; set; } // bookings upcoming
        public int CompletedBookings { get; set; } // bookings completed
        public DateTime LastLogin { get; set; } // last login time
        public List<string> Notifications { get; set; } = new();
        public List<Reminder> Reminders { get; set; } = new(); // reminders for user
        public List<Appointment> Appointments { get; set; } = new(); // appointments for user
    }
}
