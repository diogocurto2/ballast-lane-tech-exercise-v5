using BallastLane.Infrastructure.Persistence;
using BallastLane.Infrastructure.Security;
using BallastLane.Infrastructure.Time;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.Migrations.Tests;

public sealed class DemoSeedTests
{
    [Fact]
    public async Task Seeding_twice_neither_duplicates_nor_fails()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var factory = CreateFactory(database);
        await MigrateAsync(factory);

        var seeder = new DemoSeeder(factory, new IdentityPasswordHasher(), new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);

        var usersAfterFirstRun = await CountAsync(database, "users");
        var tasksAfterFirstRun = await CountAsync(database, "tasks");

        await seeder.SeedAsync(CancellationToken.None);

        Assert.NotEqual(0, usersAfterFirstRun);
        Assert.NotEqual(0, tasksAfterFirstRun);
        Assert.Equal(usersAfterFirstRun, await CountAsync(database, "users"));
        Assert.Equal(tasksAfterFirstRun, await CountAsync(database, "tasks"));

        foreach (var account in DemonstrationData.Accounts)
        {
            Assert.Equal(1, await CountAccountsWithEmailAsync(database, account.Email));
        }
    }

    [Fact]
    public async Task Every_seeded_password_verifies_against_its_stored_hash()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var factory = CreateFactory(database);
        await MigrateAsync(factory);

        var hasher = new IdentityPasswordHasher();
        var seeder = new DemoSeeder(factory, hasher, new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);

        foreach (var account in DemonstrationData.Accounts)
        {
            var storedHash = await ReadPasswordHashAsync(database, account.Email);

            Assert.NotNull(storedHash);
            Assert.NotEqual(account.Password, storedHash);
            Assert.True(hasher.Verify(storedHash, account.Password));
        }
    }

    private static NpgsqlConnectionFactory CreateFactory(ScratchDatabase database) =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database.ConnectionString,
            })
            .Build());

    private static async Task MigrateAsync(NpgsqlConnectionFactory factory)
    {
        var migrator = new SchemaMigrator(new NpgsqlMigrationStore(factory));
        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);
    }

    private static async Task<int> CountAsync(ScratchDatabase database, string tableName)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        // The table name is supplied by this test, never by input, and cannot be parameterized.
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM {tableName}", connection);
        return (int)(long)(await command.ExecuteScalarAsync() ?? 0L);
    }

    private static async Task<int> CountAccountsWithEmailAsync(ScratchDatabase database, string email)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT count(*) FROM users WHERE lower(email) = lower(@email)",
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
