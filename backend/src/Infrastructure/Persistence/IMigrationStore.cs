namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Records and applies migration scripts. The seam exists so the runner's ordering and once-only
/// rules are testable without a database.
/// </summary>
public interface IMigrationStore
{
    Task EnsureInitializedAsync(CancellationToken cancellationToken);

    Task<IReadOnlySet<string>> GetAppliedVersionsAsync(CancellationToken cancellationToken);

    Task ApplyAsync(MigrationScript script, CancellationToken cancellationToken);
}
