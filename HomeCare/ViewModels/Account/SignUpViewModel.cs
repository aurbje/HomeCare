using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Fullt navn må fylles ut")]
        [Display(Name = "Fullt navn")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-post må fylles ut")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord må fylles ut")]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bekreft passord må fylles ut")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
        [Display(Name = "Bekreft passord")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
