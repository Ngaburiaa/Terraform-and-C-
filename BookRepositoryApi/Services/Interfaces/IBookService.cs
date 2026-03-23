using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Models;

namespace BookRepositoryApi.Services.Interfaces;

public interface IBookService
{
    // Retrieves all books.
    Task<Result<IReadOnlyCollection<BookResponse>>> GetAllAsync(CancellationToken cancellationToken);

    // Retrieves a book by identifier.
    Task<Result<BookResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);

    // Creates a new book for the specified author.
    Task<Result<BookResponse>> CreateAsync(CreateBookRequest request, int authorId, CancellationToken cancellationToken);

    // Updates an existing book.
    Task<Result<BookResponse>> UpdateAsync(int id, UpdateBookRequest request, int requestingUserId, bool isAdmin, CancellationToken cancellationToken);

    // Deletes an existing book.
    Task<Result<OperationResult>> DeleteAsync(int id, int requestingUserId, bool isAdmin, CancellationToken cancellationToken);
}

