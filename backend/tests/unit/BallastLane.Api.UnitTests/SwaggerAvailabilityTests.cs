using BallastLane.SharedWeb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BallastLane.Api.UnitTests;

/// <summary>
/// The OpenAPI document describes every route and is served without authentication, so a shipped
/// environment must not expose it by accident.
/// </summary>
public sealed class SwaggerAvailabilityTests
{
    [Fact]
    public void Development_serves_the_document_without_configuration()
    {
        Assert.True(SwaggerSetup.IsEnabled(Environments.Development, CreateConfiguration(null)));
    }

    [Fact]
    public void A_shipped_environment_hides_the_document_by_default()
    {
        Assert.False(SwaggerSetup.IsEnabled(Environments.Production, CreateConfiguration(null)));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("", false)]
    public void A_shipped_environment_honours_an_explicit_opt_in(string configured, bool expected)
    {
        Assert.Equal(expected, SwaggerSetup.IsEnabled(Environments.Production, CreateConfiguration(configured)));
    }

    private static IConfiguration CreateConfiguration(string? enabled) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Swagger:Enabled"] = enabled,
            })
            .Build();
}
