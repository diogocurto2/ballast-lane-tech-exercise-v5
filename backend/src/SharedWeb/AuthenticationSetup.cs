using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.SharedWeb;

/// <summary>
/// Authentication wiring shared by both API hosts, so the security configuration cannot drift between
/// them.
/// </summary>
public static class AuthenticationSetup
{
    public const string Issuer = Application.Abstractions.TokenContract.Issuer;

    public const string Audience = Application.Abstractions.TokenContract.Audience;

    internal const string SigningKeyConfigurationName = "Jwt:SigningKey";

    /// <summary>
    /// Builds the explicit validation parameters. Every check is enabled deliberately and clock skew
    /// is zero, so an expired token is rejected on time rather than up to five minutes late.
    /// </summary>
    public static TokenValidationParameters CreateTokenValidationParameters(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var signingKey = configuration[SigningKeyConfigurationName];

        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                $"The JWT signing key '{SigningKeyConfigurationName}' is not configured. "
                + "Set the Jwt__SigningKey environment variable.");
        }

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtRegisteredClaimNames.Email,
        };
    }

    /// <summary>
    /// Registers bearer authentication plus a fallback policy that denies anonymous access. Every
    /// endpoint must therefore be marked public explicitly, which is what makes protection the default.
    /// </summary>
    public static IServiceCollection AddSharedAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = CreateTokenValidationParameters(configuration);
                options.MapInboundClaims = false;

                // The framework's default challenge writes an empty body. Writing the documented
                // ProblemDetails keeps 401 responses consistent with every other error shape.
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await WriteProblemAsync(
                            context.Response,
                            StatusCodes.Status401Unauthorized,
                            "Authentication required",
                            "A valid bearer token is required to access this resource.");
                    },
                    OnForbidden = context => WriteProblemAsync(
                        context.Response,
                        StatusCodes.Status403Forbidden,
                        "Forbidden",
                        "The authenticated identity is not permitted to perform this operation."),
                };
            });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }

    private static async Task WriteProblemAsync(
        HttpResponse response,
        int statusCode,
        string title,
        string detail)
    {
        if (response.HasStarted)
        {
            return;
        }

        response.StatusCode = statusCode;
        response.ContentType = "application/problem+json";

        // WriteAsJsonAsync would force "application/json", and content type is part of the contract.
        await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(
            new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = statusCode == StatusCodes.Status401Unauthorized
                    ? "https://tools.ietf.org/html/rfc9110#section-15.5.2"
                    : "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                Detail = detail,
            }));
    }
}
