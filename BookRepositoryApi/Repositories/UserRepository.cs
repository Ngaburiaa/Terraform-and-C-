using BookRepositoryApi.Data;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Repositories;

// Handles database access for application users.
public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    // Initializes a new instance of the UserRepository class.
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

public Task<AppUser?> GetByNormalizedUsernameAsync(string normalizedUsername, CancellationToken cancellationToken) =>
        _context.Users.FirstOrDefaultAsync(user => user.NormalizedUsername == normalizedUsername, cancellationToken);

public async Task<IReadOnlyCollection<UserResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Users
            .AsNoTracking()
            .Include(user => user.Books)
            .OrderBy(user => user.Username)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Books = user.Books
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
                    .ToList()
            })
            .ToListAsync(cancellationToken);

public Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        _context.Users
            .AsNoTracking()
            .Include(user => user.Books)
            .Where(user => user.Id == id)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                Books = user.Books
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
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

public Task<string?> GetUsernameByIdAsync(int id, CancellationToken cancellationToken) =>
        _context.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => user.Username)
            .FirstOrDefaultAsync(cancellationToken);

public async Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(existingUser => existingUser.Id == id, cancellationToken);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

