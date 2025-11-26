using HomeCare.Api.Models;

namespace HomeCare.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
}
