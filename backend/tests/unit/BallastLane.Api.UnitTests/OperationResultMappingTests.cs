using System.Text.Json;
using BallastLane.Application.Outcomes;
using BallastLane.SharedWeb;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.Api.UnitTests;

public sealed class OperationResultMappingTests
{
    [Fact]
    public void A_successful_result_maps_to_200_with_its_value()
    {
        var result = OperationResult<int>.Success(42);

        var actionResult = result.ToActionResult();

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void Shape_validation_maps_to_400_with_errors_keyed_by_field()
    {
        var result = OperationResult<int>.Validation(
        [
            new FieldError("title", "Title is required."),
            new FieldError("dueDate", "Due date must be a calendar date."),
        ]);

        var objectResult = Assert.IsType<ObjectResult>(result.ToActionResult());

        Assert.Equal(400, objectResult.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal(400, problem.Status);
        Assert.Equal("Validation failed", problem.Title);
        var errors = Assert.IsType<Dictionary<string, string[]>>(problem.Extensions["errors"]);
        Assert.Equal(["Title is required."], errors["title"]);
        Assert.Equal(["Due date must be a calendar date."], errors["dueDate"]);
    }

    [Fact]
    public void Authentication_failure_maps_to_401()
    {
        var result = OperationResult<int>.Unauthenticated("Invalid email or password.");

        var objectResult = Assert.IsType<ObjectResult>(result.ToActionResult());

        Assert.Equal(401, objectResult.StatusCode);
        var problem = Assert.IsType<ProblemDetails>(objectResult.Value);
        Assert.Equal("Invalid email or password.", problem.Detail);
    }

    [Fact]
    public void A_missing_or_not_owned_record_maps_to_404()
    {
        var result = OperationResult<int>.NotFound("Task was not found.");

        var objectResult = Assert.IsType<ObjectResult>(result.ToActionResult());

        Assert.Equal(404, objectResult.StatusCode);
        Assert.Equal(404, Assert.IsType<ProblemDetails>(objectResult.Value).Status);
    }

    [Fact]
    public void A_conflict_maps_to_409()
    {
        var result = OperationResult<int>.Conflict("An account already exists for this email address.");

        var objectResult = Assert.IsType<ObjectResult>(result.ToActionResult());

        Assert.Equal(409, objectResult.StatusCode);
        Assert.Equal(409, Assert.IsType<ProblemDetails>(objectResult.Value).Status);
    }

    [Fact]
    public void A_successful_void_result_maps_to_204_and_failures_keep_their_problem()
    {
        var noContent = Assert.IsType<NoContentResult>(OperationResult<Unit>.Success(Unit.Value).ToNoContentResult());
        var notFound = Assert.IsType<ObjectResult>(
            OperationResult<Unit>.NotFound("Task was not found.").ToNoContentResult());

        Assert.Equal(204, noContent.StatusCode);
        Assert.Equal(404, notFound.StatusCode);
    }

    [Theory]
    [InlineData("validation")]
    [InlineData("authentication")]
    [InlineData("not-found")]
    [InlineData("conflict")]
    public void A_mapped_problem_body_leaks_no_stack_trace_sql_or_driver_text(string outcome)
    {
        var result = outcome switch
        {
            "validation" => OperationResult<int>.Validation([new FieldError("title", "Title is required.")]),
            "authentication" => OperationResult<int>.Unauthenticated("Invalid email or password."),
            "not-found" => OperationResult<int>.NotFound("Task was not found."),
            _ => OperationResult<int>.Conflict("An account already exists for this email address."),
        };

        var objectResult = Assert.IsType<ObjectResult>(result.ToActionResult());
        var body = JsonSerializer.Serialize(objectResult.Value);

        Assert.DoesNotContain("   at ", body, StringComparison.Ordinal);
        Assert.DoesNotContain("BallastLane.Infrastructure", body, StringComparison.Ordinal);
        Assert.DoesNotContain("SELECT", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Npgsql", body, StringComparison.Ordinal);
        Assert.DoesNotContain("PostgresException", body, StringComparison.Ordinal);
    }
}
