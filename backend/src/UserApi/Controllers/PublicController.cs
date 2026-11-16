using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.UserApi.Controllers;

/// <summary>
/// Endpoints that intentionally require no credentials. They exist so the public half of the access
/// boundary is explicit rather than implied by omission.
/// </summary>
[ApiController]
[Route("api/public")]
public sealed class PublicController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Info() =>
        Ok(new { service = "ballastlane-user-api", status = "ok" });
}
