namespace HomeCare.Api.Enums
{
    // userroles in the system
    public enum UserRole
    {
        User,
        Caregiver,
        Admin
    }

    // extension methods for UserRole enum
    public static class UserRoleExtensions
    {
  // converts enum to string
        public static string ToRoleString(this UserRole role) => role.ToString();

       // converts string to enum, returns null if invalid
        public static UserRole? ToUserRole(this string? roleString)
        {
            if (string.IsNullOrEmpty(roleString)) return null;
            return Enum.TryParse<UserRole>(roleString, ignoreCase: true, out var role) ? role : null;
        }

        // constants for role strings
        public static class Roles
        {
            public const string User = nameof(UserRole.User);
            public const string Caregiver = nameof(UserRole.Caregiver);
            public const string Admin = nameof(UserRole.Admin);
        }
    }
}
