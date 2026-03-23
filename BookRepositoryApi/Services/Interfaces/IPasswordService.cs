using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

// Defines password hashing and verification operations.
public interface IPasswordService
{
    // Hashes a plaintext password for a user.
    string HashPassword(AppUser user, string password);

    // Verifies a plaintext password against a stored hash.
    bool VerifyPassword(AppUser user, string hashedPassword, string providedPassword);
}

