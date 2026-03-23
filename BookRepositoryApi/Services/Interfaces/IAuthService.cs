using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

public interface IAuthService
{
    // Authenticates a user and returns a JWT payload when successful.
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    // Registers a new user and returns a JWT payload when successful.
    Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}

