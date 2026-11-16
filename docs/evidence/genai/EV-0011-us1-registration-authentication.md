# EV-0011 - User Story 1: registration and authentication (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0011` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 1, T025 to T037) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US1, FR-001 to FR-004, FR-015, NFR-001) |
| Related task identifiers | `T025` to `T037` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires. |

## Actual prompt

The US1 work continued the same developer instruction that started the session:

```text
/speckit-implement
```

No further developer text was supplied. `specs/001-task-manager/tasks.md` T025 to T037 defined the
required behavior, and `specs/001-task-manager/contracts/user-api.md` defined the HTTP contract.

## Relevant context

- Requirements: US1 acceptance scenarios 1 to 6, FR-001 to FR-004, FR-015, NFR-001.
- Standards applied: `testing-and-tdd.md` (RED first), `api-standards.md`, `security.md`,
  `frontend-standards.md`, `coding-standards.md`, `clean-architecture.md`.
- Contract: `specs/001-task-manager/contracts/user-api.md`.
- Preceding record: `EV-0010` (setup and foundational layer).

## Original AI-generated output

### Registration and login controllers (T033)

The first generated controller version relied on data-annotation validation on the request DTOs:

```csharp
public sealed class RegisterRequest
{
    [Required]
    public string? Email { get; init; }

    [Required]
    public string? Password { get; init; }
}
```

```csharp
public async Task<IActionResult> Register(
    [FromBody] RegisterRequest request,
    CancellationToken cancellationToken)
{
    var result = await registerUser.ExecuteAsync(request.Email, request.Password, cancellationToken);

    return result.Status == Application.Outcomes.OperationStatus.Success
        ? StatusCode(StatusCodes.Status201Created, UserResponse.From(result.Value!))
        : result.ToActionResult();
}
```

### Authentication challenge handling (T018, corrected during US1)

The first generated wiring let the framework write its own empty challenge body:

```csharp
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = CreateTokenValidationParameters(configuration);
    options.MapInboundClaims = false;
});
```

### Frontend authentication state (T034)

The first frontend draft exposed a provider that could never authenticate, and a login page that
re-authenticated with empty credentials:

```tsx
export function AuthProvider({ children }: AuthProviderProps) {
  const value: AuthState = {
    isAuthenticated: false,
    user: null,
    error: null,
    login: async () => {},
    logout: () => {},
    refreshCurrentUser: async () => {},
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
```

```tsx
<LoginForm
  api={usersApi}
  onAuthenticated={() => {
    void auth.login({ email: "", password: "" }).then(() => navigate("/tasks"));
  }}
/>
```

### Process deviation: implementation written before RED for the unit layers

The US1 domain and application types (`Email`, `UserValidator`, `RegisterUserUseCase`,
`LoginUseCase`, `GetCurrentUserUseCase`) were generated as working implementations rather than as
compile-only stubs, so most of their tests passed at the first run:

```text
dotnet test tests\unit\BallastLane.Domain.Tests
Failed!  - Failed: 1, Passed: 13, Skipped: 0, Total: 14

dotnet test tests\unit\BallastLane.Application.Tests
Failed!  - Failed: 1, Passed: 12, Skipped: 0, Total: 13
```

