# Coding Standards

Practical code conventions for the .NET backend and the TypeScript/React frontend.

## 1. Purpose and scope

This standard defines how code is written so it is readable, reviewable, and consistent. It applies to all C#, TypeScript, and SQL source in this repository.

It does not define architecture; see [clean-architecture.md](./clean-architecture.md). It does not define API contracts; see [api-standards.md](./api-standards.md).

## 2. Mandatory rules

### 2.1 C# conventions

- Naming MUST follow the .NET conventions: `PascalCase` for types, methods, properties, and constants; `camelCase` for parameters and locals; `_camelCase` for private fields; interfaces prefixed with `I`.
- Nullable reference types MUST be enabled project-wide, and code MUST compile without nullable warnings. Do not silence a warning with `!` unless the invariant is documented.
- Asynchronous methods MUST be `async` end to end. Blocking calls such as `.Result`, `.Wait()`, and `.GetAwaiter().GetResult()` MUST NOT appear in request paths.
- Public asynchronous methods that perform I/O MUST accept a `CancellationToken` and pass it to every downstream call. The token MUST be forwarded to Npgsql commands.
- Async methods MUST be suffixed `Async`.
- Disposable resources MUST be disposed deterministically with `using` declarations or `await using`.

### 2.2 Clarity and responsibility

- Each method SHOULD do one thing at one level of abstraction. Extract when a reader must scroll to follow one idea.
- Methods SHOULD stay short enough to read on one screen; a long method is a signal, not a lint rule.
- Names MUST express intent. Prohibit abbreviations that require decoding, and prohibit vague names such as `Helper`, `Util`, `Manager`, and `Data`.
- Public types and members of shared layers MUST have XML documentation only where the intent is not evident from the signature. Do not document trivia.

### 2.3 Exceptions and error propagation

- Exceptions MUST NOT be used for normal control flow.
- Infrastructure failures MUST be translated into meaningful application outcomes at the boundary, not leaked as raw driver exceptions to the API layer.
- Business rule violations SHOULD be expressed as domain or application outcomes or explicit exception types, which the API layer maps to status codes and ProblemDetails.
- A caught exception MUST be either handled or rethrown with context. Silent swallowing is prohibited.
- `catch (Exception)` MUST NOT be used unless it is a documented top-level boundary that logs and returns a generic failure.

### 2.4 Dependency injection

- Dependencies MUST be provided through constructor injection. Service locator usage (`IServiceProvider.GetService` inside business code) is prohibited outside composition roots.
- Services SHOULD be registered with the narrowest appropriate lifetime. Scoped is the default for request-bound work.
- A class MUST NOT resolve its own dependencies, and MUST NOT construct infrastructure types directly.

### 2.5 Logging

- Logging MUST NOT include passwords, password hashes, tokens, connection strings, secret values, or full personal data.
- Log messages SHOULD be structured, with named properties rather than string concatenation.
- Exceptions SHOULD be logged with the exception object, not only its message.
- Log levels MUST be meaningful: not everything at `Information`, and not `Debug` for operational failures.

### 2.6 TypeScript and React

- TypeScript strict mode MUST be enabled, including `strict`, `noImplicitAny`, and `strictNullChecks`.
- `any` MUST NOT be introduced. Use a concrete type, a discriminated union, `unknown` with narrowing, or a documented generic.
- Type assertions (`as`) MUST be avoided where a runtime check is feasible. Untyped external data MUST be validated or narrowed before use.
- React components SHOULD be function components with typed props. Container and presentational concerns SHOULD be separated; see [frontend-standards.md](./frontend-standards.md).
- State MUST be typed, and derived values SHOULD be computed rather than stored twice.
- Hooks MUST follow the rules of hooks; exhaustive dependency arrays MUST NOT be suppressed to silence lint warnings without a documented reason.

### 2.7 Formatting, linting, and warnings

- Code MUST be formatted consistently by the repository's configured formatter; manual alignment is not a substitute.
- The backend build MUST produce no compiler warnings. If a warning cannot be avoided, it MUST be suppressed narrowly with a documented reason.
- Frontend lint MUST pass with the configured rules. Disabling a rule inline MUST be limited to a specific line and justified.
- Formatting and lint configuration MUST be committed so results are reproducible.

### 2.8 Comments and documentation

- Comments MUST explain **why**, not restate **what** the code does.
- A comment MUST be added where a decision is non-obvious: a workaround, a constraint, a security decision, or an ordering requirement.
- Commented-out code MUST NOT be committed. Version control preserves history.
- Outdated comments MUST be corrected or removed in the same change that invalidates them.

### 2.9 Change size and commits

- A change SHOULD be small enough to review in one sitting. Unrelated cleanup MUST NOT be bundled with a behavior change.
- Commits MUST be meaningful: a focused subject line in the imperative mood, and a body when the reason is not obvious.
- A commit MUST NOT mix formatting-only churn with behavior changes.
- A commit MUST NOT break the build or the test suite.

## 3. Practical examples

**CancellationToken propagation**

```csharp
public async Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken)
{
    await using var command = new NpgsqlCommand(sql, connection);
    command.Parameters.AddWithValue("id", id);
    await using var reader = await command.ExecuteReaderAsync(cancellationToken);
    return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
}
```

**Exception translation at the boundary**

```csharp
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    throw new ConflictException("A user with this email already exists.", ex);
}
```

**Typed API access instead of `any`**

```ts
type TaskItem = { id: string; title: string; status: TaskStatus };

async function listTasks(): Promise<TaskItem[]> {
  const response = await fetch("/api/tasks", { credentials: "include" });
  if (!response.ok) throw new ApiError(response.status);
  return (await response.json()) as TaskItem[];
}
```

**Comment explaining a decision**

```csharp
// Ownership is enforced here rather than in SQL so both API hosts share the rule.
```

## 4. Prohibited practices and anti-patterns

- Dead code, unused members, unused parameters kept "for later".
- Commented-out implementations.
- `catch { }` or `catch (Exception) { }` that hides failures.
- `.Result` or `.Wait()` in request paths.
- `#nullable disable` or blanket warning suppression.
- `any` in TypeScript, or `as` used to silence a type error.
- Primitive obsession for domain concepts that carry rules (bare strings for identifiers with format rules).
- Abstractions introduced without a consumer need; see [solid-principles.md](./solid-principles.md).
- Logging request bodies or tokens.
- Mixing a refactor with a feature in one commit.

## 5. Verification and review criteria

| Check | Method |
| --- | --- |
| Build warnings | `dotnet build` with warnings visible, configured as errors where practical. |
| Nullable | Nullable reference types enabled; no nullable warnings. |
| Async discipline | Review for blocking calls and missing `CancellationToken` propagation. |
| Frontend strictness | TypeScript strict flags enabled; lint passes with no `any`. |
| Secrets in logs | Review logging statements; confirm no sensitive values. |
| Dead code | Confirm no commented-out blocks or unused members are introduced. |
| Commit hygiene | Review commit subjects and the split between refactor and behavior. |
| Comments | Confirm comments explain decisions, not syntax. |

## 6. References

- Layer responsibilities: [clean-architecture.md](./clean-architecture.md)
- SOLID and abstraction limits: [solid-principles.md](./solid-principles.md)
- Security-sensitive logging and secrets: [security.md](./security.md)
- API error shapes: [api-standards.md](./api-standards.md)
- Frontend component rules: [frontend-standards.md](./frontend-standards.md)
- Build and lint pipelines: [delivery-and-ci.md](./delivery-and-ci.md)
