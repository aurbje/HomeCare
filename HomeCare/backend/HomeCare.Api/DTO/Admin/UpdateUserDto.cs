namespace HomeCare.Api.DTO.Admin
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? TlfNumber { get; set; }
        public string? Address { get; set; }
    }
}