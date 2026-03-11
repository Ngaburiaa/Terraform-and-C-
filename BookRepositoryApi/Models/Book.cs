namespace BookRepositoryApi.Models;

public sealed class Book
{
    // switch from Guid to integer; database will generate this value automatically
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int YearPublished { get; set; }
}
