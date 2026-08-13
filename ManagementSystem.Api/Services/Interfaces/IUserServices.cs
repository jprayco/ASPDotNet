using ManagementSystem.Api.Common;
using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;

namespace ManagementSystem.Api.Services.Interfaces;

public interface IUserServices
{
        Task<PagedResult<UserResponse>> GetPagedAllUsersAsync(int PageNumber, int PageSize, CancellationToken ct = default);
}
