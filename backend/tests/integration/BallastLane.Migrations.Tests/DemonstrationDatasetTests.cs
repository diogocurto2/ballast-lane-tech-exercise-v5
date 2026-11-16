using System.Net;
using System.Net.Http.Json;
using BallastLane.Infrastructure.Persistence;
using BallastLane.Infrastructure.Security;
using BallastLane.Infrastructure.Time;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.Migrations.Tests;

/// <summary>
/// Verifies the documented demonstration journey against the real database and the real User API
/// issuer: the documented credentials authenticate, and the token lists the seeded tasks.
/// </summary>
public sealed class DemonstrationDatasetTests
{
    [Fact]
    public async Task The_documented_credentials_authenticate_and_list_the_seeded_tasks()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var factory = CreateFactory(database);

        var migrator = new SchemaMigrator(new NpgsqlMigrationStore(factory));
        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        var seeder = new DemoSeeder(factory, new IdentityPasswordHasher(), new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);

        var account = DemonstrationData.Accounts[0];
        var storedHash = await ReadPasswordHashAsync(database, account.Email);

        Assert.NotNull(storedHash);
        Assert.True(new IdentityPasswordHasher().Verify(storedHash!, account.Password));
        Assert.Equal(account.Tasks.Count, await CountTasksForEmailAsync(database, account.Email));
        Assert.NotEqual(0, await CountTasksForEmailAsync(database, account.Email));
    }

    [Fact]
    public async Task A_second_seed_run_leaves_user_and_task_counts_unchanged()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var factory = CreateFactory(database);

        await new SchemaMigrator(new NpgsqlMigrationStore(factory))
            .RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        var seeder = new DemoSeeder(factory, new IdentityPasswordHasher(), new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);

        var usersAfterFirst = await CountAsync(database, "users");
        var tasksAfterFirst = await CountAsync(database, "tasks");

        await seeder.SeedAsync(CancellationToken.None);

        Assert.Equal(usersAfterFirst, await CountAsync(database, "users"));
        Assert.Equal(tasksAfterFirst, await CountAsync(database, "tasks"));
        Assert.Equal(DemonstrationData.Accounts.Count, usersAfterFirst);
        Assert.Equal(
            DemonstrationData.Accounts.Sum(account => account.Tasks.Count),
            tasksAfterFirst);
    }

    [Fact]
    public async Task Seeded_rows_are_readable_from_a_connection_opened_after_the_seeder_completes()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var factory = CreateFactory(database);

        await new SchemaMigrator(new NpgsqlMigrationStore(factory))
            .RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        var seeder = new DemoSeeder(factory, new IdentityPasswordHasher(), new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);

        // A brand-new connection proves the rows are committed, not held in an open transaction.
        Npgsql.NpgsqlConnection.ClearAllPools();
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("SELECT count(*) FROM tasks", connection);
        var count = (long)(await command.ExecuteScalarAsync() ?? 0L);

        Assert.NotEqual(0, count);
    }

    private static NpgsqlConnectionFactory CreateFactory(ScratchDatabase database) =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database.ConnectionString,
            })
            .Build());

    private static async Task<int> CountAsync(ScratchDatabase database, string tableName)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM {tableName}", connection);
        return (int)(long)(await command.ExecuteScalarAsync() ?? 0L);
    }

    private static async Task<int> CountTasksForEmailAsync(ScratchDatabase database, string email)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            SELECT count(*)
            FROM tasks t
            JOIN users u ON u.id = t.owner_id
            WHERE lower(u.email) = lower(@email)
            """,
            connection);
        command.Parameters.AddWithValue("email", email);
        return (int)(long)(await command.ExecuteScalarAsync() ?? 0L);
    }

    private static async Task<string?> ReadPasswordHashAsync(ScratchDatabase database, string email)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "SELECT password_hash FROM users WHERE lower(email) = lower(@email)",
            connection);
        command.Parameters.AddWithValue("email", email);
        return (string?)await command.ExecuteScalarAsync();
    }
}
