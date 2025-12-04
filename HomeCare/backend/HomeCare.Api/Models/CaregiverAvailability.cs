
using System.ComponentModel.DataAnnotations.Schema;
using HomeCare.Api.Models;

// represents the availability of a caregiver on a specific date
namespace HomeCare.Api.Models
{
    public class CaregiverAvailability
    {
        public int Id { get; set; }

        [ForeignKey("Caregiver")]
        public int CaregiverId { get; set; }
        public DateTime Date { get; set; }

        public User Caregiver { get; set; } = null!;
        // right now working hours are fixed (8:00-15:00)
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);  // 08:00
        public TimeSpan EndTime { get; set; } = new TimeSpan(15, 0, 0);   // 15:00
    }
}
