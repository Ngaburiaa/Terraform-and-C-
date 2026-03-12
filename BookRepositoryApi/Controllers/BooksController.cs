using BookRepositoryApi.Models;
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

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet(ApiRoutes.Books.Root)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author},{Roles.Reader}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<BookResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<BookResponse>> GetAll()
    {
        return Ok(_bookService.GetAll());
    }

    [HttpGet(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author},{Roles.Reader}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BookResponse> GetById([FromRoute] int id)
    {
        var book = _bookService.GetById(id);
        if (book is null)
        {
            return NotFound(new { message = "Book not found" });
        }

        return Ok(book);
    }

    [HttpPost(ApiRoutes.Books.Root)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
    public ActionResult<BookResponse> Create([FromBody] CreateBookRequest request)
    {
        // take the current user id from the JWT claims
        if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Forbid();
        }

        var created = _bookService.Create(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateBookRequest request)
    {
        var book = _bookService.GetById(id);
        if (book is null)
        {
            return NotFound(new { message = "Book not found" });
        }

        var isAdmin = User.IsInRole(Roles.Admin);
        if (!isAdmin)
        {
            if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var userId) || book.AuthorId != userId)
            {
                return Forbid();
            }
        }

        var updated = _bookService.Update(id, request);
        if (!updated)
        {
            return NotFound(new { message = "Book not found" });
        }

        return NoContent();
    }

    [HttpDelete(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Author}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Delete([FromRoute] int id)
    {
        var book = _bookService.GetById(id);
        if (book is null)
        {
            return NotFound(new { message = "Book not found" });
        }

        var isAdmin = User.IsInRole(Roles.Admin);
        if (!isAdmin)
        {
            if (!int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var userId) || book.AuthorId != userId)
            {
                return Forbid();
            }
        }

        var deleted = _bookService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = "Book not found" });
        }

        return NoContent();
    }
}
