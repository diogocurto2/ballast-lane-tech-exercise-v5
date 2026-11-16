namespace BallastLane.Infrastructure.Persistence;

/// <summary>
/// Applies SQL migration files in file-name order, each exactly once, recording the version in
/// <c>schema_migrations</c>. Files are never edited after being applied: corrections arrive as a new
/// file.
/// </summary>
public sealed class SchemaMigrator(IMigrationStore store)
{
    private const string SqlFilePattern = "*.sql";

    public IMigrationStore Store { get; } = store;

    public static IReadOnlyList<MigrationScript> LoadScripts(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"The migrations directory '{directoryPath}' was not found.");
        }

        var scripts = new List<MigrationScript>();

        // File-name ordering is what makes migration order deterministic and reviewable.
        foreach (var path in Directory
                     .EnumerateFiles(directoryPath, SqlFilePattern)
                     .OrderBy(Path.GetFileName, StringComparer.Ordinal))
        {
            var fileName = Path.GetFileName(path);
            try
            {
                scripts.Add(new MigrationScript(fileName, File.ReadAllText(path)));
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                throw new MigrationLoadException(fileName, exception);
            }
        }

        return scripts;
    }

    public async Task RunAsync(string directoryPath, CancellationToken cancellationToken)
    {
        var scripts = LoadScripts(directoryPath);

        await Store.EnsureInitializedAsync(cancellationToken);
        var applied = await Store.GetAppliedVersionsAsync(cancellationToken);

        foreach (var script in scripts)
        {
            if (applied.Contains(script.Version))
            {
                continue;
            }

            await Store.ApplyAsync(script, cancellationToken);
        }
    }
}
