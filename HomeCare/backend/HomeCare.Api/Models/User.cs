using HomeCare.Api.Enums;

namespace HomeCare.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string TlfNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string Role { get; set; } = UserRoleExtensions.Roles.User;

        public UserRole? RoleEnum => Role.ToUserRole();
    }
}
