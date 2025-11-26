using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    public class SignInViewModel // ViewModel for user sign-in
    {
        [Required(ErrorMessage = "E-post må fylles ut")] // error for email
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord må fylles ut")] // error for password
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Husk meg")] // remember me option
        public bool RememberMe { get; set; } = false;
    }
}
