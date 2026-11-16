# EV-0014 - User Story 4: create a task (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0014` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 4, T058 to T069) |
| Activity type | `Code generation` |
| Related specification | `specs/001-task-manager/spec.md` (US4, FR-009, FR-013, FR-014, PDC-01/02/03) |
| Related task identifiers | `T058` to `T069` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `PENDING` |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` T058 to T069 and `contracts/task-api.md` (POST /api/tasks) defined the behavior.

## Relevant context

- Requirements: US4 acceptance scenarios 1 to 5, FR-009, FR-013, FR-014, PDC-02, PDC-03.
- Standards: `database-standards.md`, `api-standards.md`, `security.md` 2.3, `frontend-standards.md`.
- Preceding record: `EV-0013`.

## Original AI-generated output

Compile-only contracts, preserved before correction:

```csharp
// src/Domain/Tasks/TaskDraft.cs - original stub
public static bool TryCreate(
    string? title, string? description, string? status, DateOnly dueDate, out TaskDraft draft)
{
    draft = new TaskDraft();
    return true;
}
```

```csharp
// src/Application/Tasks/TaskValidator.cs - original stub
public IReadOnlyList<FieldError> Validate(TaskInput input) => [];
```

```csharp
// src/Application/Tasks/CreateTaskUseCase.cs - original stub (returns a wrong success)
public Task<OperationResult<TaskItem>> ExecuteAsync(
    AuthenticatedUser identity, TaskInput input, CancellationToken cancellationToken) =>
    Task.FromResult(OperationResult<TaskItem>.Success(
        new TaskItem(default, identity.UserId, string.Empty, null, TaskStatus.Pending, default, default, default)));
```

```tsx
// frontend/src/components/TaskForm.tsx - original stub
export function TaskForm(_props: TaskFormProps) {
  return <form />;
}
```

`ITaskWriter` was first generated with `AddAsync`, `UpdateAsync`, and `DeleteAsync`. That made the
interface demand behavior belonging to US5 and US6, so the implementation would have preceded those
stories' RED checkpoints (and the US4 test fake could not compile). The interface was narrowed to
`AddAsync`; the other members arrive with their own stories.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `TaskDraft` (domain) owns the rules; `TaskValidator` (application) owns shape errors; the controller binds, delegates, and maps | `[VERIFIED]` |
| SOLID | `ITaskWriter` is narrowed to the operation that exists; `TaskDraft` and `TaskInput` carry no owner, making the FR-009 rule a type-level fact | `[VERIFIED]` |
| TDD | RED observed: 11 domain, 10 application, 11 HTTP, and 4 frontend failures, all assertion failures | `[VERIFIED]` |
| Security | A supplied `ownerId`/`userId` is ignored; the stored owner equals the token subject (asserted by reading `owner_id` from the database); creation without a token persists nothing | `[VERIFIED]` |
| Functional requirements | FR-009, FR-013, FR-014 verified for create, including 200/201-character title bounds, 2000/2001-character description bounds, `Done` rejection, and past due dates | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` `CreatedAtAction` produced an absolute `Location` (`http://localhost/api/tasks/...`),
  which the contract documents as `/api/tasks/{id}` and which would be wrong behind a port mapping.
  `Created($"/api/tasks/{id}", body)` now emits the relative form.
- `[VERIFIED]` A native `type="date"` input cannot hold a non-calendar value in a real browser or in
  jsdom, so the client "not YYYY-MM-DD" branch is unreachable through typing. The test now asserts the
  reachable invalid case (an empty date) and the format rule stays in code for non-native clients.
  This is a deliberate deviation from the wording of T060, recorded rather than hidden.
- `[VERIFIED]` `TaskDraft.TryCreate` rejects a title longer than 200 characters and a description
  longer than 2,000, and trims both, so the database `CHECK` constraints are a second line of defence
  rather than the only one.
- `[VERIFIED]` Status parsing is case-sensitive, so `completed` is rejected while `Completed` is
  accepted, matching the API's exact vocabulary.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Narrow `ITaskWriter` to `AddAsync` | Keeps update and delete behind their own RED checkpoints | `[AI-PROPOSED]` |
| Relative `Location` header | Correct behind any proxy or port mapping; matches the contract | `[AI-PROPOSED]` |
| Native date input plus format validation | The control prevents malformed dates; the rule still guards non-native clients | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| `CreatedAtAction` replaced with `Created(relativeUrl, body)` | Absolute Location leaked the internal host | Contract compliance and proxy safety |
| `ITaskWriter` narrowed to `AddAsync` | Pre-implementing later stories breaks RED ordering | `testing-and-tdd.md` 3.3 |
| Frontend date test re-targeted to the reachable case | The previous assertion covered unreachable code | A test must fail for a behavior that can actually regress |

## Commands and observed results

### RED

```text
dotnet test tests\unit\BallastLane.Domain.Tests            -> Failed: 11, Passed: 20
dotnet test tests\unit\BallastLane.Application.Tests       -> Failed: 10, Passed: 19
dotnet test tests\http\BallastLane.TaskApi.HttpTests       -> Failed: 11, Passed: 16

cd frontend; npx vitest run
Test Files  1 failed | 5 passed (6)
     Tests  4 failed | 15 passed (19)
```

### GREEN

```text
dotnet build BallastLane.sln -> Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test BallastLane.sln
BallastLane.Domain.Tests             31
BallastLane.Application.Tests        29
BallastLane.Infrastructure.UnitTests 18
BallastLane.Api.UnitTests            18
BallastLane.Architecture.Tests        9
BallastLane.PostgreSql.Tests          7
BallastLane.TaskApi.HttpTests        27
BallastLane.Migrations.Tests          6
BallastLane.UserApi.HttpTests        22
                                     ---
                                     167 passed, 0 failed

cd frontend; npx vitest run -> Test Files 6 passed, Tests 19 passed
cd frontend; npm run build  -> built in 2.04s
cd frontend; npm run lint   -> no findings
```

## Final references

- Implementation files: `src/Domain/Tasks/{TaskDraft,TaskItem}.cs`,
  `src/Application/Tasks/{TaskValidator,CreateTaskUseCase}.cs`,
  `src/Application/Abstractions/ITaskWriter.cs`,
  `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` (`AddAsync`),
  `src/TaskApi/{Controllers/TasksController.cs,Contracts/CreateTaskRequest.cs,Program.cs}`,
  `frontend/src/components/TaskForm.tsx`, `frontend/src/pages/TaskFormPage.tsx`,
  `frontend/src/state/tasksReducer.ts`, `frontend/src/App.tsx`
- Test files: `tests/unit/BallastLane.Domain.Tests/TaskValidationTests.cs`,
  `tests/unit/BallastLane.Application.Tests/CreateTaskUseCaseTests.cs`,
  `tests/http/BallastLane.TaskApi.HttpTests/TaskCreateTests.cs`,
  `frontend/tests/components/TaskFormCreate.test.tsx`
- Task identifiers: `T058` to `T069`
- Commits: `PENDING`

## Evidence status rationale

Status is `Captured`. Developer review and a commit reference remain outstanding.
