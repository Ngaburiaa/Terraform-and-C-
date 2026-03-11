using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookRepositoryApi.Data;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Security;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookRepositoryApi.Services;

public sealed class AuthService : IAuthService
{
    private readonly JwtSettings _jwt;
    private readonly PasswordHasher<AppUser> _hasher = new();
    private readonly AppDbContext _context;

    public AuthService(IOptions<JwtSettings> jwtOptions, AppDbContext context)
    {
        _jwt = jwtOptions.Value;
        _context = context;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        var normalized = request.Username.Trim().ToLowerInvariant();
        var user = _context.Users.FirstOrDefault(u => u.NormalizedUsername == normalized);
        if (user is null)
        {
            return null;
        }

        var verification = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification is PasswordVerificationResult.Failed)
        {
            return null;
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(60);
        var token = CreateToken(user, expiresAtUtc);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            Username = user.Username,
            Role = user.Role
        };
    }

    public LoginResponse? Register(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var normalized = username.ToLowerInvariant();

        if (_context.Users.Any(u => u.NormalizedUsername == normalized))
        {
            return null;
        }

        var user = new AppUser
        {
            Username = username,
            NormalizedUsername = normalized,
            Role = Roles.User
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(60);
        var token = CreateToken(user, expiresAtUtc);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            Username = user.Username,
            Role = user.Role
        };
    }

    private string CreateToken(AppUser user, DateTime expiresAtUtc)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
