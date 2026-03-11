using BookRepositoryApi.Data;
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
            .OrderBy(u => u.Username)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            })
            .ToList();
    }

    public UserResponse? GetById(int id)
    {
        return _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            })
            .FirstOrDefault();
    }
}
