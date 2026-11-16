# Ballast Lane Technical Interview Exercise V5 — Personal Task Manager

A small full-stack Task Manager built for the Ballast Lane Senior .NET technical assessment: an
ASP.NET Core backend with **two API hosts** over a shared domain and application core, PostgreSQL
persistence through **Npgsql + ADO.NET (no ORM)**, and a **React + TypeScript** frontend covering
registration, login, logout, and task CRUD with explicit loading, empty, success, validation, and error
states.

Generative AI usage, prompts, and the review of generated output are documented in
[GENAI.md](./GENAI.md) and the evidence records under [docs/evidence/genai/](./docs/evidence/genai/).

## Repository layout

The two halves of the application are grouped symmetrically, so every command in this document has an
unambiguous target:

```text
backend/
  src/      UserApi, TaskApi, Migrator, Domain, Application, Infrastructure, SharedWeb
  tests/    unit, integration, http and architecture suites
  db/       ordered SQL migrations and the seed runner's data
frontend/   React + TypeScript client, its tests, and its build
docs/       assessment, engineering standards, and GenAI evidence
specs/      the specification, plan, and task list for the feature
```

`BallastLane.sln`, `docker-compose.yml`, `Directory.Build.props`, and `.dockerignore` stay at the
repository root because they describe the whole repository rather than one side of it.

## The user story

A single person registers an account and signs in. They keep a personal list of tasks. Each task has a
title, an optional description, one of three statuses (`Pending`, `InProgress`, `Completed`), and a
calendar due date. They can create, read, update, and delete their own tasks, change status in any
direction, and never see another user's tasks. Past due dates are accepted; the application does not
change a status automatically.

## Architecture

```text
backend/src
  UserApi  ─┐                    ┌─ Domain          (entities, value objects, task rules)
  TaskApi  ─┼─► Application ◄────┤
  Migrator ─┘   (use cases,      └─ Infrastructure  (Npgsql adapters, hasher, token issuer, seeder)
                 boundaries)
                SharedWeb  (authentication wiring, ProblemDetails mapping, used by both hosts)
```

| Project | Responsibility | References |
| --- | --- | --- |
| `backend/src/Domain` | Entities, value objects (`UserId`, `TaskId`, `Email`, `TaskStatus`, `TaskDraft`), task rules | none |
| `backend/src/Application` | One use case per operation, validators, outcome types, boundary interfaces | Domain |
| `backend/src/Infrastructure` | `Npgsql*Repository`, `SchemaMigrator`, `IdentityPasswordHasher`, `JwtTokenIssuer`, `DemoSeeder` | Application, Domain |
| `backend/src/SharedWeb` | JWT bearer wiring, default-deny policy, claim mapping, outcome → ProblemDetails mapping | Application, Domain |
| `backend/src/UserApi` | Host 1: registration, login, current user, public info | Application, Domain, Infrastructure, SharedWeb |
| `backend/src/TaskApi` | Host 2: task CRUD for the authenticated user | Application, Domain, Infrastructure, SharedWeb |
| `backend/src/Migrator` | Console entry point: apply migrations, optionally seed | Infrastructure |
| `frontend/` | React SPA: API clients, auth context, pages, components, list-state reducer | HTTP contract only |

Dependencies point inward. `Domain` and `Application` reference no ASP.NET Core, Npgsql, or frontend
assembly, and `Infrastructure` is referenced only by the three composition roots. An architecture test
(`backend/tests/unit/BallastLane.Architecture.Tests`) enforces both rules by reading the project files.

## Prerequisites

| Requirement | Version used |
| --- | --- |
| .NET SDK | 10.0.404 (`net10.0`, LTS) |
| Node.js | 25.9.0 (20 or newer works) |
| Docker + Compose | 28.3.3 / Compose v2 |

## Setup