That is not a valid RED checkpoint. The implementations were backed up, replaced with stubs that
returned or accepted wrong values, and the tests were re-run to observe genuine assertion failures
before the implementations were restored. See "Commands and observed results" below.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Controllers bind, delegate to one use case, and map the outcome; `NpgsqlUserRepository` implements application-owned boundaries; the token issuer and the validator share `TokenContract` in the application layer so `Infrastructure` need not reference `SharedWeb`, which the compiler rejected at `error CS0103: The name 'SharedWeb' does not exist` | `[VERIFIED]` |
| SOLID | `IUserReader` and `IUserWriter` remain separate rather than one combined interface; `UserValidator` owns the rules; the token contract has one definition consumed by issuer and validator | `[VERIFIED]` |
| TDD | Every US1 behavior was observed failing before implementation: 13 domain, 13 application, 13 HTTP, and 9 frontend failures. The unit-layer RED was re-established after the deviation described above | `[VERIFIED]` |
| Security | Passwords hashed through `PasswordHasher<T>`; no response contains a password or hash (asserted in HTTP tests); duplicate email returns `409`; wrong password and unknown email return the same generic `401`; `sub`/`email` claims are the only identity source; tokens carry a 60-minute lifetime and a fresh `jti` | `[VERIFIED]` |
| Functional requirements | FR-001, FR-002, FR-003, FR-004, FR-015 and NFR-001 verified by unit, HTTP, and frontend tests | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` Data-annotation validation produced a `ValidationProblemDetails` keyed `Email` and
  `Password`, which does not match the lowercase field keys the contract documents and the frontend
  binds. The validation now lives only in `UserValidator`, so one shape exists.
- `[VERIFIED]` The default JWT challenge wrote an empty body with no content type, so a `401` was not
  a ProblemDetails. A `JwtBearerEvents.OnChallenge` handler now writes `application/problem+json`.
  `Response.WriteAsJsonAsync` forced `application/json`, so the handler serializes the body itself.
- `[VERIFIED]` Two `401` login bodies differ only by their correlation identifier. The requirement is
  that the two causes are indistinguishable, so the test now compares the meaningful fields (type,
  title, status, detail, errors) and the record states that the correlation id differs per request.
- `[VERIFIED]` The first `AuthProvider` could never authenticate, and the first `LoginPage`
  re-authenticated with empty credentials. The provider now records the session from the login
  result, and the page routes the form's login through the context.
- `[VERIFIED]` A registration test matched both the password hint and the password error. The hint
  text was changed to "Minimum length 8 characters." so the assertion is unambiguous.
- `[VERIFIED]` One HTTP run failed six tests with `NpgsqlException: The operation has timed out`
  during connect. The container reported `healthy` and used 6 of 100 connections. The same suite
  passed 13/13 on an immediate re-run with no code change, so the failure was environmental and is
  recorded rather than hidden.
- `[PENDING]` `GET /api/users/me` and the anonymous-access cases are also specified by US2 (T038).
  The US1 suite covers only the two cases it lists; the full boundary matrix arrives with US2.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Field validation lives only in `UserValidator` | One error shape with the documented field keys | `[AI-PROPOSED]` |
| Correlation id kept out of the "identical body" comparison | A per-request trace identifier is required by the error-shape contract and cannot be identical between requests | `[AI-PROPOSED]` |
| Bearer token held in memory only | Avoids persisting a credential in browser storage; a reload requires a fresh login | `[AI-PROPOSED]` |
| Re-establish the unit-layer RED by stubbing and re-running | The first attempt was not a valid RED checkpoint, and presenting it as one would be false evidence | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Removed `[Required]` from `RegisterRequest` and `LoginRequest` | Two competing validation sources produced inconsistent field keys | `api-standards.md` requires one documented error shape; the application validator is the single source |
| Added `JwtBearerEvents.OnChallenge`/`OnForbidden` writing ProblemDetails | `401` responses had no body and no content type | `security.md` requires every error to be a documented, machine-readable shape |
| Moved issuer/audience/lifetime into `Application.Abstractions.TokenContract` | `Infrastructure` referenced `SharedWeb`, which the compiler rejected and the architecture test forbids | One definition consumed by both sides without an inward-violating reference |
| Rewrote `AuthProvider` and `LoginPage` | The first draft could not establish a session | The login result must be the single place a session is recorded |
| Changed the password hint wording | Two DOM nodes matched the test's text query | The hint and the error must be distinguishable |
| Changed the login `401` assertion to compare meaningful fields | Correlation ids legitimately differ per request | The requirement is indistinguishability of causes, not byte equality |

## Commands and observed results

### RED - frontend (T029)

```text
cd frontend; npx vitest run
Test Files  3 failed (3)
     Tests  9 failed (9)
```

### RED - HTTP (T029)

```text
dotnet test tests\http\BallastLane.UserApi.HttpTests --no-build
Failed!  - Failed: 13, Passed: 0, Skipped: 0, Total: 13
```

### RED - unit layers, first attempt (invalid)

```text
dotnet test tests\unit\BallastLane.Domain.Tests
Failed!  - Failed: 1, Passed: 13, Skipped: 0, Total: 14

dotnet test tests\unit\BallastLane.Application.Tests
Failed!  - Failed: 1, Passed: 12, Skipped: 0, Total: 13
```

### RED - unit layers after the corrective stubs

```text
dotnet test tests\unit\BallastLane.Domain.Tests
Failed!  - Failed: 13, Passed: 1, Skipped: 0, Total: 14

