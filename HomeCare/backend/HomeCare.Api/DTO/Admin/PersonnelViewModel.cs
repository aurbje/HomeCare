namespace HomeCare.ViewModels.Admin
{
    public class PersonnelViewModel
    {
        //Userdata from User table
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TlfNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        
        // PersonnelId from PersonnelAppointment (if exists)
        public int PersonnelId { get; set; }
    }
}