1. Copy the example configuration and fill in the values:

   ```bash
   cp .env.example .env
   ```

   | Variable | Purpose |
   | --- | --- |
   | `POSTGRES_PASSWORD` | Password for the local PostgreSQL container |
   | `ConnectionStrings__Database` | Runtime connection string; use `Host=postgres` inside Compose |
   | `ConnectionStrings__TestDatabase` | Test connection string; use `Host=localhost` from the host |
   | `Jwt__SigningKey` | HMAC-SHA256 key for bearer tokens (at least 32 bytes) |
   | `FrontendOrigin` | Origin allowed by CORS (defaults to the Vite dev server) |

   `.env` is git-ignored. Never commit real credentials.

2. Start the database and apply migrations plus the demonstration seed:

   ```bash
   docker compose up -d postgres
   docker compose run --rm migrator --seed
   ```

3. Start the APIs:

   ```bash
   docker compose up -d user-api task-api
   ```

   - User API: `http://localhost:5081` (Swagger at `/swagger`)
   - Task API: `http://localhost:5082` (Swagger at `/swagger`)

4. Start the frontend:

   ```bash
   cd frontend
   npm ci
   npm run dev
   ```

   The app serves on `http://localhost:5173` and redirects an anonymous visitor to the login page.

### Running the APIs from the host instead of containers

```bash
# from the repository root, with ConnectionStrings__Database pointing at localhost
dotnet run --project backend/src/Migrator -- --seed
dotnet run --project backend/src/UserApi
dotnet run --project backend/src/TaskApi
```

## Demonstration credentials

> **Demonstration only.** These accounts exist to make the walkthrough reproducible. They are
> intentionally public, valid only against the local container, and must never be reused for anything
> real.

| Email | Password | Seeded tasks |
| --- | --- | --- |
| `demo.user@example.test` | `DemoPassw0rd!` | 3 |
| `reviewer@example.test` | `ReviewPassw0rd!` | 2 |

Re-running the seed never duplicates data and never fails.

## API surface

### User API (`http://localhost:5081`)

| Method | Route | Access | Purpose |
| --- | --- | --- | --- |
| `POST` | `/api/users/register` | public | Create an account (`201`, `400`, `409`) |
| `POST` | `/api/users/login` | public | Exchange credentials for a bearer token (`200`, `400`, `401`) |
| `GET` | `/api/users/me` | protected | The caller's own identity (`200`, `401`) |
| `GET` | `/api/public/info` | public | Demonstrates a deliberately public endpoint |

### Task API (`http://localhost:5082`)

Every route is protected; a request without a valid token receives `401` before any data is read.

| Method | Route | Responses |
| --- | --- | --- |
| `GET` | `/api/tasks` | `200` (ordered by due date, then creation time, then id), `401` |
| `GET` | `/api/tasks/{id}` | `200`, `401`, `404` for unknown, non-UUID, or another user's task |
| `POST` | `/api/tasks` | `201` with `Location`, `400`, `401` |
| `PUT` | `/api/tasks/{id}` | `200`, `400`, `401`, `404` |
| `DELETE` | `/api/tasks/{id}` | `204`, `401`, `404` |

Contract detail: [specs/001-task-manager/contracts/](./specs/001-task-manager/contracts/).
Error status convention: `400` shape validation, `401` authentication, `404` missing-or-not-owned,
`409` conflict. `422` is deliberately unused.

## Tests

```bash
# backend: unit, real-PostgreSQL integration, and HTTP integration
dotnet test -m:1

# frontend: lint, type check, production build, component tests
cd frontend && npm run lint && npm run build && npm test
```

`-m:1` runs one test project at a time. Each suite already owns its own database, but on a
resource-constrained container host the HTTP and PostgreSQL suites can exhaust the Docker Desktop
connection forwarding when several run at once, which surfaces as a spurious connect timeout. The
sequential form is deterministic; add `-m:2` or drop the flag on a machine with headroom.

Integration and HTTP tests require the PostgreSQL container and **fail** when it is missing rather
than skipping:

```bash
docker compose up -d postgres
```

