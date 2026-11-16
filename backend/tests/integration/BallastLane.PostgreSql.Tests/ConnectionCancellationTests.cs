using BallastLane.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BallastLane.PostgreSql.Tests;

/// <summary>
/// Cancellation forwarding is only observable once a connection is really open, which is why it is
/// verified against PostgreSQL rather than with a substitute.
/// </summary>
public sealed class ConnectionCancellationTests
{
    [Fact]
    public async Task Executing_a_command_with_a_cancelled_token_throws_operation_cancelled()
    {
        var factory = CreateFactory();
        await using var connection = await factory.OpenAsync(CancellationToken.None);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            factory.ExecuteAsync(
                connection,
                "SELECT 1",
                Array.Empty<NpgsqlParameter>(),
                cancellation.Token));
    }

    [Fact]
    public async Task Reading_with_a_cancelled_token_throws_operation_cancelled()
    {
        var factory = CreateFactory();
        await using var connection = await factory.OpenAsync(CancellationToken.None);
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            factory.ExecuteReaderAsync(
                connection,
                "SELECT 1 AS value",
                Array.Empty<NpgsqlParameter>(),
                cancellation.Token));
    }

    private static NpgsqlConnectionFactory CreateFactory() =>
        new(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = TestDatabase.ConnectionString,
            })
            .Build());
}
