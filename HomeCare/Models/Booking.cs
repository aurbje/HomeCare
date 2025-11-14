using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Dato må fylles ut")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Tid må fylles ut")]
        public string? Time { get; set; }

        [Required(ErrorMessage = "Tjenestetype må fylles ut")]
        public string? ServiceType { get; set; }

        public string? Notes { get; set; }

        // User
        public int? ClientId { get; set; }
        public User? Client { get; set; }

        // Personnel
        public int? PersonnelId { get; set; }
        public User? Personnel { get; set; }
    }
}
