using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.TaskApi.HttpTests;

/// <summary>
/// Owns the real test database for the Task API suite: migrations once per process, and a cleared
/// state per test. A missing database fails the run rather than skipping it.
/// </summary>
internal static class TestDatabase
{
    private static readonly SemaphoreSlim MigrationLock = new(1, 1);
    private static bool _migrated;

    internal static string ConnectionString { get; } = Isolate(
        Environment.GetEnvironmentVariable("ConnectionStrings__TestDatabase") is { Length: > 0 } configured
            ? configured
            : "Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=postgres",
        "_taskapi");

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

    /// <summary>
    /// Executes a statement built by test code (never by a request) so a test can arrange rows
    /// directly.
    /// </summary>
    internal static async Task ExecuteAsync(string sql)
    {
        await MigrateOnceAsync();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>Creates an account row directly so a test can own tasks without going through the API.</summary>
    internal static async Task<Guid> CreateUserAsync(string email, string passwordHash)
    {
        await MigrateOnceAsync();
        var id = Guid.NewGuid();

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "INSERT INTO users (id, email, password_hash) VALUES (@id, @email, @hash)",
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("email", email);
        command.Parameters.AddWithValue("hash", passwordHash);
        await command.ExecuteNonQueryAsync();

        return id;
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



