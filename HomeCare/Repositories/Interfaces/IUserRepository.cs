using HomeCare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeCare.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(int id); // id
        Task<User?> GetByEmailAsync(string email); // email
        Task<bool> EmailExistsAsync(string email); // email exists
        
        Task<IEnumerable<User>> GetAllUsersAsync(); // all users
        Task AddAsync(User user); // add new user
        Task UpdateUserAsync(User user); // update user
        Task DeleteUserAsync(int id); // delete user by id

        Task SaveChangesAsync(); // save changes to db
    }
}
