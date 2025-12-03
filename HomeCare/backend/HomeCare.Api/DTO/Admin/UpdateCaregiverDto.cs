using System.ComponentModel.DataAnnotations;

namespace HomeCare.Api.DTO.Admin
{
    public class CaregiverUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? TlfNumber { get; set; }

        public string? Address { get; set; }
    }
}