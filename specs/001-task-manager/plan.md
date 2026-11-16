# Implementation Plan: Personal Task Manager (001-task-manager)

**Branch**: `001-task-manager` | **Date**: 2026-11-16 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-task-manager/spec.md`

**Project override** of the installed plan template. Governed by
`.specify/memory/constitution.md`. Read the engineering index and the standards relevant to this
feature before filling this plan: `docs/engineering/README.md`, `clean-architecture.md`,
`solid-principles.md`, `testing-and-tdd.md`, `api-standards.md`, `security.md`,
`database-standards.md`, `frontend-standards.md`, `coding-standards.md`, `delivery-and-ci.md`,
`genai.md`.

## Summary

Build a small personal Task Manager: an ASP.NET Core backend with two API hosts over a shared
domain and application core, PostgreSQL persistence through Npgsql with ADO.NET (no ORM), and a
React + TypeScript frontend covering registration, login, logout, and task CRUD with explicit
loading, empty, success, validation, and error states.

The specification's 17 functional requirements are delivered by seven independently testable user
stories: authentication (US1), the public/protected access boundary (US2), read (US3), create
(US4), update (US5), delete (US6), and the seeded demonstration environment (US7).

## Technical Context

**Language/Version**: C# 12 on **.NET 8** (`net8.0`, SDK 8.0.428); TypeScript 5.6 with React 18.3
on Node 25.9.0 (the build tooling requires Node 20 or newer).

**Primary Dependencies**: ASP.NET Core 8 (`Microsoft.AspNetCore.Mvc`), `Npgsql` 8.x,
`Microsoft.AspNetCore.Authentication.JwtBearer` 8.x, `Microsoft.Extensions.Identity.Core` 8.x
(`PasswordHasher<T>`), `Swashbuckle.AspNetCore` 6.x (OpenAPI), `Microsoft.AspNetCore.Mvc.Testing`
8.x (HTTP integration tests), `xunit` 2.x, Vite 5.4, React Router 6, Vitest 2 + React Testing
Library 16.

**Storage**: PostgreSQL 16 in a container. Access is Npgsql + ADO.NET only. The schema is defined
by ordered SQL migrations under `db/migrations/`; seeding is a separate idempotent step.

**Testing**: xUnit for Domain, Application, Infrastructure (unit), and API controller (unit) tests;
xUnit against a real PostgreSQL 16 instance for repository, constraint, transaction, and migration
tests; xUnit + `WebApplicationFactory` for HTTP integration tests of both hosts; Vitest + React
Testing Library for component and user-flow tests.

**Target Platform**: Linux containers for PostgreSQL and the two APIs; modern evergreen browsers
for the frontend. Local development on Windows, macOS, or Linux via Docker Compose plus `npm`.

**Project Type**: web application - two backend API hosts sharing one core, plus a static frontend.

**Performance Goals**: ordinary interactive latency for a single-user demo. The specification
defines no load, concurrency, or throughput targets, and none are invented here.

**Constraints**: Entity Framework, EF Core, Dapper, Mediator, and MediatR forbidden solution-wide,
including transitively. Identity must come from validated JWT claims, never client data. All SQL
parameterized. Integration tests require a real PostgreSQL instance and must not be skipped in CI.
No infrastructure beyond PostgreSQL and the two hosts.

**Scale/Scope**: one deployment, two demonstration user accounts, a handful of seeded tasks per
account, roughly 20 endpoints in total across both hosts.

## Constitution Check

*GATE: MUST pass before implementation. Re-check after design.*

Every principle is answered explicitly. A `MUST` violation is blocking.

| Principle | Required evidence in this plan | Status |
|---|---|---|
| I. Assessment-first | Every FR traces to an assessment obligation (spec `Traceability to Assessment`); this plan covers all 17 FRs and 6 NFRs; the pre-implementation `/speckit-analyze` is scheduled before implementation and is enforced by a blocking precondition in the implement workflow | PASS |
| II. Clean Architecture and SOLID | Dependency matrix completed below; layer responsibilities named; every abstraction tied to a seam; no `Domain`/`Application` reference to ASP.NET Core, Npgsql, or frontend packages | PASS |
| III. TDD | Test strategy completed below for seven layers including Infrastructure unit and API controller unit tests; RED-first ordering stated; the analyze phase validates the *planned* ordering and implementation observes the actual RED; no test/implementation pair parallelizable | PASS |
| IV. Secure APIs and persistence | Authn/authz boundaries, resource-level ownership in the Application layer, `PasswordHasher<T>`, explicit JWT validation parameters, parameterized SQL, database constraints, unique-email constraint, no prohibited libraries | PASS |
| V. Full-stack delivery | Responsive React UI with explicit states and accessibility; idempotent seeded demo users and tasks documented as demonstration-only; Docker Compose plus documented `npm` commands; README and `GENAI.md` obligations accepted | PASS |
| VI. GenAI evidence | Capture points listed per activity below; one record per activity or related group; `EV-0007` already covers specify | PASS |

### Clean Architecture dependency analysis

| From \ To | Domain | Application | Infrastructure | API (User/Task hosts) | Frontend |
|---|---|---|---|---|---|
| Domain | - | no | no | no | no |
| Application | yes | - | no | no | no |
| Infrastructure | yes | yes | - | no | no |
| API (User/Task hosts) | yes | yes | yes (composition root only) | - | no |
| Frontend | no | no | no | HTTP contract only | - |

Each non-empty cell and its justification:

- `Application -> Domain`: allowed. Use cases and validators orchestrate domain rules.
- `Infrastructure -> Application`: allowed. `NpgsqlUserRepository`, `NpgsqlTaskRepository`,
  `IdentityPasswordHasher`, and `JwtTokenIssuer` implement boundaries the Application owns.
- `Infrastructure -> Domain`: allowed. Adapters map domain entities to rows and back.
- `API -> Application`, `API -> Domain`: allowed. Controllers bind, delegate to one use case, map
  the outcome.
- `API -> Infrastructure`: allowed **only** in `Program.cs` for dependency registration. No
  controller, filter, or hand-constructed middleware may use an Infrastructure type. Enforced by an
  architecture test.
- Prohibited and verified absent: `Domain -> anything`, `Application -> Infrastructure`,
  `Application` or `Domain` referencing `Microsoft.AspNetCore.*` or `Npgsql`, `Frontend` importing
  backend code.

Business rules live in `Domain` and `Application` only and reference no ASP.NET Core, Npgsql,
PostgreSQL, or frontend types.

### SOLID design verification

| Principle | How this design satisfies it | Justification for any abstraction added |
|---|---|---|
| Single responsibility | `TaskValidator` and `UserValidator` own rules; `NpgsqlTaskRepository` owns SQL; controllers translate HTTP only; one use case per operation | No abstraction added, only narrow concrete types and one-use-case-per-class |
| Open/closed | The status set is a closed three-value `TaskStatus` enum plus validation; an update applies a whole validated draft | No extension point is added: the status set is fixed by the specification and no variation exists. An `ITaskStatusPolicy` with one implementation would be unjustified |
| Liskov substitution | `NpgsqlTaskRepository` returns `null` for "not found" exactly as `ITaskReader` states and never throws `NotSupportedException` for a declared member | Test fakes implement the same contracts with identical not-found semantics |
| Interface segregation | `ITaskReader` and `ITaskWriter` are separate; `IUserReader`, `IUserWriter`, `IPasswordHasher`, and `ITokenIssuer` are each consumed by distinct use cases | Interfaces exist for dependency inversion and test seams (database, hashing, token issuance, time), not for symmetry |
| Dependency inversion | Boundaries are declared in `Application`; Infrastructure implements them; each host's composition root registers the adapters | `ITimeProvider` is added because timestamps and token expiry affect behavior and tests must be deterministic |

Abstractions deliberately **not** added: no generic repository, no unit of work, no mediator, no
service locator, no `ITaskValidatorFactory`, no mapping library, and no global state library in the
frontend.

## Test Strategy (Principle III)

| Layer | Test type | Framework or tool | Real dependency or double | Scenarios mapped to acceptance criteria |
|---|---|---|---|---|
| Domain | Unit | xUnit | No dependency | Status validity and transitions (US5.2, FR-013); title, description, and due-date rules (US4.2, US4.3, FR-014); blank and whitespace titles (Edge Cases) |
| Application | Unit | xUnit with hand-written fakes | Fakes for `ITaskReader`, `ITaskWriter`, `IUserReader`, `IUserWriter`, `IPasswordHasher`, `ITokenIssuer`, `ITimeProvider` | Duplicate email and password policy (US1.2, US1.3, FR-001, FR-015); generic login failure (US1.5, FR-002); ownership rejection (US3.4, US5.4, US6.3, FR-007); owner ignored from client input (US4.5, FR-009); Pending default (US4.1); empty list (US3.3); not-found (US3.5, US5.5, US6.4) |
| Infrastructure (data access) | Unit | xUnit | Un-opened `NpgsqlConnection`; command shape, parameter names and types, row-mapping with and without optional values, cancellation propagation | Column mapping for each status and for a null description; parameter type correctness; cancellation token forwarded; no string-interpolated SQL (FR-006, FR-014) |
| Infrastructure | Integration | xUnit against real PostgreSQL 16 | Real database required; no in-memory substitute | Insert/read-back mapping; unique email violation translated to `409` (US1.2); not-null and check constraints (FR-014); foreign key `ON DELETE CASCADE` (FR-016); transaction rollback; migrations applied to an empty database; seed run twice without duplicates (US7.3, FR-012) |
| API controllers | Unit | xUnit with fake use cases | No HTTP pipeline | Status-code mapping per outcome: 201 with Location, 200, 204, 400, 401, 404, 409 (FR-017); identity taken from the claims object rather than the body (FR-009); ProblemDetails shape produced by the mapping layer |
| API endpoints | HTTP integration | xUnit + `WebApplicationFactory` per host | In-process real host, real PostgreSQL | Successful registration and login (US1.1, US1.4); duplicate email `409` (US1.2); invalid input `400` (US1.3); generic `401` for bad credentials (US1.5); anonymous, malformed, and expired token `401` (US2.2, US2.3); authorized access (US2.4); identity from claims only (US2.5); task CRUD success (US3.1, US3.2, US4.1, US5.1, US5.2, US6.1, US6.2); empty list (US3.3); cross-user `404` (US3.4, US5.4, US6.3); validation `400` (US4.2, US4.3, US5.3); past due date accepted (US4.4); owner field ignored (US4.5); unknown identifier `404` (US3.5, US5.5, US6.4); no password or hash in any response (NFR-001) |
| Frontend | Component and user flow | Vitest + React Testing Library + user-event | Fetch layer replaced by typed fakes; real components and real router | Loading, empty, success, validation, and error states (FR-011, US3.3); registration validation feedback (US1.3); login failure message (US1.5); one user-flow test covering login then create, view, update, and delete (US3 to US6, SC-002); `401` clears state and redirects (NFR-001 boundary) |
| Architecture | Unit | xUnit reflecting over assembly references | None | Dependency direction per the matrix above, and absence of prohibited packages (Principle II, EC-01) |

### Requirement coverage map

Every functional and non-functional requirement is verified somewhere; no requirement is covered by
the plan only in prose. `A` = Application unit, `D` = Domain unit, `IU` = Infrastructure unit,
`PG` = real-PostgreSQL integration, `C` = API controller unit, `H` = HTTP integration, `F` =
frontend component or flow test, `Q` = quickstart manual scenario.

| Requirement | Levels | Where it is asserted |
|---|---|---|
| FR-001 registration with unique email and policy | A, PG, H | validator outcomes; unique index violation to `409`; register endpoint cases |
| FR-002 authentication with generic failure | A, H | use case returns one failure outcome; login returns one generic `401` for both causes |
| FR-003 persisted user accounts | PG, Q | users row round-trip, including the credential hash; restart scenario in quickstart section 8 |
| FR-004 logout from the web application | F, H | auth context clears the credential and protected calls then return `401` |
| FR-005 list, retrieve, create, update, delete own tasks | A, H | use-case outcomes per operation; one HTTP test per verb per owner case |
| FR-006 required task fields and rules | D, PG, H | domain invariants; `NOT NULL` and `CHECK` violations; endpoint validation cases |
| FR-007 cross-user access denied without disclosure | A, PG, H | use case returns not-found for a non-owned record; query is owner-filtered; `404` for retrieve, update, and delete |
| FR-008 two API surfaces with public and protected endpoints | H | User API public endpoints answer anonymously; every Task API endpoint returns `401` without a token; `/api/users/me` requires a token |
| FR-009 ownership from authenticated identity only | A, C, H | `TaskDraft` has no owner; controller passes the claim-derived identity; supplied `ownerId` is ignored |
| FR-010 frontend supports the full flows | F | one user-flow test covering login, create, view, update, delete |
| FR-011 frontend states: loading, empty, success, validation, error | F | component tests asserting each state renders, and that a failed mutation never renders success |
| FR-012 seeded demonstration data, idempotent | PG, Q | seed run twice with no duplicates; quickstart section 3 |
| FR-013 three statuses, Pending default, any transition | D, PG, H | enum and validator; `CHECK` constraint; create default and both-direction update |
| FR-014 title, description, and due-date rules; past dates accepted | D, A, PG, H | domain and validator rules; `CHECK` constraints; create and update endpoint cases including a past date |
| FR-015 password policy at registration | A, H | validator boundary at 7 and 8 characters; register endpoint rejection |
| FR-016 delete removes the task from the collection | H, PG | `204` then `404`; row absent after commit |
| FR-017 distinguishable error categories | C, H | controller mapping table; assertions on `400`, `401`, `404`, `409` and on ProblemDetails shape with no leaked data |
| NFR-001 credential secrecy, validated identity, 100% cross-user denial | A, H, F | no password or hash in any response; explicit token validation parameters; expired/malformed `401`; claims-only identity; `401` handling in the client |
| NFR-002 accessibility | F, Q | labels associated with inputs, semantic buttons, accessible error messages; keyboard-only pass in quickstart section 6 |
| NFR-003 responsiveness | Q | manual narrow-width pass in quickstart section 6, plus a review that no fixed-width layout forces horizontal scrolling |
| NFR-004 reproducible startup from a clean checkout | PG, Q | migrate an empty database; quickstart section 8 from a fresh clone |
| NFR-005 users and tasks survive a restart | PG, Q | committed rows readable by a new connection; restart scenario in quickstart section 8 |
| NFR-006 mandated test levels exist for every requirement | all | this table plus the analysis phase check that every row has a corresponding task |

NFR-002, NFR-003, and NFR-005 rely partly on manual quickstart scenarios because a headless suite
cannot assert visual responsiveness or a real container restart. That is recorded here so the gap is
explicit rather than implied, and those scenarios are named in `quickstart.md`.

Also recorded:

- **Test environment**: Docker Compose provides PostgreSQL 16 with a health check and creates two
  databases, `taskmanager` for the application and `taskmanager_test` for tests. Integration and
  HTTP tests read `ConnectionStrings__TestDatabase`; when it is unset they fall back to the
  documented local default. A missing database **fails** the run; it never skips.
- **Fixtures**: an xUnit collection fixture applies all migrations once to `taskmanager_test`; each
  test opens its own transaction and rolls back, so tests are order-independent and leave no state.
  The seed-idempotency test runs the seed twice against a clean schema and asserts no duplicates.
- **Determinism**: `ITimeProvider` is injected. Token-expiry tests mint tokens with an explicit past
  `exp` through the same issuer, and validation uses `ClockSkew = TimeSpan.Zero`.
- **Test-first implementation strategy**: for every behavior the test is written first and run to
  observe an *assertion* failure. Where the compiler requires a symbol to exist before a test can
  compile, a minimal throwing stub is added first and must still produce a behavioral failure; a
  build or environment error is never recorded as RED.
- **Critical behaviors with at least one meaningful test each**: authentication, token validation
  and expiry, authorization and ownership, ownership-field tampering, input validation, duplicate
  registration, CRUD state transitions, persistence correctness, error shapes, and seed idempotency.

## Security and Persistence Design (Principle IV)

- **Authentication**: JWT bearer tokens issued by the User API. HS256 with a signing key read from
  configuration (`Jwt__SigningKey`, supplied as an environment variable; `.env.example` carries a
  placeholder). Issuer `ballastlane-user-api`, audience `ballastlane-clients`, lifetime 60 minutes.
  Both hosts set `ValidateIssuer`, `ValidateAudience`, `ValidateIssuerSigningKey`, and
  `ValidateLifetime` to `true`, use an explicit `TokenValidationParameters`, and set
  `ClockSkew = TimeSpan.Zero`. Missing, malformed, bad-signature, and expired tokens all produce
  `401`.
- **Passwords**: `PasswordHasher<User>` from `Microsoft.Extensions.Identity.Core`, reached through an
  Application-owned `IPasswordHasher` boundary so Application stays framework-neutral. The unique
  salt comes from the component, and verification uses its constant-time comparison. Parameters
  recorded here as the security standard requires: the .NET 8 default v3 PBKDF2-HMAC-SHA512 at
  210,000 iterations.
- **Authorization**: every endpoint is explicitly `[AllowAnonymous]` or covered by a
  protected-by-default policy. Ownership is enforced in the **Application use case**: each task use
  case receives an `AuthenticatedUser` value object built from validated claims and passes its
  `UserId` to the repository as a query filter, then verifies the loaded entity's owner. Cross-user
  access returns `404`, the single documented choice, so existence is never confirmed (FR-007).
  Identity is never read from a body or query field; a client-supplied `ownerId` is ignored (FR-009).
- **Validation**: shape validation (required fields, formats, lengths, enum membership, date-only due
  date) lives in Application validators and returns `400` with field-level ProblemDetails extensions.
  State conflicts return `409`. Documented status convention: `400` for shape, `401` for
  authentication, `404` for missing-or-not-owned, `409` for conflict. `422` is deliberately unused so
  one consistent choice exists.
- **Persistence**: two tables.
  - `users`: `id uuid PRIMARY KEY`, `email text NOT NULL`, `password_hash text NOT NULL`,
    `created_at timestamptz NOT NULL DEFAULT now()`, plus a unique index on `lower(email)` so
    case-insensitive uniqueness (PDC-04) is a database constraint rather than an application check.
  - `tasks`: `id uuid PRIMARY KEY`, `owner_id uuid NOT NULL REFERENCES users(id) ON DELETE CASCADE`,
    `title text NOT NULL CHECK (length(btrim(title)) BETWEEN 1 AND 200)`, `description text NULL
    CHECK (description IS NULL OR length(description) <= 2000)`, `status text NOT NULL CHECK
    (status IN ('Pending', 'InProgress', 'Completed')) DEFAULT 'Pending'`, `due_date date NOT NULL`,
    `created_at timestamptz NOT NULL DEFAULT now()`, `updated_at timestamptz NOT NULL DEFAULT now()`.
  - Index `ix_tasks_owner_due_created` on `(owner_id, due_date, created_at)` supporting the ordered
    list query (PDC-05). No further index is created: each one must map to a query that exists.
  - Migrations: `db/migrations/0001_create_users.sql` and `0002_create_tasks.sql`, applied by an
    apply-once runner that records versions in `schema_migrations`. Forward-only, never edited after
    being applied, and reproducible from an empty database.
  - Transactions: single-statement writes use the implicit transaction; the seed runs in one
    explicit transaction so a partial seed cannot persist.
- **Secrets**: `Jwt__SigningKey`, `ConnectionStrings__Database`, and `POSTGRES_PASSWORD` come from
  environment variables. `.env.example` is committed with placeholders and a comment per variable;
  `.env` is git-ignored. Demonstration credentials are intentionally public, documented in the
  README, and valid only against the local container (security standard 2.6).
- **Prohibited libraries**: an architecture/packaging test asserts that no `EntityFrameworkCore`,
  `Dapper`, `MediatR`, or `Mediator` package reference exists in any project, directly or
  transitively.

## GenAI Evidence Capture Points (Principle VI)

| Activity | Evidence record | Checkpoint expectations |
|---|---|---|
| Specification | `EV-0007` (complete) | Prompt, generated spec, review findings, validation results |
| Planning | `EV-0008` (this activity) | Verbatim prompt, design alternatives and the decisions taken, observed toolchain checks, post-design Constitution Check |
| Task generation | `EV-0009` or the activity's record | Prompt, task-to-requirement mapping, planned RED ordering |
| Analysis | Record for the analyze activity | Read-only report preserved as evidence; blocking findings listed |
| Implementation | One record per implementation group, named after the user story | Original generated code before correction, observed RED assertion failure and GREEN output, corrections with justification |
| Assessment API-generation exercise | Its own record, separate from feature work | Prompt, representative generated code, validation, corrections, edge-case handling |
| Review and converge | Record for the review activity | Findings, corrections, verified results |

## Project Structure

### Documentation (this feature)

```text
specs/001-task-manager/
├── plan.md              # This file
├── spec.md              # Feature specification
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── user-api.md
│   └── task-api.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Produced by /speckit-tasks
```

### Source Code (repository root)

```text
BallastLane.sln

