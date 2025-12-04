using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Admin
{
    // DTO for updating user information
    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty; // user's full name

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // user's email address

        public string? TlfNumber { get; set; } // user's telephone number
        public string? Address { get; set; } // user's address
    }
}