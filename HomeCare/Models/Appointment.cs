using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // when the appointment actually happens (date + time)
        [Required]
        public required DateTime DateTime { get; set; }

        // optional notes the user can add
        public string? Notes { get; set; }

        // time slot this appointment is linked to
        [Required]
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        // category for the appointment (like cleaning, nursing, etc.)
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
