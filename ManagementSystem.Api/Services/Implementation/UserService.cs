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
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
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
