using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HomeCare.Models;

namespace HomeCare.ViewModels.User
{
    // data shown on the user dashboard
    public class DashboardViewModel
    {
        // basic user info
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // summary numbers for dashboard cards
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }

        // when the user logged in the last time
        public DateTime LastLogin { get; set; }

        // simple text notifications for the user
        public List<string> Notifications { get; set; } = new();

        // reminders the user should see today
        public List<Reminder> Reminders { get; set; } = new();

        // all upcoming appointments tied to the user
        public List<Appointment> Appointments { get; set; } = new();
    }
}