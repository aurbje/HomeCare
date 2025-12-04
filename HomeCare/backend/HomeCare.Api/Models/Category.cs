using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Models
{
    // represents a category of home care services
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}