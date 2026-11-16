using BallastLane.Application.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace BallastLane.Infrastructure.Security;

/// <summary>
/// Adapts the established ASP.NET Core Identity password hasher. The component generates a unique
/// salt per hash and performs a constant-time comparison during verification, so no custom
/// cryptography is written here.
/// </summary>
public sealed class IdentityPasswordHasher : IPasswordHasher
{
    // PasswordHasher<T> ignores the user instance for hashing and verification, so a stable placeholder
    // keeps a framework type out of the application layer without changing behavior.
    private static readonly object PasswordHasherUser = new();

    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return _hasher.HashPassword(PasswordHasherUser, password);
    }

    public bool Verify(string passwordHash, string password)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(password);

        // A SuccessRehashNeeded result still means the password was correct; hash rotation is out of scope.
        return _hasher.VerifyHashedPassword(PasswordHasherUser, passwordHash, password)
            != PasswordVerificationResult.Failed;
    }
}
