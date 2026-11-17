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
| Node.js and npm | 25.9.0 / 11.12.1 (Node 20 or newer works) |
| Docker + Compose | 28.3.3 / Compose v2 |

Commands are given for both shells wherever they differ: **Bash** (macOS, Linux, WSL, Git Bash) and
**Windows PowerShell**. Run them from the repository root unless a step says otherwise.

## Setup

### 1. Create your local configuration

Bash:

```bash
cp .env.example .env
```

Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Then replace every placeholder in `.env`:

| Variable | Purpose |
| --- | --- |
| `POSTGRES_PASSWORD` | Password for the local PostgreSQL container |
| `ConnectionStrings__Database` | Runtime connection string; use `Host=postgres` inside Compose |
| `ConnectionStrings__TestDatabase` | Test connection string; use `Host=localhost` and the same password as `POSTGRES_PASSWORD` (see [Tests](#tests)) |
| `Jwt__SigningKey` | HMAC-SHA256 key for bearer tokens (at least 32 bytes) |
| `FrontendOrigin` | Origin allowed by CORS (defaults to the Vite dev server) |

`.env` is git-ignored. Never commit real credentials.

### 2. Start PostgreSQL, apply the migrations, and seed the demonstration data

```bash
docker compose up -d postgres
docker compose run --rm migrator --seed
```

Both commands are identical in PowerShell. The first waits for the container health check; the second
applies the ordered migration files under `backend/db/migrations` and then seeds the two
demonstration accounts. Re-running the seed never duplicates data and never fails.

### 3. Start the API hosts

```bash
docker compose up -d user-api task-api
```

- User API: `http://localhost:5081`
- Task API: `http://localhost:5082`
- OpenAPI documents: `http://localhost:5081/swagger` and `http://localhost:5082/swagger`

The compose file runs both hosts with `ASPNETCORE_ENVIRONMENT=Production`, which is the posture a
deployed environment has, and opts into Swagger explicitly with `Swagger__Enabled=true`. Swagger is
never implicit: a non-Development host serves the document only when that variable is `true`,
because it describes every route and is not itself protected. When you run a host with `dotnet run`
it starts in Development and serves the same document with no configuration.

### 4. Start the frontend

```bash
cd frontend
npm ci
npm run dev
```

Identical in PowerShell. `npm ci` installs exactly the locked versions from `package-lock.json`. The
app serves on `http://localhost:5173` and redirects an anonymous visitor to the login page. It calls
the two API hosts on the ports above by default; set `VITE_USER_API_URL` and `VITE_TASK_API_URL` to
move them.

### Running the APIs from the host instead of containers

Useful when you want a debugger or hot reload. The hosts then run in `Development`, so Swagger is
served at `/swagger` with no configuration. Point the runtime connection string at the published
container port and reuse the container password, then override the listening ports so the host runs
answer on the same ports as the containers:

Bash:

```bash
export ConnectionStrings__Database='Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=<POSTGRES_PASSWORD>'
export Jwt__SigningKey='<the Jwt__SigningKey value from .env>'

dotnet run --project backend/src/Migrator -- --seed
dotnet run --project backend/src/UserApi --urls http://localhost:5081
dotnet run --project backend/src/TaskApi --urls http://localhost:5082
```

Windows PowerShell:

```powershell
$env:ConnectionStrings__Database = 'Host=localhost;Port=5432;Database=taskmanager;Username=postgres;Password=<POSTGRES_PASSWORD>'
$env:Jwt__SigningKey = '<the Jwt__SigningKey value from .env>'

dotnet run --project backend/src/Migrator -- --seed
dotnet run --project backend/src/UserApi --urls http://localhost:5081
dotnet run --project backend/src/TaskApi --urls http://localhost:5082
```

Run the migrator first: both hosts assume the schema exists. Without `--urls`, each project uses the
port in its `Properties/launchSettings.json` (`5255` for User API, `5244` for Task API).

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

Both suites need PostgreSQL: the integration and HTTP tests run against the real container and
**fail** when it is unreachable rather than skipping. Start it first, then export the test connection
string. The four database-backed suites run inside `taskmanager_test` and give themselves a suffixed
database each (`taskmanager_test_userapi`, `_taskapi`, `_migrations`, `_postgres`), so the value is
the base database and the container creates all of them on first start.

Bash:

```bash
docker compose up -d postgres
export ConnectionStrings__TestDatabase='Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=<POSTGRES_PASSWORD>'

dotnet test -m:1

cd frontend && npm ci && npm run lint && npm run build && npm test
```

Windows PowerShell:

```powershell
docker compose up -d postgres
$env:ConnectionStrings__TestDatabase = 'Host=localhost;Port=5432;Database=taskmanager_test;Username=postgres;Password=<POSTGRES_PASSWORD>'

dotnet test -m:1

Set-Location frontend
npm ci
npm run lint
npm run build
npm test
```

If `ConnectionStrings__TestDatabase` is not set, the suites fall back to a `Password=postgres`
connection string, which only works when the container was created with that password. A wrong
password or a missing container fails the run; nothing is skipped.

`dotnet test -m:1` runs one test project at a time. Each suite already owns its own database, but on a
resource-constrained container host the HTTP and PostgreSQL suites can exhaust the Docker Desktop
connection forwarding when several run at once, which surfaces as a spurious connect timeout. The
sequential form is deterministic; add `-m:2` or drop the flag on a machine with headroom.

On a clean checkout the same commands install everything they need: `dotnet test` restores and builds
the solution, and `npm ci` installs the locked frontend dependencies. No separate restore step and no
global tool is required.

| Suite | Level | What it covers |
| --- | --- | --- |
| `backend/tests/unit/BallastLane.Domain.Tests` | unit | Email and task rules, statuses, identifiers |
| `backend/tests/unit/BallastLane.Application.Tests` | unit | Registration, login, task CRUD outcomes, ownership, validation |
| `backend/tests/unit/BallastLane.Infrastructure.UnitTests` | unit | Migration ordering, connection factory, SQL shape and row mapping |
| `backend/tests/unit/BallastLane.Api.UnitTests` | unit | Outcome → status mapping, token validation parameters, OpenAPI exposure rule |
| `backend/tests/unit/BallastLane.Architecture.Tests` | unit | Dependency direction, prohibited libraries |
| `backend/tests/integration/BallastLane.PostgreSql.Tests` | integration | Real SQL, constraints, ordering, cancellation |
| `backend/tests/integration/BallastLane.Migrations.Tests` | integration | Schema from an empty schema, seed idempotency, demonstration dataset |
| `backend/tests/http/BallastLane.UserApi.HttpTests` | HTTP | Registration, login, token failures, access boundary, served OpenAPI document |
| `backend/tests/http/BallastLane.TaskApi.HttpTests` | HTTP | CRUD, ownership isolation, validation, injection payloads, served OpenAPI document |
| `frontend/tests` | component and flow | Form validation, auth state, list states, delete confirmation, dialog keyboard operation, session expiry and logout token clearing, stale-token isolation |

Every one of these suites passes on a clean checkout; the observed results of the last full run are
recorded in [docs/evidence/genai/EV-0026-delivery-readiness-corrections.md](docs/evidence/genai/EV-0026-delivery-readiness-corrections.md).

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
- `npm audit` reports two moderate advisories in `react-router-dom` 6.30 (an open redirect through a
  backslash in `Link`/`useNavigate`, and an SSR hydration issue). The fix requires the React Router 7
  major upgrade, which is out of scope here. This client renders in the browser only, never uses SSR,
  and every navigation target is a literal route or an API-supplied task identifier, so neither
  advisory describes a reachable path in this application.
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
