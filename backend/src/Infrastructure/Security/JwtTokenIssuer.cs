using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BallastLane.Application.Abstractions;
using BallastLane.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.Infrastructure.Security;

/// <summary>
/// Issues HS256 bearer tokens matching the validation contract both hosts enforce: the documented
/// issuer and audience, a 60-minute lifetime, and a fresh <c>jti</c> per token.
/// </summary>
public sealed class JwtTokenIssuer(IConfiguration configuration, ITimeProvider timeProvider) : ITokenIssuer
{
    public const string Issuer = TokenContract.Issuer;

    public const string Audience = TokenContract.Audience;

    public static readonly TimeSpan Lifetime = TokenContract.Lifetime;

    private const string SigningKeyConfigurationName = "Jwt:SigningKey";

    public IssuedToken Issue(UserId userId, string email)
    {
        var signingKey = configuration[SigningKeyConfigurationName];

        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                $"The JWT signing key '{SigningKeyConfigurationName}' is not configured. "
                + "Set the Jwt__SigningKey environment variable.");
        }

        var issuedAt = timeProvider.UtcNow;
        var expiresAt = issuedAt.Add(Lifetime);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            ],
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));

        return new IssuedToken(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expiresAt);
    }
}
