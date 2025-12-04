namespace HomeCare.Api.Enums
{
    /// Brukerroller i systemet
    public enum UserRole
    {
        User,
        Caregiver,
        Admin
    }

    /// Hjelpemetoder for UserRole enum
    public static class UserRoleExtensions
    {
        public static string ToRoleString(this UserRole role) => role.ToString();

        /// Konverterer string til enum
        public static UserRole? ToUserRole(this string? roleString)
        {
            if (string.IsNullOrEmpty(roleString)) return null;
            return Enum.TryParse<UserRole>(roleString, ignoreCase: true, out var role) ? role : null;
        }

        /// String-konstanter for bruk med [Authorize(Roles = "...")]
        public static class Roles
        {
            public const string User = nameof(UserRole.User);
            public const string Caregiver = nameof(UserRole.Caregiver);
            public const string Admin = nameof(UserRole.Admin);
        }
    }
}
