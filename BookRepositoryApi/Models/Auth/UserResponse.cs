namespace BookRepositoryApi.Models.Auth;

public sealed class UserResponse
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
