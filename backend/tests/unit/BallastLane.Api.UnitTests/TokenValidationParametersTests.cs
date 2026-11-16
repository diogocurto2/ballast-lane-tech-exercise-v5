using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BallastLane.SharedWeb;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.Api.UnitTests;

public sealed class TokenValidationParametersTests
{
    private const string SigningKey = "unit-test-signing-key-with-enough-length-0123456789";

    [Fact]
    public void Validation_parameters_are_explicit_and_use_the_documented_issuer_and_audience()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));

        Assert.True(parameters.ValidateIssuer);
        Assert.True(parameters.ValidateAudience);
        Assert.True(parameters.ValidateIssuerSigningKey);
        Assert.True(parameters.ValidateLifetime);
        Assert.Equal(TimeSpan.Zero, parameters.ClockSkew);
        Assert.Equal("ballastlane-user-api", parameters.ValidIssuer);
        Assert.Equal("ballastlane-clients", parameters.ValidAudience);
    }

    [Fact]
    public void A_correctly_issued_token_validates()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));
        var subject = Guid.NewGuid().ToString();
        var token = CreateToken(SigningKey, DateTimeOffset.UtcNow.AddMinutes(30), subject: subject);

        var principal = new JwtSecurityTokenHandler { MapInboundClaims = false }
            .ValidateToken(token, parameters, out var validated);

        Assert.Equal(subject, principal.FindFirstValue(JwtRegisteredClaimNames.Sub));
        Assert.Equal("ballastlane-user-api", validated.Issuer);
    }

    [Fact]
    public void A_token_signed_with_a_foreign_key_fails_validation()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));
        var token = CreateToken(
            "a-completely-different-signing-key-0123456789",
            DateTimeOffset.UtcNow.AddMinutes(30));

        Assert.ThrowsAny<SecurityTokenException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));
    }

    [Fact]
    public void A_token_past_its_expiry_fails_validation()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));
        var token = CreateToken(SigningKey, DateTimeOffset.UtcNow.AddMinutes(-5));

        Assert.Throws<SecurityTokenExpiredException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));
    }

    [Fact]
    public void A_malformed_token_fails_validation()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));

        // IdentityModel reports a non-JWT string as a malformed token rather than as a signature error.
        Assert.Throws<SecurityTokenMalformedException>(() =>
            new JwtSecurityTokenHandler().ValidateToken("this-is-not-a-jwt", parameters, out _));
    }

    [Fact]
    public void A_token_from_an_unknown_issuer_fails_validation()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));
        var token = CreateToken(SigningKey, DateTimeOffset.UtcNow.AddMinutes(30), issuer: "some-other-api");

        Assert.Throws<SecurityTokenInvalidIssuerException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));
    }

    [Fact]
    public void A_token_for_an_unknown_audience_fails_validation()
    {
        var parameters = AuthenticationSetup.CreateTokenValidationParameters(CreateConfiguration(SigningKey));
        var token = CreateToken(SigningKey, DateTimeOffset.UtcNow.AddMinutes(30), audience: "some-other-client");

        Assert.Throws<SecurityTokenInvalidAudienceException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _));
    }

    [Fact]
    public void Missing_signing_key_configuration_fails_fast()
    {
        var configuration = CreateConfiguration(signingKey: null);

        Assert.Throws<InvalidOperationException>(
            () => AuthenticationSetup.CreateTokenValidationParameters(configuration));
    }

    private static IConfiguration CreateConfiguration(string? signingKey) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SigningKey"] = signingKey,
            })
            .Build();

    private static string CreateToken(
        string signingKey,
        DateTimeOffset expires,
        string subject = "user",
        string issuer = "ballastlane-user-api",
        string audience = "ballastlane-clients")
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: [new Claim(JwtRegisteredClaimNames.Sub, subject)],
            notBefore: expires.UtcDateTime.AddHours(-1),
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
