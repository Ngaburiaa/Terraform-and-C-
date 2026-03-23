using BookRepositoryApi.Constants;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Repositories.Interfaces;
using BookRepositoryApi.Services.Interfaces;

namespace BookRepositoryApi.Services;

// Coordinates authentication and registration workflows.
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    // Initializes a new instance of the AuthService class.
    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalized = request.Username.Trim().ToLowerInvariant();
        var user = await _userRepository.GetByNormalizedUsernameAsync(normalized, cancellationToken);
        if (user is null || !_passwordService.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            return Result<LoginResponse>.Fail(ResponseMessages.InvalidCredentials);
        }

        var response = _tokenService.CreateLoginResponse(user);
        return Result<LoginResponse>.Succeed(response, ResponseMessages.LoginSucceeded);
    }

public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var normalized = username.ToLowerInvariant();
        var existingUser = await _userRepository.GetByNormalizedUsernameAsync(normalized, cancellationToken);
        if (existingUser is not null)
        {
            return Result<LoginResponse>.Fail(ResponseMessages.UsernameAlreadyExists);
        }

        var user = new AppUser
        {
            Username = username,
            NormalizedUsername = normalized,
            Role = Security.Roles.Reader
        };
        user.PasswordHash = _passwordService.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);

        var response = _tokenService.CreateLoginResponse(user);
        return Result<LoginResponse>.Succeed(response, ResponseMessages.RegistrationSucceeded);
    }
}

