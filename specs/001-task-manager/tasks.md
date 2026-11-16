---

description: "Task list for 001-task-manager (RED-first, dependency-ordered)"
---

# Tasks: Personal Task Manager (001-task-manager)

**Input**: Design documents from `specs/001-task-manager/`

**Prerequisites**: [plan.md](./plan.md) (required), [spec.md](./spec.md) (required), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md)

**Project override** of the installed tasks template. Governed by
`.specify/memory/constitution.md` Principle III.

**Tests are MANDATORY.** Every new or changed runtime behavior has test tasks that precede its
implementation task. Testing is not a final project-wide phase, and no test is omitted because the
specification did not ask for it.

## PRECONDITION - analysis gate

**No task in this file may be executed until `/speckit-analyze` has been run against the current
`spec.md`, `plan.md`, and this `tasks.md`, and its report contains no unresolved blocking findings.**
That run validates the *planned* RED ordering below. During execution, each implementation task must
also *observe* the actual behavioral failure before the production behavior is written.

## TDD Task Pattern (mandatory)

For each independently testable behavior, or coherent group of related behaviors, the tasks appear
in this order:

1. Write the relevant tests.
2. Run them and observe the expected RED failure.
3. Implement the minimum behavior required.
4. Run them and observe GREEN.
5. Refactor if needed, then rerun.
6. Record TDD and GenAI evidence for the activity.

Rules that hold in this list:

- Every implementation task traces to an observed RED checkpoint. The first implementation task in a
  group depends directly on the RED task; later tasks in the same group name the RED checkpoint they
  inherit, so no implementation task has an unexplained dependency.
- A test task and its corresponding implementation task never both carry `[P]`, and are never
  scheduled in parallel.
- Evidence capture appears in the same activity as the work, not collected at the end.
- Each user story carries its own test-first tasks.

Foundational tasks that change runtime behavior - authentication and authorization, database
migrations, persistence, error handling, and seed behavior - follow the same RED-first pattern. In
this file a RED checkpoint is a **planned dependency**; the actual behavioral failure is observed
during implementation, before the production behavior it covers is written.

Only non-behavioral scaffolding and configuration tasks are marked `Not Applicable` for TDD, each
with a written justification and a substitute validation task. A task that changes runtime behavior
is never eligible for `Not Applicable`.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: can run in parallel - different files, no dependency on an incomplete task.
- **[Story]**: the user story the task belongs to (US1 to US7). Absent for Setup, Foundational, and
  Polish.
- Every description names an exact file path: source file, test file, or evidence record.

## Path Conventions

Repository root, per [plan.md](./plan.md) Project Structure:

- Backend: `src/{Domain,Application,Infrastructure,SharedWeb,UserApi,TaskApi,Migrator}/`
- Unit tests: `tests/unit/...`, integration: `tests/integration/...`, HTTP: `tests/http/...`
- Frontend: `frontend/src/...`, `frontend/tests/...`
- Database: `db/migrations/`
- Evidence: `docs/evidence/genai/EV-00nn-*.md`
- Root: `BallastLane.sln`, `docker-compose.yml`, `.env.example`

> **Note (2026-11-16, after task execution).** The paths above are the layout in effect when T001 to
> T107 were executed, and this file is kept as the record of that execution rather than rewritten.
> A later restructure, recorded in
> [EV-0024](../../docs/evidence/genai/EV-0024-backend-folder-restructure.md), moved the backend to
> `backend/src/`, `backend/tests/`, and `backend/db/`; only the frontend path, the evidence path, and
> the root files above are unchanged. Read every `src/`, `tests/`, and `db/` path in the task list
> below with that `backend/` prefix.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization, tooling, and configuration. These tasks create no runtime
behavior, so they are `Not Applicable` for TDD with the substitute validation named in T007.

