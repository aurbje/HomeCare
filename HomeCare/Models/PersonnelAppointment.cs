using HomeCare.Models;
namespace HomeCare.Models
{
    public class PersonnelAppointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }

        public int ClientId { get; set; }
        public User Client { get; set; }

        public int PersonnelId { get; set; }
        public User Personnel { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Notes { get; set; }
    }
}