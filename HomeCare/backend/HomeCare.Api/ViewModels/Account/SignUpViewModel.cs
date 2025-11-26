using System.ComponentModel.DataAnnotations;

namespace HomeCare.ViewModels.Account
{
    public class SignUpViewModel // ViewModel for user sign-up
    {
        [Required(ErrorMessage = "Fullt navn må fylles ut")] // error for full name
        [Display(Name = "Fullt navn")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-post må fylles ut")] // error for email
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonnummer må fylles ut")] // error for phone number
        [Display(Name = "Telefonnummer")]
        public string TlfNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresse må fylles ut")] // error for address
        [Display(Name = "Adresse")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passord må fylles ut")] // error for password
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bekreft passord må fylles ut")] // error for confirm password
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
        [Display(Name = "Bekreft passord")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
