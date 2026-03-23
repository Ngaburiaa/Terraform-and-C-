using BookRepositoryApi.Models.Auth;

namespace BookRepositoryApi.Repositories.Interfaces;

// Defines persistence operations for application users.
public interface IUserRepository
{
    // Retrieves a user by normalized username.
    Task<AppUser?> GetByNormalizedUsernameAsync(string normalizedUsername, CancellationToken cancellationToken);

    // Retrieves all users with their books.
    Task<IReadOnlyCollection<UserResponse>> GetAllAsync(CancellationToken cancellationToken);

    // Retrieves a user by identifier with books.
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    // Retrieves a username by identifier.
    Task<string?> GetUsernameByIdAsync(int id, CancellationToken cancellationToken);

    // Adds a new user.
    Task AddAsync(AppUser user, CancellationToken cancellationToken);

    // Deletes a user by identifier.
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}

