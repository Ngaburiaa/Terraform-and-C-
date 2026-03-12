using BookRepositoryApi.Data;
using BookRepositoryApi.Models;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<UserResponse> GetAll()
    {
        return _context.Users
            .AsNoTracking()
            .Include(u => u.Books)
            .OrderBy(u => u.Username)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Books = u.Books
                    .OrderBy(b => b.Title)
                    .Select(b => new BookResponse
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Isbn = b.Isbn,
                        YearPublished = b.YearPublished,
                        AuthorId = b.AuthorId,
                        AuthorUsername = b.Author
                    })
                    .ToList()
            })
            .ToList();
    }

    public UserResponse? GetById(int id)
    {
        return _context.Users
            .AsNoTracking()
            .Include(u => u.Books)
            .Where(u => u.Id == id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Books = u.Books
                    .OrderBy(b => b.Title)
                    .Select(b => new BookResponse
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Isbn = b.Isbn,
                        YearPublished = b.YearPublished,
                        AuthorId = b.AuthorId,
                        AuthorUsername = b.Author
                    })
                    .ToList()
            })
            .FirstOrDefault();
    }

    public bool Delete(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }
}
