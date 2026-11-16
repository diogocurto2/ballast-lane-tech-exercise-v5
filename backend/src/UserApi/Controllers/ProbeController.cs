using BallastLane.SharedWeb;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.UserApi.Controllers;

/// <summary>
/// Diagnostic routes that prove the authorization boundary. <c>undecorated</c> carries no explicit
/// authorization attribute on purpose: the fallback policy must deny it, so protection is never lost
/// by omission.
/// </summary>
[ApiController]
[Route("api/probe")]
public sealed class ProbeController : ControllerBase
{
    [HttpGet("undecorated")]
    public IActionResult Undecorated() => Ok(new { reachable = true });

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        var identity = User.ToAuthenticatedUser();

        return identity is null
            ? Unauthorized()
            : Ok(new { userId = identity.UserId.ToString(), email = identity.Email });
    }
}
