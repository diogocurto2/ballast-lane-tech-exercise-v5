using System.Diagnostics.CodeAnalysis;

namespace BallastLane.Migrations.Tests;

/// <summary>
/// Locates repository artifacts that tests need, without encoding a machine-specific absolute path.
/// </summary>
internal static class RepositoryPaths
{
    private const string SolutionFileName = "BallastLane.sln";

    internal static string Root { get; } = FindRoot();

    internal static string MigrationsDirectory => Path.Combine(Root, "backend", "db", "migrations");

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"The repository root was not found: no ancestor directory contains {SolutionFileName}.");
    }
}

internal static class TestDatabase
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=postgres";

    /// <summary>
    /// Connection string for this suite's own database. A missing database must fail the run rather
    /// than skip it, which is why there is no skip path in these tests. The name is suffixed so a
    /// suite running in parallel cannot contend with another suite's database.
    /// </summary>
    internal static string ConnectionString { get; } = Isolate(
        Environment.GetEnvironmentVariable("ConnectionStrings__TestDatabase") is { Length: > 0 } configured
            ? configured
            : DefaultConnectionString);

    private static string Isolate(string connectionString)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = $"{new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database}_migrations",
            // A cold container can take a moment to accept connections; a short default timeout would
            // surface as a spurious assertion failure.
            Timeout = 60,
            // Several suites run at once; a large default pool per process can exhaust the server.
            MaxPoolSize = 10,
        };

        return builder.ConnectionString;
    }
}

/// <summary>
/// An empty PostgreSQL schema inside this suite's database, used as a throwaway namespace for schema
/// and seed tests. A schema costs a single catalog entry, whereas <c>CREATE DATABASE</c> copies the
/// template and contends on its lock, which made the parallel run time out.
/// </summary>
internal sealed class ScratchDatabase : IAsyncDisposable
{
    private ScratchDatabase(string schemaName, string connectionString)
    {
        SchemaName = schemaName;
        ConnectionString = connectionString;
    }

    internal string SchemaName { get; }

    internal string ConnectionString { get; }

    internal static async Task<ScratchDatabase> CreateAsync(CancellationToken cancellationToken = default)
    {
        // The schema name is generated here and never comes from user input, and PostgreSQL does not
        // accept parameters for identifiers, so composing it into DDL is safe.
        var schemaName = $"scratch_{Guid.NewGuid():N}";

        await using (var connection = new Npgsql.NpgsqlConnection(TestDatabase.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await using var command = new Npgsql.NpgsqlCommand($"CREATE SCHEMA {schemaName}", connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        var builder = new Npgsql.NpgsqlConnectionStringBuilder(TestDatabase.ConnectionString)
        {
            SearchPath = schemaName,
        };

        return new ScratchDatabase(schemaName, builder.ConnectionString);
    }

    [SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "The identifier is a generated name that matches an internal pattern, never user input.")]
    public async ValueTask DisposeAsync()
    {
        Npgsql.NpgsqlConnection.ClearAllPools();

        await using var connection = new Npgsql.NpgsqlConnection(TestDatabase.ConnectionString);
        await connection.OpenAsync();

        await using var command = new Npgsql.NpgsqlCommand($"DROP SCHEMA IF EXISTS {SchemaName} CASCADE", connection);
        await command.ExecuteNonQueryAsync();
    }
}
