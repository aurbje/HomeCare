using HomeCare.Api.Enums;

namespace HomeCare.Api.Models
{
    // representing a user in the system
    public class User
    {
        public int Id { get; set; }

        // personal information
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // authentication
        public string PasswordHash { get; set; } = string.Empty;

        // contact information
        public string TlfNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // user role with default as 'User'
        public string Role { get; set; } = UserRoleExtensions.Roles.User;

        //  computed property to get the UserRole enum from the Role string
        public UserRole? RoleEnum => Role.ToUserRole();
    }
}
