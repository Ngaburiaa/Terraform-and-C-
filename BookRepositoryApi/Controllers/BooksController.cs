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
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Book>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<Book>> GetAll()
    {
        return Ok(_bookService.GetAll());
    }

    [HttpGet(ApiRoutes.Books.ById)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Book> GetById([FromRoute] int id)
    {
        var book = _bookService.GetById(id);
        if (book is null)
        {
            return NotFound(new { message = "Book not found" });
        }

        return Ok(book);
    }

    [HttpPost(ApiRoutes.Books.Root)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status201Created)]
    public ActionResult<Book> Create([FromBody] CreateBookRequest request)
    {
        var created = _bookService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut(ApiRoutes.Books.ById)]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update([FromRoute] int id, [FromBody] UpdateBookRequest request)
    {
        var updated = _bookService.Update(id, request);
        if (!updated)
        {
            return NotFound(new { message = "Book not found" });
        }

        return NoContent();
    }

    [HttpDelete(ApiRoutes.Books.ById)]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] int id)
    {
        var deleted = _bookService.Delete(id);
        if (!deleted)
        {
            return NotFound(new { message = "Book not found" });
        }

        return NoContent();
    }
}
