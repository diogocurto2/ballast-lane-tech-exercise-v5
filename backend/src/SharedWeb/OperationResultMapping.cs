using System.Diagnostics;
using BallastLane.Application.Outcomes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.SharedWeb;

/// <summary>
/// Maps application outcomes to HTTP results. This is the single place where the status convention is
/// decided: 400 shape validation, 401 authentication, 404 missing-or-not-owned, 409 conflict, 204 on a
/// successful delete. <c>422</c> is deliberately unused.
/// </summary>
public static class OperationResultMapping
{
    private const string ValidationType = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
    private const string AuthenticationType = "https://tools.ietf.org/html/rfc9110#section-15.5.2";
    private const string NotFoundType = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
    private const string ConflictType = "https://tools.ietf.org/html/rfc9110#section-15.5.10";

    public static IActionResult ToActionResult<T>(this OperationResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Status switch
        {
            OperationStatus.Success => new OkObjectResult(result.Value),
            _ => CreateFailure(result.Status, result.Detail, result.Errors),
        };
    }

    public static IActionResult ToNoContentResult<T>(this OperationResult<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Status switch
        {
            OperationStatus.Success => new NoContentResult(),
            _ => CreateFailure(result.Status, result.Detail, result.Errors),
        };
    }

    public static IActionResult ToCreatedResult<T>(
        this OperationResult<T> result,
        Func<T, string> locationFactory)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(locationFactory);

        return result.Status switch
        {
            OperationStatus.Success => new CreatedResult(locationFactory(result.Value!), result.Value),
            _ => CreateFailure(result.Status, result.Detail, result.Errors),
        };
    }

    private static ObjectResult CreateFailure(
        OperationStatus status,
        string? detail,
        IReadOnlyList<FieldError> errors)
    {
        var (statusCode, title, type) = status switch
        {
            OperationStatus.ValidationFailed => (StatusCodes.Status400BadRequest, "Validation failed", ValidationType),
            OperationStatus.Unauthenticated => (StatusCodes.Status401Unauthorized, "Authentication required", AuthenticationType),
            OperationStatus.NotFound => (StatusCodes.Status404NotFound, "Resource not found", NotFoundType),
            OperationStatus.Conflict => (StatusCodes.Status409Conflict, "Conflict", ConflictType),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred", "about:blank"),
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = Activity.Current?.Id,
        };

        if (errors.Count > 0)
        {
            problem.Extensions["errors"] = errors
                .GroupBy(error => error.Field, StringComparer.Ordinal)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.Message).ToArray(),
                    StringComparer.Ordinal);
        }

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
