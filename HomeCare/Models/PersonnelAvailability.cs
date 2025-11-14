
using System.ComponentModel.DataAnnotations.Schema;
using HomeCare.Models;

namespace HomeCare.Models
{

    public class PersonnelAvailability
    {
        public int Id { get; set; }

        [ForeignKey("Personnel")]
        public int PersonnelId { get; set; }
        public DateTime Date { get; set; }

        public User Personnel { get; set; }
        // right now working hours are fixed (8:00-15:00)
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);  // 08:00
        public TimeSpan EndTime { get; set; } = new TimeSpan(15, 0, 0);   // 15:00

    }
}
