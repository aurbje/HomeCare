using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models
{
    // category used for appointments and booking
    public class Category
    {
        public int Id { get; set; }

        // name of the category (like "Medication", "Cooking", "Other")
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}