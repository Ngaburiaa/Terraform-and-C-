using System.ComponentModel.DataAnnotations;

namespace BookRepositoryApi.Models;

public sealed class UpdateBookRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    // author is read-only; cannot be changed via update

    [Required, MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(1800, 2027)]
    public int YearPublished { get; set; }
}
