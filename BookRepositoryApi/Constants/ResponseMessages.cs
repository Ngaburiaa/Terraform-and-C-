namespace BookRepositoryApi.Constants;

// Stores standardized user-safe response messages.
public static class ResponseMessages
{
    public const string InvalidCredentials = "Invalid username or password.";
    public const string UsernameAlreadyExists = "Username already exists.";
    public const string RegistrationSucceeded = "Registration completed successfully.";
    public const string LoginSucceeded = "Login completed successfully.";
    public const string BookNotFound = "Book not found.";
    public const string BooksRetrieved = "Books retrieved successfully.";
    public const string BookRetrieved = "Book retrieved successfully.";
    public const string BookCreated = "Book created successfully.";
    public const string BookUpdated = "Book updated successfully.";
    public const string BookDeleted = "Book deleted successfully.";
    public const string UserNotFound = "User not found.";
    public const string UsersRetrieved = "Users retrieved successfully.";
    public const string UserRetrieved = "User retrieved successfully.";
    public const string UserDeleted = "User deleted successfully.";
    public const string AuthorNotFound = "Author account was not found.";
    public const string ValidationFailed = "One or more validation errors occurred.";
    public const string Forbidden = "You are not authorized to perform this action.";
    public const string UnexpectedError = "An unexpected error occurred.";
    public const string HealthCheckSucceeded = "Health check completed successfully.";
}

