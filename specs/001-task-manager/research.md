# Research: Personal Task Manager (001-task-manager)

**Phase**: 0 - Outline and Research | **Date**: 2026-11-16 | **Plan**: [plan.md](./plan.md)

Every open question in the plan's Technical Context is resolved here, so the plan contains no
unresolved clarification markers. Observed environment facts are stated as observed; design choices
are `[AI-PROPOSED]` until the developer approves them.

## Observed environment

| Check | Observed result |
|---|---|
| `dotnet --list-sdks` | `5.0.408`, `8.0.428`, `9.0.304`, `10.0.404`; default `10.0.404` |
| `node --version` / `npm --version` | `v25.9.0` / `11.12.1` |
| `docker --version` / `docker compose version` | `28.3.3` / `v2.39.2-desktop.1` |

## R1 - .NET version

- **Decision**: .NET 8 (`net8.0`), SDK 8.0.428.
- **Rationale**: the assessment mandates no version; the target job description references .NET 6,
  7, and 8, so 8 is the closest match to the panel's stack while still being a modern LTS line.
  Every required API (JWT bearer, `PasswordHasher<T>`, `WebApplicationFactory`, minimal hosting,
  OpenAPI) is generally available. The SDK is installed locally, so the build is verifiable now.
- **Alternatives considered**:
  - **.NET 10** (installed, current LTS): better long-term support, but further from the assessment's
    stated stack and adds no capability this feature needs.
  - **.NET 9** (installed): standard-term support that has already ended, so it is the worst of both
    options.
  - Recorded trade-off: .NET 8 passed its end-of-support date shortly before this plan. That is
    acceptable for a self-contained assessment deliverable whose target is the assessment's own
    stack; it would not be acceptable for a production system.

## R2 - One API host or two

- **Decision**: two hosts, `BallastLane.UserApi` and `BallastLane.TaskApi`, sharing `Domain`,
  `Application`, and `SharedWeb`.
- **Rationale**: the assessment says "Additionally, a second API should include endpoints for user
  creation, user login, and authorized and non-authorized endpoints". `api-standards.md` allows
  either separate hosts or route groups but requires the choice to be recorded. Two hosts make the
  requirement directly observable during the presentation and let each host own its own
  composition root, which is what `clean-architecture.md` section 10 describes.
- **Alternatives considered**:
  - **One host with two route groups**: fewer moving parts, one port, one Dockerfile. Rejected
    because it makes the second API implicit and weakens the demonstration of the requirement.
  - **One host with a shared authentication middleware and an area per API**: same objection, plus
    it invites business logic into a shared controller base.

## R3 - Authentication mechanism

- **Decision**: JWT bearer tokens, HS256, issued by the User API and validated by both hosts.
  Issuer `ballastlane-user-api`, audience `ballastlane-clients`, 60-minute lifetime,
  `ClockSkew = TimeSpan.Zero`.
- **Rationale**: `security.md` requires token validation with explicit signature, issuer, audience,
  and expiry checks, and `api-standards.md` requires the credential mechanism and expiry to be
  defined. A bearer token lets the Task API validate requests without calling the User API, which
  keeps the two hosts independently deployable and independently testable. Zero clock skew makes
  expiry deterministic for tests.
- **Alternatives considered**:
  - **Server-side session cookie**: needs a shared session store or sticky sessions across two hosts,
    which adds infrastructure the constitution forbids without a functional need.
  - **Opaque tokens with introspection**: introduces a runtime dependency between the hosts and a
    token store, again beyond the assessment's scope.
  - **ASP.NET Core Identity with cookies**: heavier than required and would pull in EF-oriented
    defaults, which the assessment forbids.

## R4 - Password hashing

- **Decision**: `PasswordHasher<User>` from `Microsoft.Extensions.Identity.Core` behind an
  Application-owned `IPasswordHasher` boundary.
