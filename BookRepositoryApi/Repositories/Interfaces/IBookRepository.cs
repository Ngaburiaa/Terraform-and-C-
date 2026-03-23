using BookRepositoryApi.Models;

namespace BookRepositoryApi.Repositories.Interfaces;

// Defines persistence operations for books.
public interface IBookRepository
{
    // Retrieves all books.
    Task<IReadOnlyCollection<BookResponse>> GetAllAsync(CancellationToken cancellationToken);

    // Retrieves a book by identifier.
    Task<BookResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    // Retrieves a tracked book entity for modification.
    Task<Book?> GetForUpdateAsync(int id, CancellationToken cancellationToken);

    // Adds a new book and returns its response projection.
    Task<BookResponse> AddAsync(Book book, CancellationToken cancellationToken);

    // Saves pending updates to a book and returns the updated projection.
    Task<BookResponse> UpdateAsync(Book book, CancellationToken cancellationToken);

    // Deletes a tracked book entity.
    Task DeleteAsync(Book book, CancellationToken cancellationToken);
}

