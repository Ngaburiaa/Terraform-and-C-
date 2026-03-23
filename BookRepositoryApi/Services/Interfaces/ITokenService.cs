using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

// Defines JWT token generation operations.
public interface ITokenService
{
    // Generates a login response for a user.
    LoginResponse CreateLoginResponse(AppUser user);
}

