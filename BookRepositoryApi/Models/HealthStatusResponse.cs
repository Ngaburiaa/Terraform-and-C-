namespace BookRepositoryApi.Models;

// Represents the health status payload exposed by monitoring endpoints.
public sealed class HealthStatusResponse
{
    // Gets or sets the status value.
    public string Status { get; init; } = string.Empty;

    // Gets or sets the UTC timestamp for the health result.
    public DateTime TimestampUtc { get; init; }

    // Gets or sets the service name.
    public string? Service { get; init; }
}

