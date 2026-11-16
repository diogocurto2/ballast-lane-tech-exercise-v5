using BallastLane.Domain.Users;

namespace BallastLane.Application.Abstractions;

/// <summary>
/// Reads accounts. Email lookups are case-insensitive, matching the database's unique index on
/// <c>lower(email)</c>.
/// </summary>
public interface IUserReader
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken);
}
