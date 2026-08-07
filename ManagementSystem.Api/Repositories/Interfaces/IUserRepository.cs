using ManagementSystem.Api.Common;
using ManagementSystem.Api.Models.Entities;

namespace ManagementSystem.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<PagedResult<User>> GetAllUsersPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default);
}
