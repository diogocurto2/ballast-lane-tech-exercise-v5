namespace BallastLane.Domain.Users;

/// <summary>
/// Identifies an authenticated user. A value object so a user identifier cannot be confused with a
/// task identifier in a signature or a query.
/// </summary>
public readonly record struct UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
