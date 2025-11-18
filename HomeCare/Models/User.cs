using System.Collections.Generic;

namespace HomeCare.Models
{
    public class User
    {
        public int Id { get; set; }

        // person info
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // authentication
        public string PasswordHash { get; set; } = string.Empty;

        // contact info
        public string TlfNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // roles and access
        public string Role { get; set; } = "user";

        // relations
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public ICollection<CareTask> CareTasks { get; set; } = new List<CareTask>();
    }
}