- [X] T001 Create `BallastLane.sln` with `src/Domain/Domain.csproj`, `src/Application/Application.csproj`, `src/Infrastructure/Infrastructure.csproj`, `src/SharedWeb/SharedWeb.csproj`, `src/UserApi/UserApi.csproj`, `src/TaskApi/TaskApi.csproj`, and `src/Migrator/Migrator.csproj`, all targeting `net10.0`, with only the permitted references: Domain none; Application to Domain; Infrastructure to Application and Domain; SharedWeb to Application and Domain; UserApi and TaskApi to Application, Domain, Infrastructure, and SharedWeb; Migrator to Infrastructure. TDD Not Applicable: project scaffolding only. Substitute validation: T007
- [X] T002 [P] Configure `Directory.Build.props` with `Nullable=enable`, `TreatWarningsAsErrors=true`, and `LangVersion=14`; add package references aligned with the .NET 10 line - `Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.x, `Microsoft.Extensions.Identity.Core` 10.0.x, `Microsoft.AspNetCore.Mvc.Testing` 10.0.x, `Npgsql` 10.x, `Swashbuckle.AspNetCore` latest stable targeting `net10.0`, and `xunit` 2.x with the current stable `Microsoft.NET.Test.Sdk` - to `src/Infrastructure/Infrastructure.csproj`, `src/UserApi/UserApi.csproj`, `src/TaskApi/TaskApi.csproj`, and the test projects; record the resolved versions in `docs/evidence/genai/EV-0010-foundational-tdd.md` and fail the build on any package that resolves below the .NET 10 line or pulls a prohibited library. TDD Not Applicable: build configuration only. Substitute validation: T007
- [X] T003 [P] Create the frontend project in `frontend/package.json` and `frontend/tsconfig.json` with Vite 5.4, React 18.3, TypeScript 5.6 (`strict`, `noImplicitAny`, `strictNullChecks`, `noUnusedLocals`), React Router 6, Vitest 2, and React Testing Library 16. TDD Not Applicable: scaffolding and configuration only. Substitute validation: T007
- [X] T004 [P] Configure the seven test projects `tests/unit/BallastLane.Domain.Tests`, `tests/unit/BallastLane.Application.Tests`, `tests/unit/BallastLane.Infrastructure.UnitTests`, `tests/unit/BallastLane.Api.UnitTests`, `tests/unit/BallastLane.Architecture.Tests`, `tests/integration/BallastLane.PostgreSql.Tests`, `tests/integration/BallastLane.Migrations.Tests`, `tests/http/BallastLane.UserApi.HttpTests`, and `tests/http/BallastLane.TaskApi.HttpTests` with xUnit 2.x, `Microsoft.AspNetCore.Mvc.Testing` 8.x for the two HTTP projects, and the shared fixture `tests/integration/BallastLane.PostgreSql.Tests/DatabaseFixture.cs`. TDD Not Applicable: test harness scaffolding only. Substitute validation: T007
- [X] T005 [P] Create `docker-compose.yml` with a `postgres:16` service on `5432` carrying a `pg_isready` health check, `db/docker-entrypoint-initdb.d/01-create-test-database.sql` creating `taskmanager_test`, and `migrator`, `user-api` on `5081`, and `task-api` on `5082` services that depend on `postgres` being healthy. TDD Not Applicable: deployment configuration only. Substitute validation: T007
- [X] T006 [P] Create `.env.example` with commented placeholders for `POSTGRES_PASSWORD`, `ConnectionStrings__Database`, `ConnectionStrings__TestDatabase`, `Jwt__SigningKey`, and `FrontendOrigin`, and confirm `.env` is covered by `.gitignore`. TDD Not Applicable: configuration only. Substitute validation: T007
- [X] T007 Add `.editorconfig`, `frontend/.prettierrc`, and `frontend/eslint.config.js`; then run the substitute validation for T001 to T006: `dotnet build BallastLane.sln` with no warnings, `docker compose -f docker-compose.yml config` parsing cleanly, `docker compose up -d postgres` reaching a healthy status, and `npm run build` plus `npm run lint` in `frontend/`; record all observed output in `docs/evidence/genai/EV-0009-task-generation.md` (depends on T001, T002, T003, T004, T005, T006)

**Checkpoint**: The solution builds, the frontend builds and lints, the database container is
healthy, and no runtime behavior exists yet.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Schema, connection handling, error mapping, authentication wiring, and the idempotent
seed. All of it changes runtime behavior, so all of it is RED-first.

**CRITICAL**: No user story work begins until this phase completes.

### Tests for Phase 2 (MANDATORY)

- [X] T008 [P] Write migration tests asserting that applying `db/migrations/` to an empty database yields the `users` and `tasks` tables, the unique index on `lower(email)`, `ix_tasks_owner_due_created`, `CHECK (length(btrim(title)) BETWEEN 1 AND 200)`, `CHECK (description IS NULL OR length(description) <= 2000)`, `CHECK (status IN ('Pending','InProgress','Completed'))`, the `ON DELETE CASCADE` foreign key, and `schema_migrations`, in `tests/integration/BallastLane.Migrations.Tests/MigrationApplicationTests.cs`
- [X] T009 [P] Write tests asserting the migration runner applies files in filename order, records each version once, skips already-applied versions on re-run, and fails on an unreadable migration, in `tests/unit/BallastLane.Infrastructure.UnitTests/SchemaMigratorTests.cs`
- [X] T010 [P] Write tests asserting the connection factory reads `ConnectionStrings__Database`, returns an un-opened `NpgsqlConnection`, and forwards a `CancellationToken` to the command it creates, in `tests/unit/BallastLane.Infrastructure.UnitTests/NpgsqlConnectionFactoryTests.cs`
- [X] T011 [P] Write tests asserting each application outcome maps to the documented status and ProblemDetails shape - 400 shape validation with an `errors` object keyed by field, 401 authentication, 404 missing-or-not-owned, 409 conflict - and that no mapped body contains a stack trace, SQL text, or driver message, in `tests/unit/BallastLane.Api.UnitTests/OperationResultMappingTests.cs`
- [X] T012 [P] Write unit tests asserting the explicit token validation parameters built by `AuthenticationSetup` - `ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey`, and `ValidateLifetime` all true, `ClockSkew` of `TimeSpan.Zero`, the issuer `ballastlane-user-api`, and the audience `ballastlane-clients` - and that a token signed with a foreign key, one with a past `exp`, and a malformed token each fail validation while a correctly issued token validates, in `tests/unit/BallastLane.Api.UnitTests/TokenValidationParametersTests.cs`
- [X] T013 [P] Write tests asserting the seed is idempotent - two runs yield identical user and task counts with no duplicate email and no error - and that every seeded password verifies against its stored hash through `IPasswordHasher`, in `tests/integration/BallastLane.Migrations.Tests/DemoSeedTests.cs`

### RED checkpoint for Phase 2

- [X] T014 Create the compile-only contracts required by the Phase 2 tests - `IPasswordHasher`, `ITimeProvider`, `IUserReader`, `IUserWriter`, and `ITokenIssuer` in `src/Application/Abstractions/`, `src/Domain/Users/AuthenticatedUser.cs`, the outcome types in `src/Application/Outcomes/OperationResult.cs`, and `NotImplementedException` bodies for `src/Infrastructure/Persistence/SchemaMigrator.cs`, `src/Infrastructure/Persistence/NpgsqlConnectionFactory.cs`, `src/Infrastructure/Security/IdentityPasswordHasher.cs`, and `src/Infrastructure/Persistence/DemoSeeder.cs` - then run the Phase 2 tests in `tests/unit/BallastLane.Infrastructure.UnitTests/`, `tests/unit/BallastLane.Api.UnitTests/`, and `tests/integration/BallastLane.Migrations.Tests/`, and record each observed RED failure in `docs/evidence/genai/EV-0010-foundational-tdd.md`, confirming every failure is a behavioural assertion failure rather than a compilation or environment failure (depends on T008, T009, T010, T011, T012, T013)

### Implementation for Phase 2

- [X] T015 Implement `db/migrations/0001_create_users.sql` and `db/migrations/0002_create_tasks.sql` with the exact columns, constraints, defaults, and indexes in [data-model.md](./data-model.md), including `status text NOT NULL DEFAULT 'Pending'` and `due_date date NOT NULL` (depends on T014 RED)
- [X] T016 Implement `src/Infrastructure/Persistence/SchemaMigrator.cs` and `src/Infrastructure/Persistence/NpgsqlConnectionFactory.cs` with parameterized SQL only, `await using` disposal, and async calls carrying the token (depends on T015; RED via T014)
- [X] T017 Implement `src/Application/Outcomes/OperationResult.cs` and `src/SharedWeb/OperationResultMapping.cs` producing the ProblemDetails shape asserted in `tests/unit/BallastLane.Api.UnitTests/OperationResultMappingTests.cs` (depends on T016; RED via T014)
- [X] T018 Implement `src/SharedWeb/AuthenticationSetup.cs` with explicit `TokenValidationParameters` (`ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey`, `ValidateLifetime` all true, `ClockSkew = TimeSpan.Zero`, issuer `ballastlane-user-api`, audience `ballastlane-clients`, HS256 key from `Jwt__SigningKey`) and a default-deny fallback policy, plus claim mapping in `src/SharedWeb/ClaimsPrincipalExtensions.cs` (depends on T017; RED via T014)
- [X] T019 Implement the concrete adapters `src/Infrastructure/Security/IdentityPasswordHasher.cs` (established `PasswordHasher<T>`, unique salt, constant-time verification) and `src/Infrastructure/Time/SystemTimeProvider.cs`, so password hashing exists before the seeder consumes it (depends on T018; RED via T014)
- [X] T020 Implement `src/Infrastructure/Persistence/DemoSeeder.cs` with idempotent upserts using `ON CONFLICT (lower(email)) DO NOTHING`, matching the unique expression index created by T015 rather than a non-existent plain `email` constraint, hashing seeded passwords through the `IdentityPasswordHasher` implemented in T019 inside one explicit transaction; then implement `src/Migrator/Program.cs` with a `migrate` default action that applies pending migrations and a `--seed` action that applies migrations first and only then seeds, returning a non-zero exit code on failure (depends on T019; RED via T014)
- [X] T021 Author `tests/unit/BallastLane.Architecture.Tests/DependencyDirectionTests.cs` asserting the permitted project references, that `Domain` and `Application` reference no ASP.NET Core or Npgsql assembly, and that no `EntityFrameworkCore`, `Dapper`, `MediatR`, or `Mediator` package is referenced directly or transitively; establish its RED by temporarily adding a prohibited reference to `src/Application/Application.csproj`, observing and recording the failure, then reverting
- [X] T022 Run the Phase 2 suite across `tests/unit/` and `tests/integration/` and record the observed GREEN results in `docs/evidence/genai/EV-0010-foundational-tdd.md` (depends on T020, T021)
- [X] T023 Refactor the Phase 2 code in `src/Infrastructure/Persistence/`, `src/SharedWeb/`, and `src/Migrator/`, rerun the suite, and record the result in `docs/evidence/genai/EV-0010-foundational-tdd.md` (depends on T022)
- [X] T024 Record the foundational TDD and GenAI evidence in `docs/evidence/genai/EV-0010-foundational-tdd.md`, including every observed RED and GREEN output (depends on T023)

**Checkpoint**: Schema, migrations, error mapping, authentication wiring, and the idempotent seed are
implemented and their tests pass against a real PostgreSQL instance. User story work can begin.

---

## Phase 3: User Story 1 - Register and Authenticate (Priority: P1) MVP

**Goal**: A visitor registers, a registered user authenticates, the frontend keeps and clears the
session, and user information persists.

**Independent Test**: register a new account, log out, log in again with the same credentials, and
verify duplicate-email, wrong-password, and invalid-input rejections - with no task functionality
present.

### Tests for User Story 1 (MANDATORY)

- [X] T025 [P] [US1] Write domain tests for email normalization to lower case, the single-`@` address shape, the 320-character limit, and rejection of an empty email, in `tests/unit/BallastLane.Domain.Tests/UserTests.cs`
- [X] T026 [P] [US1] Write application tests for `RegisterUserUseCase` (stores a hash and never the plaintext; duplicate email yields a conflict outcome; a 7-character password yields a validation outcome; an 8-character password is accepted) and `LoginUseCase` (valid credentials yield a token; unknown email and wrong password yield the same generic failure outcome), in `tests/unit/BallastLane.Application.Tests/RegisterUserUseCaseTests.cs` and `tests/unit/BallastLane.Application.Tests/LoginUseCaseTests.cs`
- [X] T027 [P] [US1] Write HTTP tests for `POST /api/users/register` (201 with no password or hash in the body; 409 for a duplicate email differing only in case; 400 for a malformed email, a missing password, and a 7-character password), `POST /api/users/login` (200 with `accessToken`, `tokenType`, and `expiresAtUtc`; 401 with an identical body for a wrong password and an unknown email; 400 for a missing field), and `GET /api/users/me` (200 with the caller's identity), in `tests/http/BallastLane.UserApi.HttpTests/RegistrationTests.cs` and `tests/http/BallastLane.UserApi.HttpTests/LoginTests.cs`
- [X] T028 [P] [US1] Write frontend tests asserting the registration form shows field-level validation for a malformed email and a short password, the login form shows a single generic message for bad credentials, logging in stores the session and renders authenticated content, logging out clears it, and a 401 response clears state and redirects to login, in `frontend/tests/components/RegisterForm.test.tsx`, `frontend/tests/components/LoginForm.test.tsx`, and `frontend/tests/auth-context.test.tsx`

### RED checkpoint for User Story 1

- [X] T029 [US1] Create the compile-only contracts this story's tests require - `src/Domain/Users/User.cs`, `src/Application/Users/{UserValidator,RegisterUserUseCase,LoginUseCase,GetCurrentUserUseCase}.cs`, `src/UserApi/Controllers/{UsersController,PublicController}.cs` with `NotImplementedException` bodies, and minimal `frontend/src/api/users.ts`, `frontend/src/auth/AuthContext.tsx`, and `frontend/src/pages/{RegisterPage,LoginPage}.tsx` - so the suite compiles, then run the US1 tests in `tests/unit/BallastLane.Domain.Tests/UserTests.cs`, `tests/unit/BallastLane.Application.Tests/`, `tests/http/BallastLane.UserApi.HttpTests/`, and `frontend/tests/`, and record each observed RED failure in `docs/evidence/genai/EV-0011-us1-registration-authentication.md`, confirming each is an assertion failure rather than a compilation failure (depends on T025, T026, T027, T028)

### Implementation for User Story 1

- [X] T030 [US1] Implement `src/Domain/Users/User.cs` with `UserId`, `Email`, and password-hash handling, including email normalization asserted by `tests/unit/BallastLane.Domain.Tests/UserTests.cs` (depends on T029 RED)
- [X] T031 [US1] Implement `src/Application/Users/UserValidator.cs`, `RegisterUserUseCase.cs`, `LoginUseCase.cs`, and `GetCurrentUserUseCase.cs` against the boundary interfaces declared in T014, applying the PDC-01 minimum of 8 characters (depends on T030; RED via T029)
- [X] T032 [US1] Implement `src/Infrastructure/Persistence/NpgsqlUserRepository.cs` (lookup by `lower(email)`, unique-violation translation to a conflict outcome) and `src/Infrastructure/Security/JwtTokenIssuer.cs` (60-minute lifetime, distinct `jti` per token), reusing the hasher and time provider from T019 (depends on T031; RED via T029)
- [X] T033 [US1] Implement `src/UserApi/Controllers/UsersController.cs` and `src/UserApi/Controllers/PublicController.cs` with explicit `[AllowAnonymous]` on register, login, and public info, plus `src/UserApi/Contracts/RegisterRequest.cs`, `LoginRequest.cs`, `UserResponse.cs`, and `LoginResponse.cs`, and dependency registration in `src/UserApi/Program.cs` (depends on T032; RED via T029)
- [X] T034 [US1] Implement `frontend/src/api/client.ts` with centralized 401 handling, `frontend/src/api/users.ts`, `frontend/src/auth/AuthContext.tsx`, `useAuth.ts`, `RequireAuth.tsx`, `frontend/src/pages/RegisterPage.tsx`, and `frontend/src/pages/LoginPage.tsx` with labelled inputs and field-associated error messages (depends on T033; RED via T029)
- [X] T035 [US1] Run the US1 tests in `tests/unit/`, `tests/http/BallastLane.UserApi.HttpTests/`, and `frontend/tests/components/` and record the observed GREEN results in `docs/evidence/genai/EV-0011-us1-registration-authentication.md` (depends on T034)
- [X] T036 [US1] Refactor US1 code in `src/Domain/Users/`, `src/Application/Users/`, `src/Infrastructure/Security/`, `src/UserApi/Controllers/`, and `frontend/src/auth/`, rerun the suite, and record the result in `docs/evidence/genai/EV-0011-us1-registration-authentication.md` (depends on T035)
- [X] T037 [US1] Record US1 TDD and GenAI evidence with the observed RED and GREEN output in `docs/evidence/genai/EV-0011-us1-registration-authentication.md` (depends on T036)

**Checkpoint**: US1 is functional and independently testable. Registration, login, logout, and
persistence work end to end with no task functionality present.

---

## Phase 4: User Story 2 - Public and Protected API Access (Priority: P1)

**Goal**: Two API surfaces with distinct access rules; identity always from validated claims.

**Independent Test**: with a registered account, call each public endpoint anonymously, call each
protected endpoint with no, malformed, and expired credentials, and call one with valid credentials.

### Tests for User Story 2 (MANDATORY)

- [X] T038 [P] [US2] Write HTTP tests asserting the User API public endpoints answer without credentials; `GET /api/public/info` returns 200 anonymously; `GET /api/users/me` returns 401 for a missing header, a malformed token, a bad-signature token, and an expired token, and returns 200 with the caller's own identity for a valid token; a probe route carrying no explicit authorization attribute is denied by the fallback policy; and an identity supplied in a request body or query cannot change whose data is returned, in `tests/http/BallastLane.UserApi.HttpTests/AuthenticationBoundaryTests.cs` and `tests/http/BallastLane.TaskApi.HttpTests/AccessBoundaryTests.cs`

### RED checkpoint for User Story 2

- [X] T039 [US2] Create the compile-only contracts this story's tests require - `src/TaskApi/Program.cs` and `src/TaskApi/Controllers/ProbeController.cs` (a route with no explicit authorization attribute, and a protected route) with `NotImplementedException` bodies - so the tests in `tests/http/BallastLane.TaskApi.HttpTests/AccessBoundaryTests.cs` compile, then run them and record the observed assertion-based RED failures in `docs/evidence/genai/EV-0012-us2-access-boundary.md` (depends on T038)

### Implementation for User Story 2

- [X] T040 [US2] Implement `src/TaskApi/Program.cs` fully with the shared authentication wiring, the default-deny fallback policy, the CORS allow-list from `FrontendOrigin`, and Swagger with a bearer security scheme (depends on T039 RED)
- [X] T041 [US2] Complete claim-derived identity handling so `AuthenticatedUser` is constructed only from the `sub` and `email` claims in `src/SharedWeb/ClaimsPrincipalExtensions.cs`, and declare the public or protected classification on every endpoint in `src/UserApi/Controllers/` and `src/TaskApi/Controllers/` (depends on T040; RED via T039)
- [X] T042 [US2] Run the US2 tests in `tests/http/BallastLane.TaskApi.HttpTests/AccessBoundaryTests.cs` and record the observed GREEN results in `docs/evidence/genai/EV-0012-us2-access-boundary.md` (depends on T041)
- [X] T043 [US2] Refactor US2 code in `src/TaskApi/Program.cs` and `src/SharedWeb/`, rerun, and record the result in `docs/evidence/genai/EV-0012-us2-access-boundary.md` (depends on T042)
- [X] T044 [US2] Record US2 TDD and GenAI evidence in `docs/evidence/genai/EV-0012-us2-access-boundary.md` (depends on T043)

**Checkpoint**: US1 and US2 both work independently; the access boundary is proven on both hosts.

---

## Phase 5: User Story 3 - List and View Personal Tasks (Priority: P2)

**Goal**: An authenticated user lists and opens their own tasks; other users' tasks are invisible; an
empty account shows an empty list.

**Independent Test**: with two accounts, one owning tasks inserted by a test fixture and one owning
none, list and retrieve for both and confirm isolation and the empty result.

### Tests for User Story 3 (MANDATORY)

- [X] T045 [P] [US3] Write Infrastructure unit tests asserting row mapping for each of the three statuses, a `null` description mapped to `null`, a `DateOnly` due date mapped from a `date` column, and that list and get commands filter by `owner_id` and order by `due_date, created_at, id`, in `tests/unit/BallastLane.Infrastructure.UnitTests/NpgsqlTaskRepositoryReadTests.cs`
- [X] T046 [P] [US3] Write PostgreSQL integration tests asserting an inserted task reads back with identical fields, that listing returns only the caller's rows in due-date order with the creation-order tie-break, and that an account with no tasks returns an empty list, in `tests/integration/BallastLane.PostgreSql.Tests/TaskReadTests.cs`
- [X] T047 [P] [US3] Write HTTP tests for `GET /api/tasks` (200 with only the caller's tasks; 200 with an empty array for a fresh account; 401 unauthenticated) and `GET /api/tasks/{id}` (200 for an owned task; 404 for another user's task; 404 for an unknown or non-UUID identifier; 401 unauthenticated), in `tests/http/BallastLane.TaskApi.HttpTests/TaskReadTests.cs`
- [X] T048 [P] [US3] Write frontend tests asserting the task list renders loading, empty, success, and error states, and that the detail view renders a task's fields, in `frontend/tests/components/TaskListStates.test.tsx` and `frontend/tests/components/TaskDetail.test.tsx`

### RED checkpoint for User Story 3

- [X] T049 [US3] Create the compile-only contracts this story's tests require - `src/Domain/Tasks/{TaskItem,TaskId,TaskStatus}.cs`, `src/Application/Tasks/{ListTasksUseCase,GetTaskUseCase}.cs`, `src/Application/Abstractions/ITaskReader.cs`, `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs`, `src/TaskApi/Controllers/TasksController.cs`, and minimal `frontend/src/api/tasks.ts`, `frontend/src/pages/{TasksPage,TaskDetailPage}.tsx`, and `frontend/src/components/{Spinner,EmptyState,ErrorMessage,TaskList}.tsx` with `NotImplementedException` bodies - so the suite compiles, then run the US3 tests in `tests/unit/BallastLane.Infrastructure.UnitTests/`, `tests/integration/BallastLane.PostgreSql.Tests/TaskReadTests.cs`, `tests/http/BallastLane.TaskApi.HttpTests/TaskReadTests.cs`, and `frontend/tests/components/`, and record each observed RED failure in `docs/evidence/genai/EV-0013-us3-task-read.md` (depends on T045, T046, T047, T048)

### Implementation for User Story 3

- [X] T050 [US3] Implement `src/Domain/Tasks/TaskItem.cs`, `src/Domain/Tasks/TaskId.cs`, and `src/Domain/Tasks/TaskStatus.cs` with exactly `Pending`, `InProgress`, and `Completed` (depends on T049 RED)
- [X] T051 [US3] Implement `src/Application/Abstractions/ITaskReader.cs`, `src/Application/Tasks/ListTasksUseCase.cs`, and `src/Application/Tasks/GetTaskUseCase.cs`, returning a not-found outcome for a missing or non-owned task rather than throwing (depends on T050; RED via T049)
- [X] T052 [US3] Implement the read methods of `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` with explicit column lists, no `SELECT *`, owner filtering, and the documented ordering (depends on T051; RED via T049)
- [X] T053 [US3] Implement the `GET` actions in `src/TaskApi/Controllers/TasksController.cs` and `src/TaskApi/Contracts/TaskResponse.cs` (depends on T052; RED via T049)
- [X] T054 [US3] Implement `frontend/src/api/tasks.ts`, `frontend/src/pages/TasksPage.tsx`, `frontend/src/pages/TaskDetailPage.tsx`, and the state components `frontend/src/components/Spinner.tsx`, `EmptyState.tsx`, `ErrorMessage.tsx`, and `TaskList.tsx` (depends on T053; RED via T049)
- [X] T055 [US3] Run the US3 tests across `tests/unit/`, `tests/integration/BallastLane.PostgreSql.Tests/`, `tests/http/BallastLane.TaskApi.HttpTests/`, and `frontend/tests/components/` and record the observed GREEN results in `docs/evidence/genai/EV-0013-us3-task-read.md` (depends on T054)
- [X] T056 [US3] Refactor US3 code in `src/Domain/Tasks/`, `src/Application/Tasks/`, `src/Infrastructure/Persistence/`, `src/TaskApi/Controllers/`, and `frontend/src/`, rerun, and record the result in `docs/evidence/genai/EV-0013-us3-task-read.md` (depends on T055)
- [X] T057 [US3] Record US3 TDD and GenAI evidence in `docs/evidence/genai/EV-0013-us3-task-read.md` (depends on T056)

**Checkpoint**: Reading works and ownership isolation is proven.

---

## Phase 6: User Story 4 - Create a Task (Priority: P2)

**Goal**: An authenticated user creates a task with a required title, optional description, required
calendar due date, Pending default, and an owner the client cannot choose.

**Independent Test**: as one authenticated user, create the accepted cases and attempt every rejected
case, then verify only accepted tasks exist and all are owned by the caller.

### Tests for User Story 4 (MANDATORY)

- [X] T058 [P] [US4] Write domain and application tests asserting that a blank, whitespace-only, or missing title is rejected; a 200-character title is accepted and 201 rejected; a 2,000-character description is accepted and 2,001 rejected; a missing or `2026-11-20T10:00:00Z` due date is rejected; a past due date is accepted; an omitted status defaults to `Pending`; `Done` is rejected; and `TaskDraft` cannot carry an owner, in `tests/unit/BallastLane.Domain.Tests/TaskValidationTests.cs` and `tests/unit/BallastLane.Application.Tests/CreateTaskUseCaseTests.cs`
- [X] T059 [P] [US4] Write HTTP tests for `POST /api/tasks` asserting 201 with a `Location` header and `status: "Pending"` when status is omitted, 201 with `InProgress` when supplied, 201 with a `null` description when omitted, 201 for a past due date, 400 for a blank and for a whitespace-only title, 400 for a `dueDate` containing a time component, 400 for `status: "Done"`, 201 while ignoring a supplied `ownerId` naming another user, and 401 unauthenticated, in `tests/http/BallastLane.TaskApi.HttpTests/TaskCreateTests.cs`
- [X] T060 [P] [US4] Write frontend tests asserting the create form shows a required-title error and an invalid-date error, disables submission while a request is in flight, and adds the created task to the list without a reload, in `frontend/tests/components/TaskFormCreate.test.tsx`

### RED checkpoint for User Story 4

- [X] T061 [US4] Create the compile-only contracts this story's tests require - `src/Domain/Tasks/TaskDraft.cs`, `src/Application/Tasks/{TaskValidator,CreateTaskUseCase}.cs`, `src/Application/Abstractions/ITaskWriter.cs`, `src/TaskApi/Contracts/CreateTaskRequest.cs`, and a minimal `frontend/src/components/TaskForm.tsx` with `NotImplementedException` bodies - so the suite compiles, then run the US4 tests in `tests/unit/BallastLane.Domain.Tests/TaskValidationTests.cs`, `tests/unit/BallastLane.Application.Tests/CreateTaskUseCaseTests.cs`, `tests/http/BallastLane.TaskApi.HttpTests/TaskCreateTests.cs`, and `frontend/tests/components/TaskFormCreate.test.tsx`, and record each observed RED failure in `docs/evidence/genai/EV-0014-us4-task-create.md` (depends on T058, T059, T060)

### Implementation for User Story 4

- [X] T062 [US4] Implement `src/Domain/Tasks/TaskDraft.cs` and `src/Application/Tasks/TaskValidator.cs` with the exact constraints in [data-model.md](./data-model.md) (depends on T061 RED)
- [X] T063 [US4] Implement `src/Application/Tasks/CreateTaskUseCase.cs` and `src/Application/Abstractions/ITaskWriter.cs`, assigning `Pending` when no status is supplied and taking the owner strictly from `AuthenticatedUser` (depends on T062; RED via T061)
- [X] T064 [US4] Implement `AddAsync` in `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` with a parameter for every value and `created_at` and `updated_at` taken from `ITimeProvider` (depends on T063; RED via T061)
- [X] T065 [US4] Implement the `POST` action in `src/TaskApi/Controllers/TasksController.cs` returning `CreatedAtAction` with the task id, plus `src/TaskApi/Contracts/CreateTaskRequest.cs` (depends on T064; RED via T061)
- [X] T066 [US4] Implement create mode in `frontend/src/components/TaskForm.tsx` and the `add` action in `frontend/src/state/tasksReducer.ts` (depends on T065; RED via T061)
- [X] T067 [US4] Run the US4 tests across the four files named in T061 and record the observed GREEN results in `docs/evidence/genai/EV-0014-us4-task-create.md` (depends on T066)
- [X] T068 [US4] Refactor US4 code in `src/Domain/Tasks/`, `src/Application/Tasks/`, `src/TaskApi/Controllers/`, and `frontend/src/components/TaskForm.tsx`, rerun, and record the result in `docs/evidence/genai/EV-0014-us4-task-create.md` (depends on T067)
- [X] T069 [US4] Record US4 TDD and GenAI evidence in `docs/evidence/genai/EV-0014-us4-task-create.md` (depends on T068)

**Checkpoint**: Creation works with full validation and owner protection.

---

## Phase 7: User Story 5 - Update a Task (Priority: P2)

**Goal**: An authenticated user edits title, description, due date, and status of an owned task, with
any status transition allowed and invalid edits leaving the stored task unchanged.

**Independent Test**: as one authenticated user, update across each status transition and each field,
attempt invalid updates, and attempt to update another user's task.

### Tests for User Story 5 (MANDATORY)

- [X] T070 [P] [US5] Write application tests asserting that every transition among `Pending`, `InProgress`, and `Completed` in both directions is accepted; a blank title, an unknown status, and a malformed due date produce a validation outcome; a non-owned task produces a not-found outcome; and `id`, `createdAt`, and the owner are unchanged by an update, in `tests/unit/BallastLane.Application.Tests/UpdateTaskUseCaseTests.cs`
- [X] T071 [P] [US5] Write HTTP tests for `PUT /api/tasks/{id}` asserting 200 with the updated representation for each status transition, 400 for a blank title, an unknown status, and a `dueDate` with a time component while a subsequent `GET` shows the old values, 404 for another user's task with that task unchanged, 404 for an unknown identifier, and 401 unauthenticated, in `tests/http/BallastLane.TaskApi.HttpTests/TaskUpdateTests.cs`
- [X] T072 [P] [US5] Write frontend tests asserting the edit form pre-populates current values, shows a server validation error against the field, keeps the previous values visible when the update fails, and reflects a successful update in the list without a reload, in `frontend/tests/components/TaskFormEdit.test.tsx`

### RED checkpoint for User Story 5

- [X] T073 [US5] Create the compile-only contracts this story's tests require - `src/Application/Tasks/UpdateTaskUseCase.cs` and `src/TaskApi/Contracts/UpdateTaskRequest.cs` with `NotImplementedException` bodies, plus an edit-mode stub in `frontend/src/components/TaskForm.tsx` - so the suite compiles, then run the US5 tests in `tests/unit/BallastLane.Application.Tests/UpdateTaskUseCaseTests.cs`, `tests/http/BallastLane.TaskApi.HttpTests/TaskUpdateTests.cs`, and `frontend/tests/components/TaskFormEdit.test.tsx`, and record each observed RED failure in `docs/evidence/genai/EV-0015-us5-task-update.md` (depends on T070, T071, T072)

### Implementation for User Story 5

- [X] T074 [US5] Implement `src/Application/Tasks/UpdateTaskUseCase.cs`, reusing `TaskValidator` and loading the task through the owner-scoped reader before applying the whole validated draft (depends on T073 RED)
- [X] T075 [US5] Implement `UpdateAsync` in `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs`, setting `updated_at` from `ITimeProvider` and never modifying `owner_id` or `created_at` (depends on T074; RED via T073)
- [X] T076 [US5] Implement the `PUT` action in `src/TaskApi/Controllers/TasksController.cs` plus `src/TaskApi/Contracts/UpdateTaskRequest.cs` with a required status (depends on T075; RED via T073)
- [X] T077 [US5] Implement edit mode in `frontend/src/components/TaskForm.tsx` and the `update` action in `frontend/src/state/tasksReducer.ts` (depends on T076; RED via T073)
- [X] T078 [US5] Run the US5 tests in the three files named in T073 and record the observed GREEN results in `docs/evidence/genai/EV-0015-us5-task-update.md` (depends on T077)
- [X] T079 [US5] Refactor US5 code in `src/Application/Tasks/UpdateTaskUseCase.cs`, `src/TaskApi/Controllers/TasksController.cs`, and `frontend/src/components/TaskForm.tsx`, rerun, and record the result in `docs/evidence/genai/EV-0015-us5-task-update.md` (depends on T078)
- [X] T080 [US5] Record US5 TDD and GenAI evidence in `docs/evidence/genai/EV-0015-us5-task-update.md` (depends on T079)

**Checkpoint**: The full update lifecycle works, including every status transition.

---

## Phase 8: User Story 6 - Delete a Task (Priority: P2)

**Goal**: An authenticated user deletes an owned task; it leaves the collection; other users' tasks
are never deleted.

**Independent Test**: with two tasks, delete one, confirm it is gone and the other is untouched, and
attempt to delete another user's task.

### Tests for User Story 6 (MANDATORY)

- [X] T081 [P] [US6] Write HTTP tests for `DELETE /api/tasks/{id}` asserting 204 and a subsequent 404 for the same identifier, 404 for a second delete, 404 for another user's task with that task still present for its owner, 404 for an unknown identifier, and 401 unauthenticated, in `tests/http/BallastLane.TaskApi.HttpTests/TaskDeleteTests.cs`
- [X] T082 [P] [US6] Write frontend tests asserting a confirmation is required before any delete request is sent, cancelling sends no request, a confirmed delete removes the task from the list, and a failed delete keeps the task and shows an error, in `frontend/tests/components/TaskDelete.test.tsx`

### RED checkpoint for User Story 6

- [X] T083 [US6] Create the compile-only contracts this story's tests require - `src/Application/Tasks/DeleteTaskUseCase.cs` and `frontend/src/components/ConfirmDialog.tsx` with `NotImplementedException` bodies - so the suite compiles, then run the US6 tests in `tests/http/BallastLane.TaskApi.HttpTests/TaskDeleteTests.cs` and `frontend/tests/components/TaskDelete.test.tsx`, and record each observed RED failure in `docs/evidence/genai/EV-0016-us6-task-delete.md` (depends on T081, T082)

### Implementation for User Story 6

- [X] T084 [US6] Implement `src/Application/Tasks/DeleteTaskUseCase.cs`, returning a not-found outcome when the task is missing or not owned (depends on T083 RED)
- [X] T085 [US6] Implement `DeleteAsync` in `src/Infrastructure/Persistence/NpgsqlTaskRepository.cs` scoped by both `id` and `owner_id` (depends on T084; RED via T083)
- [X] T086 [US6] Implement the `DELETE` action in `src/TaskApi/Controllers/TasksController.cs` returning `204 No Content` (depends on T085; RED via T083)
- [X] T087 [US6] Implement `frontend/src/components/ConfirmDialog.tsx` and the `remove` action in `frontend/src/state/tasksReducer.ts` (depends on T086; RED via T083)
- [X] T088 [US6] Run the US6 tests in the two files named in T083 and record the observed GREEN results in `docs/evidence/genai/EV-0016-us6-task-delete.md` (depends on T087)
- [X] T089 [US6] Refactor US6 code in `src/Application/Tasks/DeleteTaskUseCase.cs`, `src/TaskApi/Controllers/TasksController.cs`, and `frontend/src/components/ConfirmDialog.tsx`, rerun, and record the result in `docs/evidence/genai/EV-0016-us6-task-delete.md` (depends on T088)
- [X] T090 [US6] Record US6 TDD and GenAI evidence in `docs/evidence/genai/EV-0016-us6-task-delete.md` (depends on T089)

**Checkpoint**: All four CRUD operations work with ownership enforced throughout.

---

## Phase 9: User Story 7 - Seeded Demonstration Environment (Priority: P3)

**Goal**: The documented startup alone produces users and tasks, the documented credentials log into a
populated account, and repeated startup neither duplicates nor corrupts data.

**Independent Test**: from a clean environment, follow only the documented instructions, log in with
the documented credentials, see seeded tasks; restart and confirm data is intact and not duplicated.

### Tests for User Story 7 (MANDATORY)

- [X] T091 [P] [US7] Write tests asserting that the documented demonstration credentials authenticate through `POST /api/users/login`, that the resulting token lists the documented number of seeded tasks, that a second seed run leaves user and task counts unchanged, and that seeded rows are readable from a connection opened after the seeder completes, in `tests/integration/BallastLane.Migrations.Tests/DemonstrationDatasetTests.cs`

### RED checkpoint for User Story 7

- [X] T092 [US7] Declare the demonstration dataset constants this story's tests assert on in `src/Infrastructure/Persistence/DemoSeeder.cs` so the suite compiles, then run the US7 tests in `tests/integration/BallastLane.Migrations.Tests/DemonstrationDatasetTests.cs` and record the observed RED failures in `docs/evidence/genai/EV-0017-us7-demonstration-environment.md` (depends on T091)

### Implementation for User Story 7

- [X] T093 [US7] Finalize the demonstration dataset in `src/Infrastructure/Persistence/DemoSeeder.cs` and document the credentials as demonstration-only in `README.md` (depends on T092 RED)
- [X] T094 [US7] Run the US7 tests in `tests/integration/BallastLane.Migrations.Tests/DemonstrationDatasetTests.cs` and record the observed GREEN results in `docs/evidence/genai/EV-0017-us7-demonstration-environment.md` (depends on T093)
- [X] T095 [US7] Execute [quickstart.md](./quickstart.md) sections 3 and 8 against a clean environment and record the observed results, including the repeat-seed output and the restart check, in `docs/evidence/genai/EV-0017-us7-demonstration-environment.md` (depends on T094)
- [X] T096 [US7] Record US7 TDD and GenAI evidence in `docs/evidence/genai/EV-0017-us7-demonstration-environment.md`, stating which scenarios were automated and which were executed manually (depends on T095)

**Checkpoint**: The whole journey is reproducible from documentation alone.

---

## Phase 10: Verification and Convergence

**Purpose**: Cross-cutting verification that is not the behavior of a single story.

- [X] T097 [P] Verify the architectural dependency rules by inspecting every `src/*/*.csproj` and running `tests/unit/BallastLane.Architecture.Tests/DependencyDirectionTests.cs`, confirming `Domain` and `Application` reference no ASP.NET Core or Npgsql assembly and that `Infrastructure` is referenced only inside `src/UserApi/Program.cs`, `src/TaskApi/Program.cs`, and `src/Migrator/Program.cs`
- [X] T098 [P] Verify no prohibited library is referenced directly or transitively by scanning every `src/**/*.csproj` and the resolved package graph for `EntityFrameworkCore`, `Dapper`, `MediatR`, and `Mediator`, recording the scan output in `docs/evidence/genai/EV-0018-review-and-convergence.md`
- [X] T099 [P] Verify the required security behaviors with observed results in `tests/http/BallastLane.TaskApi.HttpTests/` and `tests/http/BallastLane.UserApi.HttpTests/`: cross-user retrieve, update, and delete; ownership-field tampering; a SQL-injection payload stored and read back as data; no password or hash in any response; duplicate registration returning 409
- [X] T100 [P] Verify the frontend passes lint, the strict type check, and the production build with no warnings, running `frontend/package.json` scripts and recording the output in `docs/evidence/genai/EV-0018-review-and-convergence.md`
- [X] T101 Run the complete suite described in `BallastLane.sln` and `frontend/package.json` from a clean checkout, including the real PostgreSQL integration and HTTP tests, and record the observed output in `docs/evidence/genai/EV-0018-review-and-convergence.md`, confirming no test is skipped for a missing database
- [X] T102 Execute [quickstart.md](./quickstart.md) sections 4, 5, and 6 and record the observed results for every listed step in `docs/evidence/genai/EV-0018-review-and-convergence.md`, marking any step not executed rather than assuming it passed
- [X] T103 Verify `README.md` claims match implemented behavior and that no unimplemented feature is described as working
- [X] T104 Complete `README.md` with the user story, architecture overview, dependency direction, setup steps, test commands, demonstration credentials labeled demonstration-only, API surface summary, engineering rationale, known limitations, and pointers to `GENAI.md` and `docs/evidence/genai/`
- [X] T105 Run `/speckit-analyze` against the implemented artifacts and record its report in `docs/evidence/genai/EV-0018-review-and-convergence.md`, resolving any blocking finding before claiming readiness
- [X] T106 Verify repository hygiene - no secrets, no machine-specific absolute paths, no tracked symbolic links - and record final evidence and commit references in `docs/evidence/genai/EV-0018-review-and-convergence.md`
- [X] T107 Consolidate the Generative AI REST API generation evidence in `GENAI.md`: the actual prompt and representative original generated output are captured during the API implementation tasks themselves (T033 for the user endpoints, T053 for the read endpoints, T065 for create, T076 for update, T086 for delete) and preserved in the implementation evidence records before any correction; this task assembles them into `GENAI.md` and `docs/evidence/genai/EV-0021-genai-api-exercise.md`, documenting the validation performed, the corrections or justified acceptance, and how authentication, authorization, validation rules, and edge cases were handled, then verifies every `GENAI.md` claim against observed output. TDD Not Applicable: documentation-only deliverable that changes no runtime behavior; substitute validation is the recorded build and test output it cites (depends on T106)

---

## Dependencies and Execution Order

### Phase dependencies

- **Setup (Phase 1)**: no dependencies.
- **Foundational (Phase 2)**: depends on Setup; blocks all user stories.
- **User stories (Phase 3 to 9)**: depend on Foundational.
- **Verification (Phase 10)**: depends on the desired stories being complete.

### Story dependencies

- **US1 (P1)**: after Foundational; independent of other stories.
- **US2 (P1)**: after Foundational; boots the second host and needs US1's account creation to
  produce a token for the valid-credential case. It needs no task functionality.
- **US3 (P2)**: after US2, because it consumes the Task API host. Its tests insert their own rows via
  a fixture, so it does not depend on US4.
- **US4 (P2)**: after US3, reusing the task entity, repository, and controller introduced there.
- **US5 (P2)**: after US4, reusing `TaskValidator` and the create contract shape.
- **US6 (P2)**: after US4, because its tests need tasks to exist.
- **US7 (P3)**: after US1, US3, and US4, because the demonstration needs a populated, listable account.
  It refines the seeder introduced in Foundational.

### Within each user story

```text
write tests -> observe RED -> implement -> observe GREEN -> refactor -> record evidence
```

- Tests are written and observed failing before implementation.
- The first implementation task depends directly on the RED checkpoint; every later task in the same
  group names the RED checkpoint it inherits, so no implementation task has an unexplained dependency.
- Domain before application, application before infrastructure, infrastructure before endpoints,
  endpoints before frontend integration.
- A story is independently testable before the next priority begins.

### Parallel opportunities

`[P]` applies to independent test files, independent projects, and independent stories, and appears
only on test-authoring and independent verification tasks. It is never applied across a
test/implementation pair and never allows implementation before its RED checkpoint is observed.

---

## Parallel Example: Phase 2

```text
# Six independent test files, all authored before the single RED checkpoint T014:
T008  tests/integration/BallastLane.Migrations.Tests/MigrationApplicationTests.cs
T009  tests/unit/BallastLane.Infrastructure.UnitTests/SchemaMigratorTests.cs
T010  tests/unit/BallastLane.Infrastructure.UnitTests/NpgsqlConnectionFactoryTests.cs
T011  tests/unit/BallastLane.Api.UnitTests/OperationResultMappingTests.cs
T012  tests/unit/BallastLane.Api.UnitTests/TokenValidationParametersTests.cs
T013  tests/integration/BallastLane.Migrations.Tests/DemoSeedTests.cs
```

## Parallel Example: User Story 4

```text
# All three test files first; none is parallelized with its implementation:
T058  tests/unit/BallastLane.Domain.Tests/TaskValidationTests.cs
T059  tests/http/BallastLane.TaskApi.HttpTests/TaskCreateTests.cs
T060  frontend/tests/components/TaskFormCreate.test.tsx
# Then T061 RED, then T062 to T066 sequentially.
```

---

## Implementation Strategy

### First increment: P1

1. Phase 1 Setup.
2. Phase 2 Foundational (blocks everything).
3. Phase 3 US1, then Phase 4 US2 - both P1. Together they deliver registration, login, logout,
   persistence, and the security boundary.
4. Validate US1 and US2 independently.

**P1 is the first implementation increment, not the complete assessment.** User stories US3 to US6
(task CRUD and the frontend task-management flows), US7 (seeded demonstration users and data), and
T107 (`GENAI.md` with the Generative AI API-generation exercise) remain **mandatory deliverables**.
Completing P1 does not reduce the obligation to deliver them; it only fixes the order in which they
are built.

### Incremental delivery

1. US1 and US2 - identity and the access boundary.
2. US3 - read, which first makes the task API meaningful.
3. US4, then US5, then US6 - the remaining CRUD operations.
4. US7 - the reproducible demonstration.
5. Phase 10 - verification, documentation, and readiness.

Each step leaves the previous steps working.

### Scope honesty

The priority order decides build order, not delivery scope. CRUD (US3 to US6), frontend task
management, seeded demonstration data (US7), and `GENAI.md` (T107) are mandatory assessment
deliverables and cannot be dropped silently. If any of them is not delivered, it must be reported as
not delivered in `README.md`, `GENAI.md`, and the evidence record, and no test may be skipped or
removed to make a suite appear green.

---

## Notes

- `[P]` tasks touch different files and have no incomplete dependencies.
- `[Story]` maps each task to a user story for traceability to [spec.md](./spec.md).
- `Not Applicable` for TDD appears only in Phase 1 (T001 to T007), each with a written justification
  and the substitute validation task T007, because no runtime behavior is created there.
- Constraints from [data-model.md](./data-model.md) are quoted in the task that implements them so
  they are not left to implementation-time discretion.
- The Generative AI API-generation prompt and the representative original generated code are captured
  **during** the API implementation tasks (T033, T053, T065, T076, T086), before any correction, and
  stored in that implementation activity's evidence record. T107 consolidates them into `GENAI.md`;
  it does not reconstruct them afterwards.
- Commit after each logical group; do not commit without the evidence that supports it.

---

## Requirement Traceability

Every requirement has at least one task. This table makes the mapping machine-checkable rather than
inferred from the `[USn]` label alone.

| Requirement | Tasks |
|---|---|
| FR-001 registration | T026, T027 |
| FR-002 authentication | T026, T027 |
| FR-003 persisted users | T032, T046, T091 |
| FR-004 logout | T028, T034 |
| FR-005 task CRUD | T047, T053, T059, T065, T071, T076, T081, T086 |
| FR-006 task fields | T046, T050, T058 |
| FR-007 cross-user denial | T051, T071, T081 |
| FR-008 two API surfaces | T027, T033, T038, T040 |
| FR-009 ownership from claims | T059, T063 |
| FR-010 frontend flows | T028, T034, T054, T066, T077, T087 |
| FR-011 frontend states | T048, T054, T060, T072, T082 |
| FR-012 seeded demo data | T013, T020, T091, T093 |
| FR-013 statuses | T050, T058, T059, T070 |
| FR-014 field rules | T058, T059, T062, T070 |
| FR-015 password policy | T026, T027 |
| FR-016 delete semantics | T081, T084, T085 |
| FR-017 error categories | T011, T017, T059, T071 |
| NFR-001 security | T012, T018, T027, T099 |
| NFR-002 accessibility | T034, T048, T102 |
| NFR-003 responsiveness | T102 |
| NFR-004 reproducible startup | T005, T095, T104 |
| NFR-005 durability | T046, T095 |
| NFR-006 test levels | T008 to T013, T045 to T048, T058 to T060, T070 to T072, T081, T082 |
| DO-01 README | T104 |
| DO-02 GENAI.md and the API-generation exercise | capture in T033, T053, T065, T076, T086; consolidated in T107 |
| DO-03 user story and presentation | T104 |
| DO-04 single public repository | T106 |
| DO-05 no symbolic links | T106 |
| SC-001, SC-002 journey timing | T102 (timed manual recording), with SC-002 also exercised by the frontend flow test T028 and the create/update flows T066, T077 |
| SC-003 cross-user denial | T099 |
| SC-004 credential rejection | T012, T038, T099 |
| SC-005 UI states | T048, T060, T072, T082, T102 |
| SC-006 clean-start demo | T095 |
| SC-007 restart durability | T095 |
| SC-008 clean console | T102 |
