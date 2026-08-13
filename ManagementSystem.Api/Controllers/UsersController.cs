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

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "No refresh token provided." });

        var result = await _userService.RefreshAsync(refreshToken);

        SetRefreshTokenCookie(result.RefreshToken);

        return Ok(new { accessToken = result.AccessToken, refreshToken = result.RefreshToken,expiresAtUtc = result.ExpiresAtUtc });
    }

    [HttpGet("me")]
    public IActionResult Me() => Ok(new { User.Identity!.Name, Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value) });

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")] // only Admin role
    public IActionResult Delete(Guid id) => NoContent(); // wire up to a delete service call

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,           // needs HTTPS — see note below
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/users/refresh"
        });
    }
}