- **Rationale**: `security.md` names this component first and forbids rolling custom cryptography.
  It provides a per-password salt and constant-time verification. The boundary keeps `Application`
  free of framework dependencies.
- **Alternatives considered**: BCrypt.Net-Next is equally acceptable per the standard and would add a
  third-party dependency for no benefit; ASP.NET Core Identity's full stack brings user stores and
  EF conventions that are prohibited or unnecessary.

## R5 - PostgreSQL access

- **Decision**: Npgsql + ADO.NET directly, with hand-written SQL, `await using` disposal, and
  `CancellationToken` on every async call.
- **Rationale**: `database-standards.md` mandates this and the assessment prohibits EF, Dapper, and
  Mediator. Hand-written SQL keeps the query surface small and reviewable.
- **Alternatives considered**: a micro-ORM (prohibited); a custom query builder (adds an abstraction
  with no consumer need).

## R6 - Migration mechanism

- **Decision**: ordered SQL files under `db/migrations/`, applied by a small runner in
  `BallastLane.Migrator` that records applied versions in a `schema_migrations` table.
- **Rationale**: `database-standards.md` requires version-controlled, forward-only, apply-once,
  reproducible migrations with seeding kept separate. A ~60-line runner over an allow-listed file
  set satisfies this without a dependency, and it keeps the mechanism visible for code review.
- **Alternatives considered**: DbUp or Flyway add a dependency and configuration for the same
  behaviour; applying DDL at API startup races between the two hosts and hides the step.

## R7 - Test database strategy

- **Decision**: Docker Compose creates `taskmanager` and `taskmanager_test`; tests read
  `ConnectionStrings__TestDatabase` and fall back to the documented local default. A collection
  fixture applies migrations once; each test runs in its own transaction and rolls back.
- **Rationale**: `testing-and-tdd.md` and `database-standards.md` require a real PostgreSQL instance,
  and `delivery-and-ci.md` requires those tests to run in CI rather than be skipped. A
  pre-created database plus transactional isolation is the simplest arrangement that is
  order-independent and fast.
- **Alternatives considered**:
  - **Testcontainers**: gives per-run isolation and no shared state, but adds a dependency and slows
    each run; the compose-created database already provides a real server.
  - **Recreating the schema per test**: correct but slow and it obscures which behavior failed.
  - Recorded trade-off: the transactional approach cannot test DDL, so migration tests
    (`BallastLane.Migrations.Tests`) instead apply migrations to a freshly created database.

## R8 - Frontend stack

- **Decision**: React 18.3 + TypeScript 5.6 on Vite 5.4, React Router 6, plain CSS, Vitest 2 with
  React Testing Library 16.
- **Rationale**: `frontend-standards.md` and `coding-standards.md` propose React with TypeScript and
  require strict typing, a separated API client, and component/flow tests. Vite keeps the build
  small and fast; Vitest shares the Vite transform pipeline.
- **Alternatives considered**: Next.js adds a server runtime the assessment does not need; a global
  state library is excluded by `frontend-standards.md` 2.9 for this scope.

## R9 - Token storage in the frontend

- **Decision**: the access token is held in memory inside an auth context and mirrored to
  `sessionStorage` so a page refresh keeps the session; it is sent as an `Authorization: Bearer`
  header.
- **Rationale**: `frontend-standards.md` forbids storing tokens in `localStorage` without a recorded
  decision, and requires centralized 401 handling. `sessionStorage` scopes the credential to the tab
  and dies with it, does not survive a browser restart, and is not sent automatically, so it cannot
  be used for CSRF.
- **Alternatives considered**:
  - **`httpOnly` cookie**: immune to script access, but needs CSRF protection and cross-origin
    credential configuration between two hosts and the dev server; disproportionate here.
  - **In-memory only**: safest against persistence, but every refresh logs the user out, which
    conflicts with requirement FR-004's intent that a session survives normal use.
  - Recorded trade-off: like any browser-stored bearer token, `sessionStorage` is readable by a
    successful XSS. The mitigation is that the app renders no untrusted HTML and React escapes by
    default; this trade-off is stated in the README.

