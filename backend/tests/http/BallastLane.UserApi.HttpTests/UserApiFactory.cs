using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BallastLane.UserApi.HttpTests;

/// <summary>
/// Hosts the User API in-process against the real test database, so HTTP behavior is verified against
/// the same PostgreSQL constraints the application uses.
/// </summary>
public sealed class UserApiFactory : WebApplicationFactory<UserApiEntryPoint>
{
    public const string SigningKey = "http-test-signing-key-with-enough-length-0123456789";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = TestDatabase.ConnectionString,
                ["Jwt:SigningKey"] = SigningKey,
                ["FrontendOrigin"] = "http://localhost:5173",
            });
        });
    }
}

[CollectionDefinition(Name)]
public sealed class UserApiCollection : ICollectionFixture<UserApiFactory>
{
    public const string Name = "user-api";
}

