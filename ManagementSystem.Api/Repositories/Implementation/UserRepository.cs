using ManagementSystem.Api.Common;
using ManagementSystem.Api.Data;
using ManagementSystem.Api.Models.Entities;
using ManagementSystem.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManagementSystem.Api.Repositories.Implementation;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    public async Task<PagedResult<User>> GetAllUsersPagedAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Users
                            .AsNoTracking()
                            .Include(u => u.Roles)
                            .OrderBy(u => u.Id);

        var totalCount = await query.CountAsync(ct);

        var items = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);

        return new PagedResult<User>
        {
            Data = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public Task<User?> GetByUsernameAsync(string username)=>
        _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<User?> GetByRefreshTokenAsync(string refreshTokenHash)=>
        _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenHash);

    public Task<User?> GetByIdAsync(Guid id)=>
        _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<bool> ExistsAsync(string username, string email) =>
        _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
}
