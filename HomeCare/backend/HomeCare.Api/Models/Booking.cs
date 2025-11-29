using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    // simple booking model used for basic service bookings
    public class Booking
    {
        public int Id { get; set; }

        // when the booking should happen (just the date)
        [Required]
        public DateTime Date { get; set; }

        // time chosen for the booking ("14:00")
        [Required]
        public string? Time { get; set; }

        // what kind of service the user wants
        [Required]
        public string? ServiceType { get; set; }

        // optional message from the user
        public string? Notes { get; set; }

        // user who created the booking (optional until feature is complete)
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}