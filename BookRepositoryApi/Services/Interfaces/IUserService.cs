using BookRepositoryApi.Models.Common;
using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Services.Interfaces;

public interface IUserService
{
    // Retrieves all users.
    Task<Result<IReadOnlyCollection<UserResponse>>> GetAllAsync(CancellationToken cancellationToken);

    // Retrieves a user by identifier.
    Task<Result<UserResponse>> GetByIdAsync(int id, CancellationToken cancellationToken);

    // Deletes a user by identifier.
    Task<Result<OperationResult>> DeleteAsync(int id, CancellationToken cancellationToken);
}

