namespace HomeCare.Models
{
    public class User
    {
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int TlfNumber { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}