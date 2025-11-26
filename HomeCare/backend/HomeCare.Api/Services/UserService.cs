using HomeCare.Api.Models;
using HomeCare.Api.Repositories.Interfaces;

namespace HomeCare.Api.Services;

public class UserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public Task<ApplicationUser?> GetByEmailAsync(string email)
        => _repo.GetByEmailAsync(email);

    public Task<ApplicationUser?> GetByIdAsync(string id)
        => _repo.GetByIdAsync(id);
}
