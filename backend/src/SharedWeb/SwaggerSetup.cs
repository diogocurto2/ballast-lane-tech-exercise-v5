using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BallastLane.SharedWeb;

/// <summary>
/// Decides whether each host serves its OpenAPI document and Swagger UI. Shared so a change to the
/// exposure rule cannot apply to one host and miss the other.
/// </summary>
public static class SwaggerSetup
{
    internal const string EnabledConfigurationName = "Swagger:Enabled";

    /// <summary>
    /// True in Development, and in any other environment only when the deployment opts in with
    /// <c>Swagger:Enabled=true</c>. The document describes every route and is served
    /// unauthenticated, so exposure is never implicit outside local development.
    /// </summary>
    public static bool IsEnabled(string environmentName, IConfiguration configuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(environmentName);
        ArgumentNullException.ThrowIfNull(configuration);

        if (environmentName == Environments.Development)
        {
            return true;
        }

        return bool.TryParse(configuration[EnabledConfigurationName], out var enabled) && enabled;
    }
}
