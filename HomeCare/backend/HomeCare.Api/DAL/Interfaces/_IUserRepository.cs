/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in IUserRepository.cs
 * This file kept for reference purposes
 * ============================================================

using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // Handles all user-related data access
    public interface IUserRepository
    {
        // get user by id
        Task<User?> GetUserByIdAsync(int id);

        // get user by email
        Task<User?> GetByEmailAsync(string email);

        // check if email exists
        Task<bool> EmailExistsAsync(string email);

        // get all users
        Task<IEnumerable<User>> GetAllUsersAsync();

        // add new user
        Task AddAsync(User user);

        // update user
        Task<bool> UpdateUserAsync(User user);

        // delete user by id
        Task<bool> DeleteUserAsync(int id);

        // save changes to db
        Task SaveChangesAsync();
    }
}

*/