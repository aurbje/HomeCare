using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Admin
{
    // DTO for updating a caregiver's information
    public class UpdateCaregiverDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty; // full name of the caregiver

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; // email address of the caregiver

        public string? TlfNumber { get; set; } // phone number of the caregiver (optional)

        public string? Address { get; set; } // address of the caregiver (optional)
    }
}