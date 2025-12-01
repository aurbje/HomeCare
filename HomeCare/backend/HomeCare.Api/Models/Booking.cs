using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // Represents a booking made by a user for a service
    public class Booking
    {
        public int Id { get; set; }

        // When the booking should happen (just the date)
        [Required]
        public DateTime Date { get; set; }

        // Time chosen for the booking ("14:00")
        [Required, StringLength(20)]
        public string Time { get; set; } = string.Empty;

        // Time slot this booking is linked to
        [Required]
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Type of service booked (cleaning, nursing, etc.)
        [Required, StringLength(100)]
        public string ServiceType { get; set; } = string.Empty;

        // Optional message or special request from the user
        [StringLength(500)]
        public string? Notes { get; set; }

        // The user who made the booking
        public int? UserId { get; set; }
        public User? User {get; set; }

        // Optional caregiver assigned to this booking (if used later)
        public string CaregiverId { get; set; } = string.Empty;

        // Current booking status
        [Required, StringLength(20)]
        public string Status { get; set; } = "Booked";
    }
}