## R10 - OpenAPI

- **Decision**: `Swashbuckle.AspNetCore` 6.x with a Swagger UI endpoint enabled in Development and a
  generated document per host.
- **Rationale**: `api-standards.md` requires generated, not hand-maintained, OpenAPI. On .NET 8
  Swashbuckle is the mature option that also serves UI for the demonstration.
- **Alternatives considered**: `Microsoft.AspNetCore.OpenApi` provides document generation only and
  has no built-in UI; NSwag is larger than needed.

## R11 - Due date representation

- **Decision**: `DateOnly` in C#, PostgreSQL `date`, and `YYYY-MM-DD` strings on the wire.
- **Rationale**: FR-014 requires a calendar date with no time or timezone. `DateOnly` maps cleanly to
  PostgreSQL `date` through Npgsql, and a string on the wire avoids the timezone shifts that ISO
  datetimes cause.
- **Alternatives considered**: `DateTime` with time truncated to midnight invites timezone bugs;
  `timestamptz` contradicts the specification.

## R12 - Case-insensitive email uniqueness

- **Decision**: unique index on `lower(email)`; the application normalizes to lower case before
  writing and before lookup.
- **Rationale**: PDC-04 requires case-insensitive uniqueness. A functional unique index enforces it
  in the database, which `database-standards.md` requires for uniqueness, while normalization keeps
  application lookups consistent.
- **Alternatives considered**: PostgreSQL `citext` requires an extension; validation-only checks are
  race-prone and rejected by the standard.

## R13 - Cross-user response code

- **Decision**: `404 Not Found` for both "does not exist" and "belongs to another user".
- **Rationale**: FR-007 requires that the system never confirm the existence of another user's task.
  `403` would confirm existence, so `404` is the only consistent choice. `api-standards.md` allows
  either provided the choice is documented and consistent.
- **Alternatives considered**: `403 Forbidden` is more semantically precise for a known record but
  leaks existence, which the specification explicitly forbids.

## R14 - Status code for validation and conflict

- **Decision**: `400` for shape validation, `409` for duplicate email, `404` for
  missing-or-not-owned, `401` for authentication. No `422` anywhere.
- **Rationale**: `api-standards.md` requires the `409`/`422` choice to be documented and consistent.
  The only business conflict this product has is duplicate email, which is naturally `409`.
- **Alternatives considered**: `422` for rule violations; rejected because there is no violation in
  this product that is not either a shape error or a uniqueness conflict.

## R15 - Frontend state approach

- **Decision**: component state for forms; one `AuthContext` for identity; a `tasksReducer` for the
  task collection so create, update, and delete apply consistently without refetching the whole list.
- **Rationale**: `frontend-standards.md` allows local state plus context at this scope and requires
  that after a mutation the view reflects the new state without a full reload.
- **Alternatives considered**: a global store library (excluded), or refetching the list after each
  mutation (simpler but causes visible loading flicker and loses scroll position).

## R16 - Due-date ordering tie-break

- **Decision**: `ORDER BY due_date ASC, created_at ASC, id ASC`.
- **Rationale**: PDC-05 specifies due date then creation order; `id` is added only to make the order
  fully deterministic when two tasks share both values, which keeps list assertions stable.
- **Alternatives considered**: leaving the tie-break to the database provides no guaranteed order and
  makes tests flaky.

## Open items

- The eight `PDC` decisions adopted by the plan remain `[AI-PROPOSED]` until developer confirmation.
  None blocks implementation; each is recorded in `plan.md` under "Adopted Specification Decisions".
- The .NET 8 end-of-support trade-off in R1 is recorded for the developer to accept or override. If
  the developer prefers a supported runtime, .NET 10 is installed and the plan's design is unchanged
  apart from the target framework moniker and the OpenAPI package choice.
