using System;

namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required DateTime DateTime { get; set; }
        public required string ServiceType { get; set; }
        public string? Notes { get; set; }
    }
}
