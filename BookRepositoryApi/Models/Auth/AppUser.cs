namespace BookRepositoryApi.Models.Auth;

public sealed class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NormalizedUsername { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // navigation property for books created by this user
    public ICollection<BookRepositoryApi.Models.Book> Books { get; set; } = new List<BookRepositoryApi.Models.Book>();
}

