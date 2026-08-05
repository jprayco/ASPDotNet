using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;

namespace ManagementSystem.Api.Services.Interfaces;

public interface IUserServices
{
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<List<UserResponse>> GetAllUsersAsync();
}
