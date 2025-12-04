namespace HomeCare.Api.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? TlfNumber { get; set; }
        public string? Address { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
