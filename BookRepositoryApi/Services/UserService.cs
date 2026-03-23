using BookRepositoryApi.Constants;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Repositories.Interfaces;
using BookRepositoryApi.Services.Interfaces;

namespace BookRepositoryApi.Services;

// Handles user-related business operations.
public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    // Initializes a new instance of the UserService class.
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

public async Task<Result<IReadOnlyCollection<UserResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<UserResponse>>.Succeed(users, ResponseMessages.UsersRetrieved);
    }

public async Task<Result<UserResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null
            ? Result<UserResponse>.Fail(ResponseMessages.UserNotFound)
            : Result<UserResponse>.Succeed(user, ResponseMessages.UserRetrieved);
    }

public async Task<Result<OperationResult>> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var deleted = await _userRepository.DeleteAsync(id, cancellationToken);
        return deleted
            ? Result<OperationResult>.Succeed(new OperationResult { Completed = true }, ResponseMessages.UserDeleted)
            : Result<OperationResult>.Fail(ResponseMessages.UserNotFound);
    }
}

