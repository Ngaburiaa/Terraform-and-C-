namespace BookRepositoryApi.Models.Auth;

using BookRepositoryApi.Models;

public sealed class UserResponse
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;

    // when fetching users we include any books they own
    public IReadOnlyCollection<BookResponse> Books { get; init; } = Array.Empty<BookResponse>();
}
