using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HomeCare.Api.Models;

namespace HomeCare.Api.DTO.User
{
    // DTO representing all data shown on the user's dashboard
    public class DashboardDto
    {
        // Basic user info
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Summary numbers for dashboard cards
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }

        // Last login timestamp
        public DateTime LastLogin { get; set; }

        // Simple text notifications for the user
        public List<string> Notifications { get; set; } = new();

        // Reminders the user should see today
        public List<Reminder> Reminders { get; set; } = new();

        // All upcoming appointments tied to the user
        public List<Booking> Bookings { get; set; } = new();
    }
}
