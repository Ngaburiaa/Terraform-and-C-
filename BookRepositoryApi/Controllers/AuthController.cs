using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Routes;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        var result = _authService.Login(request);
        if (result is null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        return Ok(result);
    }

    [HttpPost(ApiRoutes.Auth.Register)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<LoginResponse> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username and password are required" });
        }

        var result = _authService.Register(request);
        if (result is null)
        {
            return Conflict(new { message = "Username already exists" });
        }

        return Created(ApiRoutes.Auth.Login, result);
    }
}
