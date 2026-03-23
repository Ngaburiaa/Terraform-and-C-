using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookRepositoryApi.Constants;
using BookRepositoryApi.Models.Auth;
using BookRepositoryApi.Security;
using BookRepositoryApi.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookRepositoryApi.Services;

// Generates JWT access tokens for authenticated users.
public sealed class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    // Initializes a new instance of the TokenService class.
    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

public LoginResponse CreateLoginResponse(AppUser user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(AuthConstants.AccessTokenLifetimeMinutes);
        var signingCredentials = CreateSigningCredentials();

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: BuildClaims(user),
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            Username = user.Username,
            Role = user.Role
        };
    }

    private static Claim[] BuildClaims(AppUser user) =>
    [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    ];

    private SigningCredentials CreateSigningCredentials()
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }
}

