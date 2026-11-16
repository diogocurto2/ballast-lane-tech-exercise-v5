using BallastLane.Application.Tasks;
using BallastLane.Domain.Tasks;
using BallastLane.SharedWeb;
using BallastLane.TaskApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.TaskApi.Controllers;

/// <summary>
/// Task endpoints for the authenticated user. Every action derives the owner from the validated
/// claims; no request value can select another owner.
/// </summary>
[ApiController]
[Route("api/tasks")]
public sealed class TasksController(
    ListTasksUseCase listTasks,
    GetTaskUseCase getTask,
    CreateTaskUseCase createTask,
    UpdateTaskUseCase updateTask,
    DeleteTaskUseCase deleteTask) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<TaskResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        var result = await createTask.ExecuteAsync(identity, request.ToInput(), cancellationToken);

        return result.IsSuccess
            // A relative Location keeps the header correct behind any proxy or port mapping, which an
            // absolute URI built from the request host would not.
            ? Created($"/api/tasks/{result.Value!.Id}", TaskResponse.From(result.Value!))
            : result.ToActionResult();
    }

    [HttpPut("{id}")]
    [ProducesResponseType<TaskResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        if (!TaskId.TryParse(id, out var taskId))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Detail = "Task was not found.",
            });
        }

        var result = await updateTask.ExecuteAsync(identity, taskId, request.ToInput(), cancellationToken);

        return result.IsSuccess
            ? Ok(TaskResponse.From(result.Value!))
            : result.ToActionResult();
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        if (!TaskId.TryParse(id, out var taskId))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Detail = "Task was not found.",
            });
        }

        var result = await deleteTask.ExecuteAsync(identity, taskId, cancellationToken);

        return result.ToNoContentResult();
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        var result = await listTasks.ExecuteAsync(identity, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!.Select(TaskResponse.From).ToArray())
            : result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType<TaskResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var identity = User.ToAuthenticatedUser();

        if (identity is null)
        {
            return Unauthorized();
        }

        // A value that is not a UUID cannot identify a resource, so it is reported the same way a
        // missing task is rather than as a malformed request.
        if (!TaskId.TryParse(id, out var taskId))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource not found",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Detail = "Task was not found.",
            });
        }

        var result = await getTask.ExecuteAsync(identity, taskId, cancellationToken);

        return result.IsSuccess
            ? Ok(TaskResponse.From(result.Value!))
            : result.ToActionResult();
    }
}





