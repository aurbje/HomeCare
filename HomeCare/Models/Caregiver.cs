using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HomeCare.Models
{
    public class Caregiver : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // connection to the caregiver’s tasks and visits
        public List<CareTask> Tasks { get; set; } = new();
        public List<Visit> Visits { get; set; } = new();
    }
}

