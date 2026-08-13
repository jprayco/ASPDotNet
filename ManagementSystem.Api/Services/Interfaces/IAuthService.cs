using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;

namespace ManagementSystem.Api.Services.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(CreateUserRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshAsync(string refreshToken);

}
