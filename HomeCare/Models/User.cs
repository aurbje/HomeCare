namespace HomeCare.Models
{
    public class User
    {
        public int Id { get; set; }

        // person info
        public string FullName { get; set; } = string.Empty;     // combines name from de other version 
        public string UserName { get; set; } = string.Empty;     // username for login
        public string Email { get; set; } = string.Empty;

        // autentification
        public string PasswordHash { get; set; } = string.Empty; // safer save than just password

        //contact information
        public string TlfNumber { get; set; } = string.Empty;    // keep as a string for flexibility (countrycodes etc.) 
        public string Address { get; set; } = string.Empty;

        // roles and access
        public string Role { get; set; } = "user";               // standardrole for new users
    }
}
