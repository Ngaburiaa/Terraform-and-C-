namespace BookRepositoryApi.Models;

public sealed class BookResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public int YearPublished { get; init; }

    // author info for convenience
    public int AuthorId { get; init; }
    public string AuthorUsername { get; init; } = string.Empty;
}
