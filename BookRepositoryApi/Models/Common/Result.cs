namespace BookRepositoryApi.Models.Common;

// Represents the outcome of an application operation.
public sealed class Result<T>
{
    // Gets a value indicating whether the operation succeeded.
    public bool Success { get; init; }

    // Gets the user-safe operation message.
    public string Message { get; init; } = string.Empty;

    // Gets the operation payload when successful.
    public T? Data { get; init; }

    // Creates a successful result.
    public static Result<T> Succeed(T data, string message) =>
        new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    // Creates a failed result.
    public static Result<T> Fail(string message) =>
        new()
        {
            Success = false,
            Message = message
        };
}

