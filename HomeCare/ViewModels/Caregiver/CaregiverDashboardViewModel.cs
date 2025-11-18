using System.Collections.Generic;
using HomeCare.Models;

namespace HomeCare.ViewModels.Caregiver
{
    public class CaregiverDashboardViewModel // ViewModel for caregiver dashboard
    {
       public string CaregiverName { get; set; } = string.Empty;

        public List<CareTask> Tasks { get; set; } // cargivers tasks
        public List<Visit> TodayVisits { get; set; } // visits scheduled for today

        public int CompletedTasks { get; set; } // tasks
        public int PendingTasks { get; set; }
        public int UrgentTasks { get; set; }

        public CaregiverDashboardViewModel() // constructor initializing lists
        {
            Tasks = new List<CareTask>();
            TodayVisits = new List<Visit>();
        }
    }
}
