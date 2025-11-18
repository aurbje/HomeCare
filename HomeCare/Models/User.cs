using System.Collections.Generic;

namespace HomeCare.Models
{
    public class User
    {
        public int Id { get; set; }

        // person info
        public string FullName { get; set; } = string.Empty; // e.g. "Name Surname"
        public string UserName { get; set; } = string.Empty; // e.g. "name.surname"
        public string Email { get; set; } = string.Empty; // email address

        // authentication
        public string PasswordHash { get; set; } = string.Empty; // hashed password

        // contact info
        public string TlfNumber { get; set; } = string.Empty; // telephone number
        public string Address { get; set; } = string.Empty; // home address

        // roles and access
        public string Role { get; set; } = "user"; // e.g. "admin", "caregiver", "user"

        // relations
        public ICollection<Visit> Visits { get; set; } = new List<Visit>(); // visits associated with the user
        public ICollection<CareTask> CareTasks { get; set; } = new List<CareTask>(); // care tasks assigned to the user
    }
}
