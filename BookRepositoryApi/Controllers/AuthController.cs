using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Routes;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // Initializes a new instance of the AuthController class.
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Authenticates a user with username and password credentials.
    [HttpPost(ApiRoutes.Auth.Login)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (!result.Success)
        {
            return Unauthorized(ApiResponse<LoginResponse>.Failure(result.Message));
        }

        return Ok(ApiResponse<LoginResponse>.FromResult(result));
    }

    // Registers a new reader account.
    [HttpPost(ApiRoutes.Auth.Register)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        if (!result.Success)
        {
            return Conflict(ApiResponse<LoginResponse>.Failure(result.Message));
        }

        return Created(ApiRoutes.Auth.Login, ApiResponse<LoginResponse>.FromResult(result));
    }
}

