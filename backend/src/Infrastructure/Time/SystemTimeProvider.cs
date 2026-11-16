using BallastLane.Application.Abstractions;

namespace BallastLane.Infrastructure.Time;

/// <summary>
/// The production clock. Tests substitute <see cref="ITimeProvider"/> so timestamps and token expiry
/// are deterministic.
/// </summary>
public sealed class SystemTimeProvider : ITimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
