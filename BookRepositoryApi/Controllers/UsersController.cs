using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Routes;
using BookRepositoryApi.Security;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    // Initializes a new instance of the UsersController class.
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // Retrieves all users for administrators.
    [Authorize(Roles = Roles.Admin)]
    [HttpGet(ApiRoutes.Users.Root)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<UserResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<UserResponse>>.FromResult(result));
    }

    // Retrieves a user by identifier for administrators.
    [Authorize(Roles = Roles.Admin)]
    [HttpGet(ApiRoutes.Users.ById)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<UserResponse>.Failure(result.Message));
        }

        return Ok(ApiResponse<UserResponse>.FromResult(result));
    }

    // Deletes a user by identifier for administrators.
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete(ApiRoutes.Users.ById)]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OperationResult>>> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<OperationResult>.Failure(result.Message));
        }

        return Ok(ApiResponse<OperationResult>.FromResult(result));
    }
}

