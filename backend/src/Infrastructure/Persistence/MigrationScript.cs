namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// One ordered SQL migration file. <paramref name="Version"/> is the file name, which is what the
/// runner records so a migration is applied exactly once.
/// </summary>
public sealed record MigrationScript(string Version, string Sql);
