using System;

namespace HomeCare.Models
{
    public class Visit
    {
        public int VisitId { get; set; }

        // when and how long
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // connection to the user/patient
        public int UserId { get; set; }
        public User User { get; set; }

        // who arrives
        public string CaregiverId { get; set; }
        public Caregiver? Caregiver { get; set; }

        // what the visit is for
        public string Purpose { get; set; }

        // the patients adress 
        public string Address { get; set; }

        // status
        public bool IsCompleted { get; set; }
    }
}
