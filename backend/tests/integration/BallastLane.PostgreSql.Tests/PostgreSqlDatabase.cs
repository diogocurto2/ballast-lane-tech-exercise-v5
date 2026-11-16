using BallastLane.Domain.Tasks;
using BallastLane.Domain.Users;
using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.PostgreSql.Tests;

internal static class PostgreSqlDatabase
{
    private static readonly SemaphoreSlim MigrationLock = new(1, 1);
    private static bool _migrated;

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

        await using var connection = new NpgsqlConnection(TestDatabase.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("TRUNCATE TABLE tasks, users CASCADE", connection);
        await command.ExecuteNonQueryAsync();
    }

    internal static async Task<UserId> CreateUserAsync(string email)
    {
        await MigrateOnceAsync();
        var id = Guid.NewGuid();

        await using var connection = new NpgsqlConnection(TestDatabase.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "INSERT INTO users (id, email, password_hash) VALUES (@id, @email, @hash)",
            connection);
        command.Parameters.AddWithValue("id", id);
        command.Parameters.AddWithValue("email", email);
        command.Parameters.AddWithValue("hash", "not-a-real-hash");
        await command.ExecuteNonQueryAsync();

        return new UserId(id);
    }

    internal static async Task InsertTaskAsync(
        TaskId id,
        UserId ownerId,
        string title,
        string? description,
        string status,
        DateOnly dueDate,
        DateTimeOffset? createdAt = null)
    {
        await MigrateOnceAsync();
        var timestamp = createdAt ?? new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero);

        await using var connection = new NpgsqlConnection(TestDatabase.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at)
            VALUES (@id, @ownerId, @title, @description, @status, @dueDate, @createdAt, @createdAt)
            """,
            connection);
        command.Parameters.AddWithValue("id", id.Value);
        command.Parameters.AddWithValue("ownerId", ownerId.Value);
        command.Parameters.AddWithValue("title", title);
        command.Parameters.AddWithValue("description", (object?)description ?? DBNull.Value);
        command.Parameters.AddWithValue("status", status);
        command.Parameters.AddWithValue("dueDate", dueDate);
        command.Parameters.AddWithValue("createdAt", timestamp);
        await command.ExecuteNonQueryAsync();
    }

    internal static async Task<string> ScalarAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(TestDatabase.ConnectionString);
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
                        ["ConnectionStrings:Database"] = TestDatabase.ConnectionString,
                    })
                    .Build());

            await new SchemaMigrator(new NpgsqlMigrationStore(factory))
                .RunAsync(Path.Combine(RepositoryRoot, "backend", "db", "migrations"), CancellationToken.None);
            _migrated = true;
        }
        finally
        {
            MigrationLock.Release();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "postgresql";
}

public sealed class PostgreSqlFixture;
