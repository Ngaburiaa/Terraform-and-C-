using BookRepositoryApi.Data;
using BookRepositoryApi.Models;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Services;

public sealed class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<BookResponse> GetAll()
    {
        return _context.Books
            .AsNoTracking()
            .Include(b => b.AuthorUser)
            .OrderBy(b => b.Title)
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Isbn = b.Isbn,
                YearPublished = b.YearPublished,
                AuthorId = b.AuthorId,
                AuthorUsername = b.AuthorUser != null ? b.AuthorUser.Username : b.Author
            })
            .ToList();
    }

    public BookResponse? GetById(int id)
    {
        return _context.Books
            .AsNoTracking()
            .Include(b => b.AuthorUser)
            .Where(b => b.Id == id)
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                Isbn = b.Isbn,
                YearPublished = b.YearPublished,
                AuthorId = b.AuthorId,
                AuthorUsername = b.AuthorUser != null ? b.AuthorUser.Username : b.Author
            })
            .FirstOrDefault();
    }

    public BookResponse Create(CreateBookRequest request, int authorId)
    {
        var username = _context.Users
                              .Where(u => u.Id == authorId)
                              .Select(u => u.Username)
                              .FirstOrDefault() ?? string.Empty;

        var book = new Book
        {
            // let the database generate the id
            Title = request.Title.Trim(),
            AuthorId = authorId,
            // duplicate the username for convenience; the controller will pass this in if it has access to the name claim
            Author = username,
            Isbn = request.Isbn.Trim(),
            YearPublished = request.YearPublished
        };

        _context.Books.Add(book);
        _context.SaveChanges();

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            YearPublished = book.YearPublished,
            AuthorId = book.AuthorId,
            AuthorUsername = username
        };
    }

    public bool Update(int id, UpdateBookRequest request)
    {
        var book = _context.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return false;

        book.Title = request.Title.Trim();
        book.Isbn = request.Isbn.Trim();
        book.YearPublished = request.YearPublished;

        _context.SaveChanges();

        return true;
    }

    public bool Delete(int id)
    {
        var book = _context.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
            return false;

        _context.Books.Remove(book);
        _context.SaveChanges();

        return true;
    }
}