dotnet test tests\unit\BallastLane.Application.Tests
Failed!  - Failed: 12, Passed: 1, Skipped: 0, Total: 13

Assert.Equal() Failure: Values differ
Expected: Conflict
Assert.Equal() Failure: Values differ
Expected: ValidationFailed
Assert.Equal() Failure: Values differ
Expected: Unauthenticated
Assert.Single() Failure: The collection was empty
```

The infrastructure adapters (`NpgsqlUserRepository`, `JwtTokenIssuer`, `UsersController`,
`PublicController`) were created as `NotImplementedException` / `501` stubs for the same checkpoint.

### GREEN - US1

```text
dotnet build BallastLane.sln
Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test tests\unit\BallastLane.Domain.Tests        -> Passed: 14
dotnet test tests\unit\BallastLane.Application.Tests   -> Passed: 13
dotnet test tests\http\BallastLane.UserApi.HttpTests   -> Passed: 13

cd frontend; npx vitest run
Test Files  3 passed (3)
     Tests  9 passed (9)

cd frontend; npm run build   -> tsc --noEmit && vite build, built in 2.48s
cd frontend; npm run lint    -> no findings
```

### Full backend suite after US1

```text
Passed! - Failed: 0, Passed: 13, Total: 13   BallastLane.Application.Tests.dll
Passed! - Failed: 0, Passed: 18, Total: 18   BallastLane.Api.UnitTests.dll
Passed! - Failed: 0, Passed: 14, Total: 14   BallastLane.Domain.Tests.dll
Passed! - Failed: 0, Passed:  2, Total:  2   BallastLane.PostgreSql.Tests.dll
Passed! - Failed: 0, Passed:  9, Total:  9   BallastLane.Infrastructure.UnitTests.dll
Passed! - Failed: 0, Passed:  9, Total:  9   BallastLane.Architecture.Tests.dll
Passed! - Failed: 0, Passed: 13, Total: 13   BallastLane.UserApi.HttpTests.dll
Passed! - Failed: 0, Passed:  6, Total:  6   BallastLane.Migrations.Tests.dll
```

## Final references

- Implementation files: `src/Domain/Users/{Email,UserId,User,AuthenticatedUser}.cs`,
  `src/Application/Users/{UserValidator,RegisterUserUseCase,LoginUseCase,GetCurrentUserUseCase}.cs`,
  `src/Application/Abstractions/ITokenIssuer.cs`,
  `src/Infrastructure/Persistence/NpgsqlUserRepository.cs`,
  `src/Infrastructure/Security/JwtTokenIssuer.cs`,
  `src/SharedWeb/{AuthenticationSetup,ClaimsPrincipalExtensions,OperationResultMapping}.cs`,
  `src/UserApi/{Program.cs,Controllers/UsersController.cs,Controllers/PublicController.cs,Contracts/*.cs}`,
  `frontend/src/api/{client,users,types,validation}.ts`,
  `frontend/src/auth/{AuthContext.ts,AuthProvider.tsx,useAuth.ts,RequireAuth.tsx}`,
  `frontend/src/components/{RegisterForm,LoginForm}.tsx`,
  `frontend/src/pages/{RegisterPage,LoginPage}.tsx`, `frontend/src/App.tsx`
- Test files: `tests/unit/BallastLane.Domain.Tests/UserTests.cs`,
  `tests/unit/BallastLane.Application.Tests/{RegisterUserUseCaseTests,LoginUseCaseTests,TestDoubles}.cs`,
  `tests/http/BallastLane.UserApi.HttpTests/{UserApiFactory,TestDatabase,HttpAssertions,RegistrationTests,LoginTests}.cs`,
  `frontend/tests/components/{RegisterForm,LoginForm,auth-context}.test.tsx`
- Task identifiers: `T025` to `T037`
- Commits: `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires.

## Evidence status rationale

Status is `Captured`: the prompt, the original generated excerpts, the observed RED for every US1
suite (including the corrective re-run after the process deviation), the review findings, and the
observed GREEN output are recorded. The record is not `Reviewed` or `Verified` because the developer
has not reviewed it and no commit exists yet.