src/
├── Domain/                        # BallastLane.Domain - no project dependencies
├── Application/                   # BallastLane.Application - depends on Domain only
│   ├── Abstractions/
│   ├── Users/
│   ├── Tasks/
│   └── Outcomes/
├── Infrastructure/                # BallastLane.Infrastructure - Npgsql adapters
│   ├── Persistence/
│   ├── Security/
│   └── Time/
├── SharedWeb/                     # BallastLane.SharedWeb - auth wiring + error mapping
├── UserApi/                       # BallastLane.UserApi - host 1
├── TaskApi/                       # BallastLane.TaskApi - host 2
└── Migrator/                      # BallastLane.Migrator - console entry point

tests/
├── unit/
│   ├── BallastLane.Domain.Tests/
│   ├── BallastLane.Application.Tests/
│   ├── BallastLane.Infrastructure.UnitTests/
│   ├── BallastLane.Api.UnitTests/
│   └── BallastLane.Architecture.Tests/
├── integration/
│   ├── BallastLane.PostgreSql.Tests/
│   └── BallastLane.Migrations.Tests/
└── http/
    ├── BallastLane.UserApi.HttpTests/
    └── BallastLane.TaskApi.HttpTests/

frontend/
├── src/
│   ├── api/
│   ├── auth/
│   ├── components/
│   ├── pages/
│   ├── state/
│   └── App.tsx
└── tests/
    ├── components/
    └── flows/

