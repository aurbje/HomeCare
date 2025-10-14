using System;

namespace HomeCare.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string ServiceType { get; set; }
        public string Notes { get; set; }
    }
}
