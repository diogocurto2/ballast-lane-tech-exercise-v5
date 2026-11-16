using System.Security.Cryptography;

namespace BallastLane.Application.Tests;

/// <summary>
/// Stable test identity values so assertions never depend on a random identifier.
/// </summary>
internal static class TestIdentities
{
    internal static Guid AuthUserId => Guid.Parse("11111111-1111-4111-8111-111111111111");

    internal static Guid OtherUserId => Guid.Parse("22222222-2222-4222-8222-222222222222");

    internal static Guid TaskId => Guid.Parse("33333333-3333-4333-8333-333333333333");
}

/// <summary>
/// A fake hasher for application tests. It never performs real cryptography; the production hasher is
/// covered by its own adapter test, and persistence behavior is covered against PostgreSQL. Hash
/// values are opaque, so a test can prove the plaintext is not stored.
/// </summary>
internal sealed class FakePasswordHasher : BallastLane.Application.Abstractions.IPasswordHasher
{
    private readonly Dictionary<string, string> _hashes = new(StringComparer.Ordinal);

    public string Hash(string password)
    {
        var hash = $"AQAAAA{Guid.NewGuid():N}";
        _hashes[hash] = password;
        return hash;
    }

    public bool Verify(string passwordHash, string password) =>
        _hashes.TryGetValue(passwordHash, out var stored) && string.Equals(stored, password, StringComparison.Ordinal);
}

internal sealed class FixedTimeProvider(DateTimeOffset now) : BallastLane.Application.Abstractions.ITimeProvider
{
    public DateTimeOffset UtcNow { get; } = now;
}

internal sealed class FakeTokenIssuer : BallastLane.Application.Abstractions.ITokenIssuer
{
    public string AccessToken { get; set; } = "issued-token";

    public DateTimeOffset ExpiresAtUtc { get; set; } =
        new(2026, 11, 16, 10, 0, 0, TimeSpan.Zero);

    public int IssueCount { get; private set; }

    public BallastLane.Application.Abstractions.IssuedToken Issue(
        BallastLane.Domain.Users.UserId userId,
        string email)
    {
        IssueCount++;
        return new BallastLane.Application.Abstractions.IssuedToken(AccessToken, "Bearer", ExpiresAtUtc);
    }
}
