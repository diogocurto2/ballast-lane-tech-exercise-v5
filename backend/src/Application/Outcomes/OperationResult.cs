namespace BallastLane.Application.Outcomes;

/// <summary>
/// The outcome category of an application operation. Each category maps to exactly one HTTP status,
/// which keeps the API's error contract single-valued.
/// </summary>
public enum OperationStatus
{
    Success,
    ValidationFailed,
    Unauthenticated,
    NotFound,
    Conflict,
}

/// <summary>
/// A field-specific validation message, keyed by the request field it belongs to.
/// </summary>
public sealed record FieldError(string Field, string Message);

/// <summary>
/// The result of an operation that returns no value.
/// </summary>
public readonly record struct Unit
{
    public static Unit Value => default;
}

/// <summary>
/// An application outcome that either carries a value or describes why the operation failed, without
/// throwing for expected failures.
/// </summary>
public sealed class OperationResult<T>
{
    private OperationResult(OperationStatus status, T? value, string? detail, IReadOnlyList<FieldError> errors)
    {
        Status = status;
        Value = value;
        Detail = detail;
        Errors = errors;
    }

    public OperationStatus Status { get; }

    public T? Value { get; }

    public string? Detail { get; }

    public IReadOnlyList<FieldError> Errors { get; }

    public bool IsSuccess => Status == OperationStatus.Success;

    public static OperationResult<T> Success(T value) =>
        new(OperationStatus.Success, value, detail: null, errors: []);

    public static OperationResult<T> Validation(
        IReadOnlyList<FieldError> errors,
        string detail = "One or more validation errors occurred.") =>
        new(OperationStatus.ValidationFailed, default, detail, errors);

    public static OperationResult<T> Unauthenticated(string detail) =>
        new(OperationStatus.Unauthenticated, default, detail, errors: []);

    public static OperationResult<T> NotFound(string detail) =>
        new(OperationStatus.NotFound, default, detail, errors: []);

    public static OperationResult<T> Conflict(string detail) =>
        new(OperationStatus.Conflict, default, detail, errors: []);
}
