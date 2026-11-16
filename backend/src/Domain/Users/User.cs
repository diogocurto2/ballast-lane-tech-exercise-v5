namespace BallastLane.Domain.Users;

/// <summary>
/// A registered account. The plaintext password never reaches this type: it holds only a hash
/// produced by the application's hashing component.
/// </summary>
public sealed class User
{
    public User(UserId id, Email email, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public UserId Id { get; }

    public Email Email { get; }

    public string PasswordHash { get; }

    public DateTimeOffset CreatedAt { get; }
}
