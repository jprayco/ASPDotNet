using ManagementSystem.Api.Models.Entities;

namespace ManagementSystem.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllUsersAsync();
    Task<User> AddAsync(User user);
}
