namespace BookRepositoryApi.Models.Common;

// Represents the standardized API response contract.
public sealed class ApiResponse<T>
{
    // Gets or sets a value indicating whether the request succeeded.
    public bool Success { get; init; }

    // Gets or sets the user-safe response message.
    public string Message { get; init; } = string.Empty;

    // Gets or sets the response payload.
    public T? Data { get; init; }

    // Gets or sets the validation or processing errors.
    public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();

    // Creates a response from an application result.
    public static ApiResponse<T> FromResult(Result<T> result) =>
        new()
        {
            Success = result.Success,
            Message = result.Message,
            Data = result.Data
        };

    // Creates a failed response without a payload.
    public static ApiResponse<T> Failure(string message, IReadOnlyCollection<string>? errors = null) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors ?? Array.Empty<string>()
        };
}

