using BallastLane.Application.Users;
using BallastLane.SharedWeb;
using BallastLane.UserApi.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.UserApi.Controllers;

/// <summary>
/// Account endpoints. Each action binds, delegates to exactly one use case, and maps the outcome; no
/// business rule lives here.
/// </summary>
[ApiController]
[Route("api/users")]
public sealed class UsersController(
    RegisterUserUseCase registerUser,
    LoginUseCase login,
    GetCurrentUserUseCase getCurrentUser) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await registerUser.ExecuteAsync(request.Email, request.Password, cancellationToken);

        // No Location header: no read endpoint exists for an arbitrary user id, so advertising one
        // would promise a resource the contract does not provide.
        return result.Status == Application.Outcomes.OperationStatus.Success
            ? StatusCode(StatusCodes.Status201Created, UserResponse.From(result.Value!))
            : result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await login.ExecuteAsync(request.Email, request.Password, cancellationToken);

        return result.IsSuccess
            ? Ok(LoginResponse.From(result.Value!))
            : result.ToActionResult();
    }

    [HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        var result = await getCurrentUser.ExecuteAsync(identity, cancellationToken);

        return result.IsSuccess
            ? Ok(UserResponse.From(result.Value!))
            : result.ToActionResult();
    }
}
