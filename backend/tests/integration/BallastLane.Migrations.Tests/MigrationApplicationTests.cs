using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.Migrations.Tests;

public sealed class MigrationApplicationTests
{
    [Fact]
    public async Task Applying_the_migrations_to_an_empty_database_creates_the_documented_schema()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var migrator = CreateMigrator(database);

        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        Assert.Equal(
            ["schema_migrations", "tasks", "users"],
            await ReadTableNamesAsync(database));
    }

    [Fact]
    public async Task The_schema_enforces_the_users_uniqueness_and_task_rules()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var migrator = CreateMigrator(database);

        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        var usersIndexes = await ReadIndexDefinitionsAsync(database, "users");
        Assert.Contains(
            usersIndexes,
            definition => definition.Contains("UNIQUE INDEX", StringComparison.Ordinal)
                && definition.Contains("lower(email)", StringComparison.Ordinal));

        var taskIndexes = await ReadIndexDefinitionsAsync(database, "tasks");
        Assert.Contains(
            taskIndexes,
            definition => definition.Contains("ix_tasks_owner_due_created", StringComparison.Ordinal));

        var taskChecks = await ReadCheckConstraintsAsync(database, "tasks");
        Assert.Contains(taskChecks, check => check.Contains("btrim(title)", StringComparison.Ordinal));
        Assert.Contains(taskChecks, check => check.Contains("length(description)", StringComparison.Ordinal));
        Assert.Contains(taskChecks, check => check.Contains("'InProgress'", StringComparison.Ordinal));

        var foreignKeys = await ReadForeignKeyConstraintsAsync(database, "tasks");
        var reference = Assert.Single(foreignKeys);
        Assert.Contains("REFERENCES users(id)", reference, StringComparison.Ordinal);
        Assert.Contains("ON DELETE CASCADE", reference, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Task_columns_hold_a_calendar_due_date_and_a_defaulted_status()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var migrator = CreateMigrator(database);

        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        string? dueDateType;
        string? statusDefault;
        bool statusNullable;

        await using (var command = new NpgsqlCommand(
            """
            SELECT data_type
            FROM information_schema.columns
            WHERE table_schema = current_schema() AND table_name = 'tasks' AND column_name = 'due_date'
            """,
            connection))
        {
            dueDateType = (string?)await command.ExecuteScalarAsync();
        }

        await using (var command = new NpgsqlCommand(
            """
            SELECT column_default, is_nullable
            FROM information_schema.columns
            WHERE table_schema = current_schema() AND table_name = 'tasks' AND column_name = 'status'
            """,
            connection))
        await using (var reader = await command.ExecuteReaderAsync())
        {
            Assert.True(await reader.ReadAsync());
            statusDefault = reader.IsDBNull(0) ? null : reader.GetString(0);
            statusNullable = reader.GetString(1) == "YES";
        }

        Assert.Equal("date", dueDateType);
        Assert.False(statusNullable);
        Assert.NotNull(statusDefault);
        Assert.Contains("Pending", statusDefault, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Re_running_the_migrations_on_a_migrated_database_changes_nothing()
    {
        await using var database = await ScratchDatabase.CreateAsync();
        var migrator = CreateMigrator(database);

        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);
        var appliedAfterFirstRun = await CountAppliedMigrationsAsync(database);

        await migrator.RunAsync(RepositoryPaths.MigrationsDirectory, CancellationToken.None);

        Assert.Equal(2, appliedAfterFirstRun);
        Assert.Equal(appliedAfterFirstRun, await CountAppliedMigrationsAsync(database));
    }

    private static SchemaMigrator CreateMigrator(ScratchDatabase database)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = database.ConnectionString,
            })
            .Build();

        return new SchemaMigrator(new NpgsqlMigrationStore(new NpgsqlConnectionFactory(configuration)));
    }

    private static async Task<IReadOnlyList<string>> ReadTableNamesAsync(ScratchDatabase database)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = current_schema() ORDER BY table_name",
            connection);
        await using var reader = await command.ExecuteReaderAsync();

        var names = new List<string>();
        while (await reader.ReadAsync())
        {
            names.Add(reader.GetString(0));
        }

        return names;
    }

    private static async Task<IReadOnlyList<string>> ReadIndexDefinitionsAsync(
        ScratchDatabase database,
        string tableName)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT indexdef FROM pg_indexes WHERE schemaname = current_schema() AND tablename = @table",
            connection);
        command.Parameters.AddWithValue("table", tableName);
        await using var reader = await command.ExecuteReaderAsync();

        var definitions = new List<string>();
        while (await reader.ReadAsync())
        {
            definitions.Add(reader.GetString(0));
        }

        return definitions;
    }

    private static async Task<IReadOnlyList<string>> ReadCheckConstraintsAsync(
        ScratchDatabase database,
        string tableName)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            SELECT pg_get_constraintdef(c.oid)
            FROM pg_constraint c
            JOIN pg_class t ON t.oid = c.conrelid
            WHERE c.contype = 'c' AND t.relname = @table
            """,
            connection);
        command.Parameters.AddWithValue("table", tableName);
        await using var reader = await command.ExecuteReaderAsync();

        var definitions = new List<string>();
        while (await reader.ReadAsync())
        {
            definitions.Add(reader.GetString(0));
        }

        return definitions;
    }

    private static async Task<IReadOnlyList<string>> ReadForeignKeyConstraintsAsync(
        ScratchDatabase database,
        string tableName)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            SELECT pg_get_constraintdef(c.oid)
            FROM pg_constraint c
            JOIN pg_class t ON t.oid = c.conrelid
            WHERE c.contype = 'f' AND t.relname = @table
            """,
            connection);
        command.Parameters.AddWithValue("table", tableName);
        await using var reader = await command.ExecuteReaderAsync();

        var definitions = new List<string>();
        while (await reader.ReadAsync())
        {
            definitions.Add(reader.GetString(0));
        }

        return definitions;
    }

    private static async Task<int> CountAppliedMigrationsAsync(ScratchDatabase database)
    {
        await using var connection = new NpgsqlConnection(database.ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("SELECT count(*) FROM schema_migrations", connection);
        return (int)(long)(await command.ExecuteScalarAsync() ?? 0L);
    }
}

