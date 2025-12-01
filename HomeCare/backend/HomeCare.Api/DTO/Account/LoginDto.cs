using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Account
{
    // DTO used for user login requests from the React frontend
    public class LoginDto
    {
        [Required(ErrorMessage = "E-post må fylles ut")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord må fylles ut")]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Husk meg")]
        public bool RememberMe { get; set; } = false;
    }
}
