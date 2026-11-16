using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BallastLane.Domain.Users;

namespace BallastLane.SharedWeb;

/// <summary>
/// Turns validated claims into the acting identity. Ownership is always derived from here, never from
/// a request body or query value.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Builds the authenticated user from the <c>sub</c> and <c>email</c> claims. Returns
    /// <c>null</c> when the principal is unauthenticated or its subject is not a valid identifier, so
    /// callers deny rather than guess.
    /// </summary>
    public static AuthenticatedUser? ToAuthenticatedUser(this ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(subject, out var userId))
        {
            return null;
        }

        var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email)
            ?? principal.FindFirstValue(ClaimTypes.Email)
            ?? string.Empty;

        return new AuthenticatedUser(new UserId(userId), email);
    }
}
