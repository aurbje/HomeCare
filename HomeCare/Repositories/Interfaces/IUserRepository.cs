using HomeCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);       // NEW
        Task<bool> EmailExistsAsync(string email);       // NEW
        
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task AddAsync(User user);                        // NEW (instead of AddUserAsync)
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);

        Task SaveChangesAsync();                         // NEW
    }
}
