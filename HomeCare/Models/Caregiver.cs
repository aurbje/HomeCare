using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HomeCare.Models
{
    public class Caregiver : IdentityUser
    {
        public string FullName { get; set; }

        // connection to the caregivers and visits
        public List<Task> Tasks { get; set; }
        public List<Visit> Visits { get; set; }
    }
}
