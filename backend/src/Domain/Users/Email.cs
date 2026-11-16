namespace BallastLane.Domain.Users;

/// <summary>
/// A validated, normalized email address. Construct only through <see cref="Create"/> or
/// <see cref="TryCreate"/>, so an unvalidated string cannot be stored as an address.
/// </summary>
public sealed record Email
{
    public const int MaximumLength = 320;

    private Email(string value) => Value = value;

    /// <summary>The normalized (lower-case, trimmed) address.</summary>
    public string Value { get; }

    public static string Normalize(string? input) => (input ?? string.Empty).Trim().ToLowerInvariant();

    public static bool TryCreate(string? input, out Email? email)
    {
        email = null;
        var normalized = Normalize(input);

        if (!IsWellFormed(normalized))
        {
            return false;
        }

        email = new Email(normalized);
        return true;
    }

    public static Email Create(string? input)
    {
        if (!TryCreate(input, out var email))
        {
            throw new ArgumentException("The value is not a valid email address.", nameof(input));
        }

        return email!;
    }

    public override string ToString() => Value;

    // A single '@' with a non-empty local part and a domain containing a dot, no whitespace, and no
    // consecutive or boundary dots. Deliberately conservative: the address is an identifier, not a
    // deliverability claim.
    private static bool IsWellFormed(string normalized)
    {
        if (normalized.Length is 0 or > MaximumLength || normalized.Contains(' ', StringComparison.Ordinal))
        {
            return false;
        }

        var parts = normalized.Split('@');
        if (parts.Length != 2)
        {
            return false;
        }

        var (local, domain) = (parts[0], parts[1]);
        if (local.Length == 0 || domain.Length == 0)
        {
            return false;
        }

        if (!domain.Contains('.', StringComparison.Ordinal)
            || domain.StartsWith('.')
            || domain.EndsWith('.')
            || domain.Contains("..", StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }
}
