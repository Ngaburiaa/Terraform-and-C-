namespace BookRepositoryApi.Models;

public sealed class Book
{
    // switch from Guid to integer; database will generate this value automatically
    public int Id { get; set; }

    // foreign key to the user who created / owns this book
    public int AuthorId { get; set; }

    // optional duplicate of the author name for convenience / display
    public string Author { get; set; } = string.Empty;

    public BookRepositoryApi.Models.Auth.AppUser? AuthorUser { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int YearPublished { get; set; }
}
