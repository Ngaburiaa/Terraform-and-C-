using BookRepositoryApi.Data;
using BookRepositoryApi.Models;
using BookRepositoryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Repositories;

// Handles database access for books.
public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    // Initializes a new instance of the BookRepository class.
    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

public async Task<IReadOnlyCollection<BookResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Books
            .AsNoTracking()
            .Include(book => book.AuthorUser)
            .OrderBy(book => book.Title)
            .Select(book => new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                YearPublished = book.YearPublished,
                AuthorId = book.AuthorId,
                AuthorUsername = book.AuthorUser != null ? book.AuthorUser.Username : book.Author
            })
            .ToListAsync(cancellationToken);

public Task<BookResponse?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _context.Books
            .AsNoTracking()
            .Include(book => book.AuthorUser)
            .Where(book => book.Id == id)
            .Select(book => new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                YearPublished = book.YearPublished,
                AuthorId = book.AuthorId,
                AuthorUsername = book.AuthorUser != null ? book.AuthorUser.Username : book.Author
            })
            .FirstOrDefaultAsync(cancellationToken);

public Task<Book?> GetForUpdateAsync(int id, CancellationToken cancellationToken) =>
        _context.Books.FirstOrDefaultAsync(book => book.Id == id, cancellationToken);

public async Task<BookResponse> AddAsync(Book book, CancellationToken cancellationToken)
    {
        await _context.Books.AddAsync(book, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Books
            .AsNoTracking()
            .Include(savedBook => savedBook.AuthorUser)
            .Where(savedBook => savedBook.Id == book.Id)
            .Select(savedBook => new BookResponse
            {
                Id = savedBook.Id,
                Title = savedBook.Title,
                Isbn = savedBook.Isbn,
                YearPublished = savedBook.YearPublished,
                AuthorId = savedBook.AuthorId,
                AuthorUsername = savedBook.AuthorUser != null ? savedBook.AuthorUser.Username : savedBook.Author
            })
            .SingleAsync(cancellationToken);
    }

public async Task<BookResponse> UpdateAsync(Book book, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);

        return await _context.Books
            .AsNoTracking()
            .Include(savedBook => savedBook.AuthorUser)
            .Where(savedBook => savedBook.Id == book.Id)
            .Select(savedBook => new BookResponse
            {
                Id = savedBook.Id,
                Title = savedBook.Title,
                Isbn = savedBook.Isbn,
                YearPublished = savedBook.YearPublished,
                AuthorId = savedBook.AuthorId,
                AuthorUsername = savedBook.AuthorUser != null ? savedBook.AuthorUser.Username : savedBook.Author
            })
            .SingleAsync(cancellationToken);
    }

public async Task DeleteAsync(Book book, CancellationToken cancellationToken)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

