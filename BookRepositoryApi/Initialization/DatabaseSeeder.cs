using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Security;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookRepositoryApi.Initialization;

// Seeds application data required for local development or controlled environments.
public sealed class DatabaseSeeder
{
    private readonly Data.AppDbContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IPasswordService _passwordService;

    // Initializes a new instance of the DatabaseSeeder class.
    public DatabaseSeeder(
        Data.AppDbContext dbContext,
        ILogger<DatabaseSeeder> logger,
        IPasswordService passwordService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _passwordService = passwordService;
    }

    // Applies migrations and optionally seeds configured users.
    public async Task InitializeAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        await _dbContext.Database.MigrateAsync(cancellationToken);

        if (await _dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        if (!configuration.GetValue("SeedUsers:Enabled", false))
        {
            _logger.LogInformation("User seeding is disabled.");
            return;
        }

        var usersToSeed = BuildSeedUsers(configuration);
        if (usersToSeed.Count == 0)
        {
            _logger.LogWarning("User seeding was enabled, but no valid seed users were configured.");
            return;
        }

        await _dbContext.Users.AddRangeAsync(usersToSeed, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private List<AppUser> BuildSeedUsers(IConfiguration configuration)
    {
        var users = new List<AppUser>();
        AddSeedUser(users, "admin", Roles.Admin, configuration["SeedUsers:AdminPassword"]);
        AddSeedUser(users, "author", Roles.Author, configuration["SeedUsers:AuthorPassword"]);
        AddSeedUser(users, "reader", Roles.Reader, configuration["SeedUsers:ReaderPassword"]);
        return users;
    }

    private void AddSeedUser(ICollection<AppUser> users, string username, string role, string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Skipping seed user {Username} because no password was configured.", username);
            return;
        }

        var user = new AppUser
        {
            Username = username,
            NormalizedUsername = username.ToLowerInvariant(),
            Role = role
        };
        user.PasswordHash = _passwordService.HashPassword(user, password);
        users.Add(user);
    }
}

