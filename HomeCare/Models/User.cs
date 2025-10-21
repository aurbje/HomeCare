namespace HomeCare.Models
{
    public class User
    {
        public int Id { get; set; }

        // Personopplysninger
        public string FullName { get; set; } = string.Empty;     // kombinerer Name fra den andre versjonen
        public string UserName { get; set; } = string.Empty;     // valgfritt brukernavn for innlogging
        public string Email { get; set; } = string.Empty;

        // Autentisering
        public string PasswordHash { get; set; } = string.Empty; // sikrere lagring enn ren Password

        // Kontaktinformasjon
        public string TlfNumber { get; set; } = string.Empty;    // beholdt som string for fleksibilitet (landkoder, mellomrom, etc.)
        public string Address { get; set; } = string.Empty;

        // Roller og tilgang
        public string Role { get; set; } = "user";               // standardrolle for nye brukere
    }
}
