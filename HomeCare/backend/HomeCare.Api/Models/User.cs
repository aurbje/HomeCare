using HomeCare.Api.Enums;

namespace HomeCare.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        // Personopplysninger
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Autentisering
        public string PasswordHash { get; set; } = string.Empty;

        // Kontaktinformasjon
        public string TlfNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Roller og tilgang (lagres som string i DB for kompatibilitet med ASP.NET Authorization)
        public string Role { get; set; } = UserRoleExtensions.Roles.User;

        /// <summary>
        /// Henter rollen som enum for enklere logikk
        /// </summary>
        public UserRole? RoleEnum => Role.ToUserRole();
    }
}
