using System;

using System.ComponentModel.DataAnnotations;


namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; } // date and time of appointment
        public string? Notes { get; set; } // optional notes
        public int TimeSlotId { get; set; } // foreign key
        public TimeSlot TimeSlot { get; set; } = null!; // never null

        public int CategoryId { get; set; } // foreign key
        public Category Category { get; set; } = null!; // never null
    }
}
