using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BookRepositoryApi.Services;

// Provides password hashing and verification behavior.
public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

public string HashPassword(AppUser user, string password) =>
        _passwordHasher.HashPassword(user, password);

public bool VerifyPassword(AppUser user, string hashedPassword, string providedPassword) =>
        _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword)
            is not PasswordVerificationResult.Failed;
}

