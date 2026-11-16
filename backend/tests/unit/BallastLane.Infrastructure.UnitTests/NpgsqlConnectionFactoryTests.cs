using System.Data;
using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.Infrastructure.UnitTests;

public sealed class NpgsqlConnectionFactoryTests
{
    private const string ConfiguredConnectionString =
        "Host=127.0.0.1;Port=5432;Database=taskmanager;Username=postgres;Password=local-test";

    [Fact]
    public void Creating_a_connection_returns_an_unopened_connection_with_the_configured_string()
    {
        var factory = CreateFactory(ConfiguredConnectionString);

        using var connection = factory.Create();

        Assert.Equal(ConnectionState.Closed, connection.State);
        Assert.Equal(ConfiguredConnectionString, connection.ConnectionString);
    }

    [Fact]
    public void Creating_a_connection_reads_the_ConnectionStrings__Database_environment_variable()
    {
        const string environmentConnectionString =
            "Host=127.0.0.1;Port=5432;Database=from_environment;Username=postgres;Password=local-test";

        Environment.SetEnvironmentVariable("ConnectionStrings__Database", environmentConnectionString);

        try
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var factory = new NpgsqlConnectionFactory(configuration);

            using var connection = factory.Create();

            Assert.Equal(environmentConnectionString, connection.ConnectionString);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__Database", null);
        }
    }

    [Fact]
    public void Creating_a_connection_without_a_configured_string_fails_with_a_clear_message()
    {
        var factory = CreateFactory(connectionString: null);

        var exception = Assert.Throws<InvalidOperationException>(() => factory.Create());

        Assert.Contains("ConnectionStrings:Database", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Executing_a_command_does_not_swallow_a_connection_failure()
    {
        var factory = CreateFactory(ConfiguredConnectionString);
        using var connection = factory.Create();

        // A closed connection must surface as an error rather than a silent zero-row success.
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            factory.ExecuteAsync(
                connection,
                "SELECT 1",
                Array.Empty<NpgsqlParameter>(),
                CancellationToken.None));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            factory.ExecuteReaderAsync(
                connection,
                "SELECT 1 AS value",
                Array.Empty<NpgsqlParameter>(),
                CancellationToken.None));
    }

    private static NpgsqlConnectionFactory CreateFactory(string? connectionString) =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = connectionString,
            })
            .Build());
}
