using System.ComponentModel.DataAnnotations;

namespace BookRepositoryApi.Models;

public sealed class CreateBookRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(120)]
    public string Author { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1400, 3000)]
    public int YearPublished { get; set; }
}
