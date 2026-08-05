using ManagementSystem.Api.Models.Entities;

namespace ManagementSystem.Api.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetRolesByIdAsync(int id);
    Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds);
    Task<Role> AddAsync(Role role);
}