db/migrations/
├── 0001_create_users.sql
└── 0002_create_tasks.sql

docker-compose.yml
.env.example
```

**Structure Decision**: four backend layers as separate projects so dependency direction is enforced
by the compiler and verifiable by an architecture test. `Domain` has no project references,
`Application` references only `Domain`, `Infrastructure` references `Application` and `Domain`, and
the two hosts reference `Application` and `Domain` plus `Infrastructure` in `Program.cs` only. The
two API concerns are delivered as **two hosts** because the assessment requires a second API, and a
separate host makes that boundary explicit and independently demonstrable; `SharedWeb` holds
authentication wiring and error mapping used by both without holding business rules. `Migrator` is a
console entry point so `Infrastructure` contains no entry point and migration stays a deliberate,
auditable step rather than implicit startup work. The frontend separates API access (`api/`),
authentication state (`auth/`), presentation (`components/`, `pages/`), and list-state reduction
(`state/`), matching the frontend standard's separation rules.

## Complexity Tracking

| Deviation | Why needed | Simpler alternative rejected because | Approved by |
|---|---|---|---|
| Two API hosts instead of one host with two route groups | The assessment explicitly requires a second API; two hosts make the boundary observable and independently testable | A single host with route groups is smaller but leaves the "second API" requirement implicit, and the plan must demonstrate it | `Pending` - developer review |
| Separate `Migrator` console project | Keeps migration explicit, idempotent, and runnable ahead of both hosts | Running migrations at API startup couples schema change to process start, races between two hosts, and conflicts with "seeding separate from migrations" | `Pending` - developer review |
| `SharedWeb` project shared by both hosts | Authentication wiring and ProblemDetails mapping must be identical in both hosts | Copying the code into each host duplicates security configuration, which the architecture standard forbids | `Pending` - developer review |
| `ITimeProvider` abstraction | Timestamps and token expiry affect behavior and must be deterministic in tests | Using `DateTimeOffset.UtcNow` directly makes expiry and timestamp assertions time-dependent and flaky | `Pending` - developer review |

No deviation from a constitutional `MUST` is recorded here. All four entries are proportionality
notes, not exceptions to a principle.

## Post-Design Constitution Check

Re-evaluated after design:

| Principle | Result after design |
|---|---|
| I. Assessment-first | PASS - all 17 FRs and 6 NFRs retain a mapping path; no requirement dropped; the `/speckit-analyze` gate precedes implementation |
| II. Clean Architecture and SOLID | PASS - the matrix has no prohibited edge; every abstraction carries a stated seam; `SharedWeb` holds no business rules |
| III. TDD | PASS - seven test rows including Infrastructure unit and controller unit; RED-first ordering stated; no pair parallelizable |
| IV. Secure APIs and persistence | PASS - explicit token validation, claim-derived ownership in the Application layer, hashed passwords, parameterized SQL, database-enforced uniqueness and checks, prohibited libraries verified absent |
| V. Full-stack delivery | PASS - responsive accessible UI with all states, idempotent seed, Compose plus documented commands, README and GENAI.md obligations |
| VI. GenAI evidence | PASS - capture points assigned per activity, one record per activity or group |

## Adopted Specification Decisions

The specification records eight proposed product decisions requiring developer review. This plan
adopts their proposed defaults so implementation is unambiguous. Each remains `[AI-PROPOSED]` until
the developer confirms it, and changing one is a specification change, not a plan change.

| ID | Decision adopted in this plan |
|---|---|
| PDC-01 | Password policy: minimum 8 characters, no maximum below 128 |
| PDC-02 | Title maximum 200 characters, enforced by a `CHECK` and by the validator |
| PDC-03 | Description maximum 2,000 characters, enforced by a `CHECK` and by the validator |
| PDC-04 | Email uniqueness is case-insensitive, enforced by a unique index on `lower(email)` |
| PDC-05 | Task list ordered by `due_date` ascending, then `created_at` ascending |
| PDC-06 | One update operation changes title, description, status, and due date together |
| PDC-07 | Logout clears the client-side session credential; the server issues no revocation list |
| PDC-08 | Past due dates are stored and displayed unchanged; no automatic status change |

## Delivery Context

The submission deadline and the post-submission repository freeze are developer-owned process
obligations recorded in `docs/assessment/submission-requirements.md`. They are **not** engineering
rules, and this plan deliberately creates no gate, hook, or test around them. They matter only as
scope guidance: the specification's priority order (P1 first) determines what is delivered first, and
anything not delivered must be reported as not delivered rather than described as complete.
