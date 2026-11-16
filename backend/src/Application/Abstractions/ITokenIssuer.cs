using BallastLane.Domain.Users;

namespace BallastLane.Application.Abstractions;

/// <summary>
/// A bearer credential issued after successful authentication.
/// </summary>
public sealed record IssuedToken(string AccessToken, string TokenType, DateTimeOffset ExpiresAtUtc);

/// <summary>
/// The token contract shared by the issuer (Infrastructure) and the validator (SharedWeb). Declaring
/// it in the application layer keeps both sides on one definition without Infrastructure referencing
/// the web layer.
/// </summary>
public static class TokenContract
{
    public const string Issuer = "ballastlane-user-api";

    public const string Audience = "ballastlane-clients";

    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(60);
}

/// <summary>
/// Issues bearer tokens for authenticated users. Declared here so the application layer never
/// depends on a token library.
/// </summary>
public interface ITokenIssuer
{
    IssuedToken Issue(UserId userId, string email);
}
