namespace BookRepositoryApi.Routes;

public static class ApiRoutes
{
    private const string Base = "api";

    public static class Books
    {
        public const string Root = Base + "/books";
        // integer identifier instead of guid
        public const string ById = Root + "/{id:int}";
    }

    public static class Auth
    {
        public const string Login = Base + "/auth/login";
        public const string Register = Base + "/auth/register";
    }

    public static class Users
    {
        public const string Root = Base + "/users";
        public const string ById = Root + "/{id:int}";
    }
}
