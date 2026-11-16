using BallastLane.Domain.Users;

namespace BallastLane.Application.Abstractions;

/// <summary>
/// Stores new accounts. Returns <c>false</c> when the database rejects the email as a duplicate, so
/// the use case can report a conflict instead of surfacing a driver exception.
/// </summary>
public interface IUserWriter
{
    Task<bool> AddAsync(User user, CancellationToken cancellationToken);
}
