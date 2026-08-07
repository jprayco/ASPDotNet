using AutoMapper;
using ManagementSystem.Api.Common;
using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;
using ManagementSystem.Api.Models.Entities;
using ManagementSystem.Api.Repositories.Interfaces;
using ManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ManagementSystem.Api.Services.Implementation;

public class UserService : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");
        }

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        if (request.RoleIds.Count > 0)
        {
            var roles = await _roleRepository.GetRolesByIdsAsync(request.RoleIds);
            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }
        }

        var createdUser = await _userRepository.AddAsync(user);

        return _mapper.Map<UserResponse>(createdUser);
    }

    public async Task<PagedResult<UserResponse>> GetPagedAllUsersAsync(int PageNumber, int PageSize, CancellationToken ct = default)
    {
        var paged = await _userRepository.GetAllUsersPagedAsync(PageNumber, PageSize, ct);

        return new PagedResult<UserResponse>
        {
            Data = _mapper.Map<List<UserResponse>>(paged.Data),
            PageNumber = paged.PageNumber,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }
}
