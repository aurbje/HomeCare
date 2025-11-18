using System.Collections.Generic;
using HomeCare.Models;

namespace HomeCare.ViewModels.Caregiver
{
    public class CaregiverDashboardViewModel
    {
       public string CaregiverName { get; set; } = string.Empty;

        public List<CareTask> Tasks { get; set; }
        public List<Visit> TodayVisits { get; set; }

        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int UrgentTasks { get; set; }

        public CaregiverDashboardViewModel()
        {
            Tasks = new List<CareTask>();
            TodayVisits = new List<Visit>();
        }
    }
}
