namespace HomeCare.Api.Enums
{
    /// <summary>
    /// Brukerroller i systemet
    /// </summary>
    public enum UserRole
    {
        User,
        Caregiver,
        Admin
    }

    /// <summary>
    /// Hjelpemetoder for UserRole enum
    /// </summary>
    public static class UserRoleExtensions
    {
        /// <summary>
        /// Konverterer enum til string for database/authorization
        /// </summary>
        public static string ToRoleString(this UserRole role) => role.ToString();

        /// <summary>
        /// Konverterer string til enum
        /// </summary>
        public static UserRole? ToUserRole(this string? roleString)
        {
            if (string.IsNullOrEmpty(roleString)) return null;
            return Enum.TryParse<UserRole>(roleString, ignoreCase: true, out var role) ? role : null;
        }

        /// <summary>
        /// String-konstanter for bruk med [Authorize(Roles = "...")]
        /// </summary>
        public static class Roles
        {
            public const string User = nameof(UserRole.User);
            public const string Caregiver = nameof(UserRole.Caregiver);
            public const string Admin = nameof(UserRole.Admin);
        }
    }
}
