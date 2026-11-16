using BallastLane.Infrastructure.Persistence;
using BallastLane.Infrastructure.Security;
using BallastLane.Infrastructure.Time;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var seedRequested = args.Any(argument =>
    string.Equals(argument, "--seed", StringComparison.OrdinalIgnoreCase));

try
{
    var connectionFactory = new NpgsqlConnectionFactory(configuration);
    var migrator = new SchemaMigrator(new NpgsqlMigrationStore(connectionFactory));

    var migrationsPath = ResolveMigrationsPath(configuration);
    await migrator.RunAsync(migrationsPath, CancellationToken.None);
    Console.WriteLine($"Migrations applied from '{migrationsPath}'.");

    if (seedRequested)
    {
        // Seeding runs only after migrations succeed, so the documented dataset is never written
        // against a schema that does not exist yet.
        var seeder = new DemoSeeder(connectionFactory, new IdentityPasswordHasher(), new SystemTimeProvider());
        await seeder.SeedAsync(CancellationToken.None);
        Console.WriteLine("Demonstration data seeded.");
    }

    return 0;
}
catch (Exception exception)
{
    // A failed migration or seed must fail the container rather than leave an unseeded stack running.
    Console.Error.WriteLine($"Migration run failed: {exception.Message}");
    return 1;
}

static string ResolveMigrationsPath(IConfiguration configuration)
{
    var configured = configuration["Migrations:Path"];

    if (!string.IsNullOrWhiteSpace(configured))
    {
        return Path.GetFullPath(configured);
    }

    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null)
    {
        var candidate = Path.Combine(current.FullName, "db", "migrations");
        if (Directory.Exists(candidate))
        {
            return candidate;
        }

        current = current.Parent;
    }

    throw new DirectoryNotFoundException(
        "The migrations directory was not found. Set Migrations__Path to its location.");
}
