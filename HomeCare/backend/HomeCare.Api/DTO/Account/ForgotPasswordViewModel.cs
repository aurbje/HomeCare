using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    // ViewModel for forgot password
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-postadresse er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        public string Email { get; set; } = string.Empty;
    }
}