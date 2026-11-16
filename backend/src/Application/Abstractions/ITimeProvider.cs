namespace BallastLane.Application.Abstractions;

/// <summary>
/// Supplies the current instant. Injected so timestamps and token expiry are deterministic in tests.
/// </summary>
public interface ITimeProvider
{
    DateTimeOffset UtcNow { get; }
}
