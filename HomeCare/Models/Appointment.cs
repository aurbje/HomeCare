using System;

namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; } // AvailableDate.Date + TimeSlot.Slot
        public string? Notes { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!; // never null

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // never null

    }
}
