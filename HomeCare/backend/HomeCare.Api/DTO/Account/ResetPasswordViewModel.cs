using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    // ViewModel used for resetting a user's password
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty; // token for password reset verification

        [Required]
        public string Email { get; set; } = string.Empty; // user's email address

        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passordet må være minst 6 tegn")] // password must be at least 6 characters
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")] //  passwords do not match
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}