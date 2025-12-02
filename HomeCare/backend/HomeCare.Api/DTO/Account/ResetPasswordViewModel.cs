using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passordet må være minst 6 tegn")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}