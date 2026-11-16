# EV-0012 - User Story 2: public and protected API access (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0012` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 2, T038 to T044) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US2, FR-008, FR-009, NFR-001, SC-004) |
| Related task identifiers | `T038` to `T044` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires. |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

No further developer text was supplied. `tasks.md` T038 to T044 defined the required behavior and
`contracts/task-api.md` defined the protection rule for the second host.

## Relevant context

- Requirements: US2 acceptance scenarios 1 to 5, FR-008, FR-009, NFR-001, SC-004.
- Standards: `security.md` (2.2, 2.3, 5), `api-standards.md`, `testing-and-tdd.md`.
- Preceding records: `EV-0010` (foundational authentication wiring), `EV-0011` (US1).

## Original AI-generated output

The first generated Task API host registered authentication but the probe routes and the
default-deny policy were the only boundary; the first US2 test run passed everything on the first
attempt, which is not a RED checkpoint. The protection was therefore temporarily removed to observe
the failure it prevents.

Defect A - the default-deny fallback policy removed:

```csharp
services.AddAuthorization();
```

(instead of `AddAuthorizationBuilder().SetFallbackPolicy(... RequireAuthenticatedUser() ...)`)

Defect B - identity taken from the query string instead of validated claims:

```csharp
var identity = new BallastLane.Domain.Users.AuthenticatedUser(
    new BallastLane.Domain.Users.UserId(Guid.TryParse(Request.Query["userId"], out var supplied) ? supplied : Guid.NewGuid()),
    Request.Query["email"].ToString());
```

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | Both hosts share `AuthenticationSetup` from `SharedWeb`; controllers contain no security decision beyond reading the claim-derived identity | `[VERIFIED]` |
| SOLID | The probe routes exist to make the boundary observable; they carry no business rule | `[VERIFIED]` |
| TDD | RED observed by removing the two protections: 6 of 7 Task API and 1 of 9 User API boundary tests failed, then returned to green after restoring them | `[VERIFIED]` |
| Security | Fallback policy denies anonymous access to any route without an explicit attribute; missing, malformed, bad-signature, and expired tokens all return `401` ProblemDetails; identity derives from `sub` and `email` claims only | `[VERIFIED]` |
| Functional requirements | FR-008 (two surfaces with public and protected endpoints) and FR-009 (identity never from client input) verified on both hosts | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` The undecorated probe route returns `200` when the fallback policy is absent, and `401`
  when it is present, which is the behavior FR-008 depends on.
- `[VERIFIED]` Supplying `userId`/`ownerId` in the query string cannot change the identity the host
  reports; the claim-derived path ignores it.
- `[VERIFIED]` Both hosts declared `public partial class Program`, so the Task API HTTP test project
  could not reference both hosts (`error CS0433: The type 'Program' exists in both`). Distinct marker
  types (`UserApiEntryPoint`, `TaskApiEntryPoint`) replaced it.
- `[VERIFIED]` The Task API HTTP test project references the User API project so a later story can
  obtain a token from the real issuer. That reference is what made the duplicate `Program` visible.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Distinct entry-point marker types per host | Two assemblies cannot both expose a global `Program` to the same test project | `[AI-PROPOSED]` |
| Duplicate the small database/host helpers in the two HTTP test projects | The plan defines no shared test-support project; duplication stays inside test code | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Replaced `Program` with per-host marker types | Duplicate global type across two referenced hosts | `WebApplicationFactory<T>` needs a type in the target assembly, not specifically `Program` |
| Re-established RED by removing the fallback policy and claim mapping | The first run passed on a first attempt, which proves nothing about the tests | A test that passes before the protection exists does not exercise it |

## Commands and observed results

### RED with the protections removed

```text
dotnet test tests\http\BallastLane.TaskApi.HttpTests
Failed!  - Failed: 6, Passed: 1, Skipped: 0, Total: 7
  A_route_without_an_explicit_authorization_attribute_is_denied_by_the_fallback_policy [FAIL]
  A_protected_route_rejects_every_invalid_credential_with_401(missing|malformed|expired) [FAIL]
  Identity_comes_from_the_token_rather_than_a_query_or_body_value [FAIL]

dotnet test tests\http\BallastLane.UserApi.HttpTests --filter AuthenticationBoundaryTests
Failed!  - Failed: 1, Passed: 8, Skipped: 0, Total: 9
  A_route_without_an_explicit_authorization_attribute_is_denied_by_the_fallback_policy [FAIL]
```

### GREEN after restoring the protections

```text
dotnet build BallastLane.sln   -> Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test tests\http\BallastLane.TaskApi.HttpTests
Passed!  - Failed: 0, Passed: 7, Skipped: 0, Total: 7

dotnet test tests\http\BallastLane.UserApi.HttpTests
Passed!  - Failed: 0, Passed: 22, Skipped: 0, Total: 22
```

## Final references

- Implementation files: `src/SharedWeb/{AuthenticationSetup,ClaimsPrincipalExtensions}.cs`,
  `src/TaskApi/{Program.cs,Controllers/ProbeController.cs}`,
  `src/UserApi/Controllers/ProbeController.cs`
- Test files: `tests/http/BallastLane.TaskApi.HttpTests/{TaskApiFactory,TestDatabase,HttpAssertions,AccessBoundaryTests}.cs`,
  `tests/http/BallastLane.UserApi.HttpTests/AuthenticationBoundaryTests.cs`
- Task identifiers: `T038` to `T044`
- Commits: `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires.

## Evidence status rationale

Status is `Captured`. The RED demonstration required removing the two protections the story exists
to guarantee; that is recorded explicitly so the cycle is not mistaken for a first-attempt pass.
Developer review and a commit reference are outstanding.
