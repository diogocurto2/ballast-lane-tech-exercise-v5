namespace BallastLane.PostgreSql.Tests;

internal static class TestDatabase
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=postgres";

    /// <summary>
    /// Connection string for the shared test database. A missing database fails the run rather than
    /// skipping it, so there is no skip path here.
    /// </summary>
    internal static string ConnectionString { get; } = Isolate(
        Environment.GetEnvironmentVariable("ConnectionStrings__TestDatabase") is { Length: > 0 } configured
            ? configured
            : DefaultConnectionString);

    private static string Isolate(string connectionString)
    {
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = $"{new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Database}_postgres",
            // A cold container can take a moment to accept connections; a short default timeout would
            // surface as a spurious assertion failure.
            Timeout = 60,
            // Several suites run at once; a large default pool per process can exhaust the server.
            MaxPoolSize = 10,
        };

        return builder.ConnectionString;
    }
}



