using System.ComponentModel.DataAnnotations;

namespace BookRepositoryApi.Models;

public sealed class CreateBookRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // author is derived from the authenticated user; not supplied by the client

    [Required, MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1400, 3000)]
    public int YearPublished { get; set; }
}

