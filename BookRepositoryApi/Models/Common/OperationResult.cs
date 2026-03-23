namespace BookRepositoryApi.Models.Common;

// Represents a simple success outcome without a complex payload.
public sealed class OperationResult
{
    // Gets or sets a value indicating whether the operation succeeded.
    public bool Completed { get; init; }
}

