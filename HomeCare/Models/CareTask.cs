using System;

namespace HomeCare.Models
{
    public class CareTask
    {
        public int CareTaskId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueTime { get; set; }

        // connected to the patient
        public int UserId { get; set; }
        public User User { get; set; }

        // connected to the caregiver
        public string CaregiverId { get; set; }
        public Caregiver Caregiver { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsUrgent { get; set; }
    }
}
