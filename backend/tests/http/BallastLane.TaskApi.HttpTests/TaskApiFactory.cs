using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BallastLane.TaskApi.HttpTests;

/// <summary>
/// Hosts the Task API in-process against the real test database.
/// </summary>
public sealed class TaskApiFactory : WebApplicationFactory<TaskApiEntryPoint>
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
public sealed class TaskApiCollection : ICollectionFixture<TaskApiFactory>
{
    public const string Name = "task-api";
}

