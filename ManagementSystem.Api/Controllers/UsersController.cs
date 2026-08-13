using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams pagination)
    {
        var result = await _userService.GetPagedAllUsersAsync(pagination.PageNumber, pagination.PageSize);
        return Ok(result);
    }

    [HttpGet("me")]
    public IActionResult Me() => Ok(new { User.Identity!.Name, Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value) });

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")] // only Admin role
    public IActionResult Delete(Guid id) => NoContent(); // wire up to a delete service call
}
