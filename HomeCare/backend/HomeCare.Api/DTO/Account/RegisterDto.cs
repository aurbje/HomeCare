using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Account
{
    // DTO used for user registration (sign-up) 
    public class RegisterDto
    {
        [Required(ErrorMessage = "Fullt navn må fylles ut")]
        [Display(Name = "Fullt navn")]
        public string FullName { get; set; } = string.Empty; // user's full name

        [Required(ErrorMessage = "E-post må fylles ut")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse")]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; } = string.Empty; // user's email address

        [Required(ErrorMessage = "Telefonnummer må fylles ut")]
        [Display(Name = "Telefonnummer")]
        public string TlfNumber { get; set; } = string.Empty; // user's phone number

        [Required(ErrorMessage = "Adresse må fylles ut")]
        [Display(Name = "Adresse")]
        public string Address { get; set; } = string.Empty; // user's address

        [Required(ErrorMessage = "Passord må fylles ut")]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = string.Empty; // user's password

        [Required(ErrorMessage = "Bekreft passord må fylles ut")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene stemmer ikke overens")]
        [Display(Name = "Bekreft passord")]
        public string ConfirmPassword { get; set; } = string.Empty; // confirmation of the user's password
    }
}