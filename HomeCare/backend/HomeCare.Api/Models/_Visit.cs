using System;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.Models
{
    // a scheduled visit from a caregiver to a user
    public class Visit
    {
        public int VisitId { get; set; }

        // when the visit starts and ends
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // user (patient) connected to this visit
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // what the visit is about
        [Required]
        public string Purpose { get; set; } = string.Empty;

        // where the visit takes place
        [Required]
        public string Address { get; set; } = string.Empty;

        // if the visit has been completed
        public bool IsCompleted { get; set; }
    }
}