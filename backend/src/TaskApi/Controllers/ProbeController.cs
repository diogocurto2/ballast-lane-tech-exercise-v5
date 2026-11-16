using BallastLane.SharedWeb;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskApi.Controllers;

/// <summary>
/// Diagnostic routes that prove the authorization boundary of this host. <c>undecorated</c> carries no
/// explicit authorization attribute on purpose: the fallback policy must deny it.
/// </summary>
[ApiController]
[Route("api/probe")]
public sealed class ProbeController : ControllerBase
{
    [HttpGet("undecorated")]
    public IActionResult Undecorated() => Ok(new { reachable = true });

    [HttpGet("protected")]
    public IActionResult Protected() => Ok(new { reachable = true });

    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        var identity = User.ToAuthenticatedUser();

        return identity is null
            ? Unauthorized()
            : Ok(new { userId = identity.UserId.ToString(), email = identity.Email });
    }
}
