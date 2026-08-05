using ManagementSystem.Api.Data;
using ManagementSystem.Api.Models.Entities;
using ManagementSystem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Api.Repositories.Implementation;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetRolesByIdAsync(int id)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<List<Role>> GetRolesByIdsAsync(List<int> roleIds)
    {
        return await _context.Roles
            .Where(r => roleIds.Contains(r.Id))
            .ToListAsync();
    }
    public async Task<Role> AddAsync(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }
}
