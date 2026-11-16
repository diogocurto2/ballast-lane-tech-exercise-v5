namespace BallastLane.Domain.Users;

/// <summary>
/// The acting identity for a request. Constructed only from validated claims, so no client-supplied
/// field can produce one.
/// </summary>
public sealed record AuthenticatedUser(UserId UserId, string Email);
