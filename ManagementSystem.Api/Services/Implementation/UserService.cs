using AutoMapper;
using System.Security.Claims;
using ManagementSystem.Api.Common;
using ManagementSystem.Api.Configuration;
using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;
using ManagementSystem.Api.Models.Entities;
using ManagementSystem.Api.Repositories.Interfaces;
using ManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using ManagementSystem.Api.Common.Security;

namespace ManagementSystem.Api.Services.Implementation;

public class UserService : IUserServices
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly JwtSettings _jwt;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IMapper mapper, IPasswordHasher<User> passwordHasher,IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwt = jwtOptions.Value;
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

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        var (accessToken, expiresAtUtc) = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = TokenHasher.Hash(refreshToken);
        user.RefreshTokenExpiresUtc = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);
        user.SessionCreatedUtc = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAtUtc = expiresAtUtc };
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var incomingHash = TokenHasher.Hash(refreshToken);

        var user = await _userRepository.GetByRefreshTokenAsync(incomingHash);
        if (user is null || user.RefreshTokenExpiresUtc < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        if (user.SessionCreatedUtc is null || user.SessionCreatedUtc.Value.AddDays(_jwt.MaxSessionDays) < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Session too old, please log in again.");

        var (accessToken, expiresAtUtc) = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = TokenHasher.Hash(newRefreshToken);
        user.RefreshTokenExpiresUtc = DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays);
        await _userRepository.UpdateAsync(user);

        // Return the RAW token to the client — only the hash lives in the DB
        return new AuthResponse { AccessToken = accessToken, RefreshToken = newRefreshToken, ExpiresAtUtc = expiresAtUtc };
    }

    public async Task<UserResponse> RegisterAsync(CreateUserRequest request)
    {
        if (await _userRepository.ExistsAsync(request.Username, request.Email))
            throw new InvalidOperationException("Username or email already in use.");

        var user = _mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        if (request.RoleIds.Count > 0)
        {
            var roles = await _roleRepository.GetRolesByIdsAsync(request.RoleIds);
            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }
        }

        await _userRepository.AddAsync(user);

        return _mapper.Map<UserResponse>(user);
    }

    private (string token, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);

        var token = new JwtSecurityToken(_jwt.Issuer, _jwt.Audience, claims, expires: expires, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
