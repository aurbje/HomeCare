using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; } 
        [Required(ErrorMessage = "Dato og tid må fylles ut")]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "Tjenestetype må fylles ut")]
        public string ServiceType { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;
    }
}
