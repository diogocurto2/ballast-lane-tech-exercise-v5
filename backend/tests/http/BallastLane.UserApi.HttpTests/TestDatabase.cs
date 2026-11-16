using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.UserApi.HttpTests;

/// <summary>
/// Owns the real test database: applies the migrations once per process and clears application data
/// between tests. A missing database fails the run rather than skipping it.
/// </summary>
internal static class TestDatabase
{
    private static readonly SemaphoreSlim MigrationLock = new(1, 1);
    private static bool _migrated;

    internal static string ConnectionString { get; } = Isolate(
        Environment.GetEnvironmentVariable("ConnectionStrings__TestDatabase") is { Length: > 0 } configured
            ? configured
            : "Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=postgres",
        "_userapi");

    /// <summary>
    /// Gives this suite its own database so suites running in parallel never truncate or assert on
    /// another suite's rows.
    /// </summary>
    private static string Isolate(string connectionString, string suffix)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = $"{new NpgsqlConnectionStringBuilder(connectionString).Database}{suffix}",
            // A cold container can take a moment to accept connections; a short default timeout would
            // surface as a spurious assertion failure.
            Timeout = 60,
            // Several suites run at once; a large default pool per process can exhaust the server.
            MaxPoolSize = 10,
        };

        return builder.ConnectionString;
    }

    internal static string RepositoryRoot
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "BallastLane.sln")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName
                ?? throw new InvalidOperationException("The repository root was not found.");
        }
    }

    internal static async Task ResetAsync()
    {
        await MigrateOnceAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("TRUNCATE TABLE tasks, users CASCADE", connection);
        await command.ExecuteNonQueryAsync();
    }

    internal static async Task ExecuteAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Reads a single scalar as text. Used for row counts that prove a rejected request persisted
    /// nothing. The SQL comes from test code, never from a request.
    /// </summary>
    internal static async Task<string> ScalarAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        var result = await command.ExecuteScalarAsync();
        return Convert.ToString(result, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static async Task MigrateOnceAsync()
    {
        if (_migrated)
        {
            return;
        }

        await MigrationLock.WaitAsync();
        try
        {
            if (_migrated)
            {
                return;
            }

            var factory = new NpgsqlConnectionFactory(
                new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Database"] = ConnectionString,
                    })
                    .Build());

            var migrator = new SchemaMigrator(new NpgsqlMigrationStore(factory));
            await migrator.RunAsync(Path.Combine(RepositoryRoot, "backend", "db", "migrations"), CancellationToken.None);
            _migrated = true;
        }
        finally
        {
            MigrationLock.Release();
        }
    }
}



