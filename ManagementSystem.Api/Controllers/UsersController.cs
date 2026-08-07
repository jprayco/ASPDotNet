using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserServices _userService;

    public UsersController(IUserServices userServices)
    {
        _userService = userServices;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(CreateUser), new {id = result.Id, result});
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination)
    {
        var result = await _userService.GetPagedAllUsersAsync(pagination.PageNumber, pagination.PageSize);
        return Ok(result);
    }
}
