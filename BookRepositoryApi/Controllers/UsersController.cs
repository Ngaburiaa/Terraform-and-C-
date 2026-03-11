using BookRepositoryApi.Models.Auth;
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

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet(ApiRoutes.Users.Root)]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<UserResponse>> GetAll()
    {
        return Ok(_userService.GetAll());
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpGet(ApiRoutes.Users.ById)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserResponse> GetById(int id)
    {
        var user = _userService.GetById(id);
        if (user is null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
    }
}