| Suite | Level | What it covers |
| --- | --- | --- |
| `backend/tests/unit/BallastLane.Domain.Tests` | unit | Email and task rules, statuses, identifiers |
| `backend/tests/unit/BallastLane.Application.Tests` | unit | Registration, login, task CRUD outcomes, ownership, validation |
| `backend/tests/unit/BallastLane.Infrastructure.UnitTests` | unit | Migration ordering, connection factory, SQL shape and row mapping |
| `backend/tests/unit/BallastLane.Api.UnitTests` | unit | Outcome → status mapping, token validation parameters |
| `backend/tests/unit/BallastLane.Architecture.Tests` | unit | Dependency direction, prohibited libraries |
| `backend/tests/integration/BallastLane.PostgreSql.Tests` | integration | Real SQL, constraints, ordering, cancellation |
| `backend/tests/integration/BallastLane.Migrations.Tests` | integration | Schema from an empty schema, seed idempotency, demonstration dataset |
| `backend/tests/http/BallastLane.UserApi.HttpTests` | HTTP | Registration, login, token failures, access boundary |
| `backend/tests/http/BallastLane.TaskApi.HttpTests` | HTTP | CRUD, ownership isolation, validation, injection payloads |
| `frontend/tests` | component | Form validation, auth state, list states, delete confirmation |

## Engineering rationale

- **Two API hosts instead of one.** The assessment requires a second API; separate hosts make the
  boundary observable and independently testable.
- **A separate `Migrator` console project.** Schema change stays an explicit, auditable step and never
  races between two hosts at startup; seeding is a separate opt-in action.
- **`SharedWeb`.** Authentication wiring and error mapping are identical in both hosts; duplicating
  security configuration is worse than sharing it.
- **Boundaries only where there is a seam.** `IPasswordHasher`, `ITimeProvider`, `ITokenIssuer`,
  `IUserReader`/`IUserWriter`, `ITaskReader`/`ITaskWriter`, `IDbConnectionFactory`, and
  `IMigrationStore` exist for dependency inversion and testability. There is no generic repository, no
  unit of work, and no mediator.
- **No ORM.** All SQL is parameterized and lives in `Infrastructure`; the schema is defined by ordered
  migration files and constrained in the database (unique `lower(email)`, `CHECK` constraints, foreign
  key with `ON DELETE CASCADE`, owner/due-date index).
- **Ownership is enforced in the Application layer** from validated claims and again in SQL
  (`WHERE ... owner_id = @ownerId`). Cross-user access is `404`, so existence is never confirmed.
- **Credential handling.** Passwords use ASP.NET Core Identity's `PasswordHasher<T>` (PBKDF2-HMAC,
  unique salt, constant-time verification). The bearer token lives in memory only and is never written
  to browser storage.
- **Frontend separation.** API access (`api/`), authentication state (`auth/`), presentation
  (`components/`, `pages/`), and list reduction (`state/`) are separate. A `401` clears the session and
  redirects to login.

## Known limitations

- Single-user personal context; no sharing, roles, or administration.
- No account lifecycle: no password reset, email change, email verification, or account deletion.
- No server-side token revocation (logout clears the client session only).
- The access token is not refreshed; after 60 minutes the user signs in again.
- No pagination, filtering, or search beyond listing one's own tasks.
- Concurrency between two simultaneous edits is not resolved (last write wins).
- The demonstration credentials are public by design and valid only locally.
- Responsiveness, keyboard operation, and a clean browser console are verified manually through
  [specs/001-task-manager/quickstart.md](./specs/001-task-manager/quickstart.md) section 6; a headless
  suite cannot assert them.

## Documentation

- Assessment text: [docs/assessment/technical-exercise-v5.md](./docs/assessment/technical-exercise-v5.md)
- Engineering standards: [docs/engineering/README.md](./docs/engineering/README.md)
- Constitution: [.specify/memory/constitution.md](./.specify/memory/constitution.md)
- Feature specification, plan, tasks, contracts: [specs/001-task-manager/](./specs/001-task-manager/)
- Generative AI response: [GENAI.md](./GENAI.md)
- GenAI evidence records: [docs/evidence/genai/](./docs/evidence/genai/README.md)
