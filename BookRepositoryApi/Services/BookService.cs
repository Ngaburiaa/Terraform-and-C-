using BookRepositoryApi.Constants;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Repositories.Interfaces;
using BookRepositoryApi.Services.Interfaces;

namespace BookRepositoryApi.Services;

// Handles book-related business operations.
public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;

    // Initializes a new instance of the BookService class.
    public BookService(IBookRepository bookRepository, IUserRepository userRepository)
    {
        _bookRepository = bookRepository;
        _userRepository = userRepository;
    }

public async Task<Result<IReadOnlyCollection<BookResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<BookResponse>>.Succeed(books, ResponseMessages.BooksRetrieved);
    }

public async Task<Result<BookResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(id, cancellationToken);
        return book is null
            ? Result<BookResponse>.Fail(ResponseMessages.BookNotFound)
            : Result<BookResponse>.Succeed(book, ResponseMessages.BookRetrieved);
    }

public async Task<Result<BookResponse>> CreateAsync(CreateBookRequest request, int authorId, CancellationToken cancellationToken)
    {
        var username = await _userRepository.GetUsernameByIdAsync(authorId, cancellationToken);
        if (string.IsNullOrWhiteSpace(username))
        {
            return Result<BookResponse>.Fail(ResponseMessages.AuthorNotFound);
        }

        var book = new Book
        {
            Title = request.Title.Trim(),
            AuthorId = authorId,
            Author = username,
            Isbn = request.Isbn.Trim(),
            YearPublished = request.YearPublished
        };

        var createdBook = await _bookRepository.AddAsync(book, cancellationToken);
        return Result<BookResponse>.Succeed(createdBook, ResponseMessages.BookCreated);
    }

public async Task<Result<BookResponse>> UpdateAsync(
        int id,
        UpdateBookRequest request,
        int requestingUserId,
        bool isAdmin,
        CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetForUpdateAsync(id, cancellationToken);
        if (book is null)
        {
            return Result<BookResponse>.Fail(ResponseMessages.BookNotFound);
        }

        if (!isAdmin && book.AuthorId != requestingUserId)
        {
            return Result<BookResponse>.Fail(ResponseMessages.Forbidden);
        }

        book.Title = request.Title.Trim();
        book.Isbn = request.Isbn.Trim();
        book.YearPublished = request.YearPublished;

        var updatedBook = await _bookRepository.UpdateAsync(book, cancellationToken);
        return Result<BookResponse>.Succeed(updatedBook, ResponseMessages.BookUpdated);
    }

public async Task<Result<OperationResult>> DeleteAsync(int id, int requestingUserId, bool isAdmin, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetForUpdateAsync(id, cancellationToken);
        if (book is null)
        {
            return Result<OperationResult>.Fail(ResponseMessages.BookNotFound);
        }

        if (!isAdmin && book.AuthorId != requestingUserId)
        {
            return Result<OperationResult>.Fail(ResponseMessages.Forbidden);
        }

        await _bookRepository.DeleteAsync(book, cancellationToken);

        return Result<OperationResult>.Succeed(
            new OperationResult { Completed = true },
            ResponseMessages.BookDeleted);
    }
}

