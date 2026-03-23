using System.Security.Claims;
using BookRepositoryApi.Constants;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Routes;
using BookRepositoryApi.Security;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookRepositoryApi.Controllers;

[ApiController]
[Authorize]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    // Initializes a new instance of the BooksController class.
    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // Retrieves all books visible to authenticated users.
    [HttpGet(ApiRoutes.Books.Root)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author},{Roles.Reader}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyCollection<BookResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<BookResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<BookResponse>>.FromResult(result));
    }

    // Retrieves a single book by identifier.
    [HttpGet(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author},{Roles.Reader}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(ApiResponse<BookResponse>.Failure(result.Message));
        }

        return Ok(ApiResponse<BookResponse>.FromResult(result));
    }

    // Creates a new book owned by the authenticated user.
    [HttpPost(ApiRoutes.Books.Root)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Create(
        [FromBody] CreateBookRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiResponse<BookResponse>.Failure(ResponseMessages.Forbidden));
        }

        var result = await _bookService.CreateAsync(request, userId, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(ApiResponse<BookResponse>.Failure(result.Message));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<BookResponse>.FromResult(result));
    }

    // Updates an existing book when the caller is an administrator or owner.
    [HttpPut(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Update(
        [FromRoute] int id,
        [FromBody] UpdateBookRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiResponse<BookResponse>.Failure(ResponseMessages.Forbidden));
        }

        var result = await _bookService.UpdateAsync(id, request, userId, User.IsInRole(Roles.Admin), cancellationToken);
        if (!result.Success && result.Message == ResponseMessages.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<BookResponse>.Failure(result.Message));
        }

        if (!result.Success)
        {
            return NotFound(ApiResponse<BookResponse>.Failure(result.Message));
        }

        return Ok(ApiResponse<BookResponse>.FromResult(result));
    }

    // Deletes an existing book when the caller is an administrator or owner.
    [HttpDelete(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<OperationResult>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OperationResult>>> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized(ApiResponse<OperationResult>.Failure(ResponseMessages.Forbidden));
        }

        var result = await _bookService.DeleteAsync(id, userId, User.IsInRole(Roles.Admin), cancellationToken);
        if (!result.Success && result.Message == ResponseMessages.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<OperationResult>.Failure(result.Message));
        }

        if (!result.Success)
        {
            return NotFound(ApiResponse<OperationResult>.Failure(result.Message));
        }

        return Ok(ApiResponse<OperationResult>.FromResult(result));
    }

    // Attempts to read the current authenticated user identifier.
    private bool TryGetCurrentUserId(out int userId) =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}

