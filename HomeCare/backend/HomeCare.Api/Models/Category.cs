using Microsoft.EntityFrameworkCore;

namespace HomeCare.Api.Models
{
    /// <summary>
    /// Service category (e.g., Vask, Omsorg, Kjøkkenarbeid).
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}