# EV-0026 - Final delivery-readiness corrections

> **Scope note.** This record covers the six delivery-readiness corrections requested in the prompt
> below, their TDD cycles, and the fresh-checkout verification. It was written while the work happened:
> each section was appended when the evidence it describes was observed, including the failures.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0026` |
| Date | `2026-11-16` |
| Development phase | 11 - Final delivery readiness (documentation, configuration, session handling) |
| Activity type | `Documentation and specification`, `Code generation`, `Testing`, and `Review` |
| Related specification | `specs/001-task-manager/spec.md` (US1, FR-004, FR-010, FR-011, NFR-004, PDC-07) |
| Related task identifiers | `T028`, `T034` (session handling), `T040` (host configuration), `T097` to `T106` (review and convergence) |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Reviewed` |
| Commit references | `0d6c2b9` - `fix(web): keep a stale 401 from ending the current session` - the frontend guard and its regression test. `02747ee` - `fix(api): serve Swagger in Development or on explicit opt-in` - the shared rule, both hosts, the compose configuration, the four backend test files, and the two contracts. `df7c374` - `docs: make the setup guide reliable and record delivery readiness` - the README, quickstart, contract status, and this record as first written. Every reference was recorded only after that commit existed. |

## Actual prompt

Verbatim developer prompt that opened the activity:

```text
Perform the final delivery-readiness corrections for feature 001-task-manager.

Follow the existing constitution, engineering standards, and genai-evidence skill.

1. Make README.md a reliable clean-clone setup and test guide for Windows PowerShell and Bash, including PostgreSQL test connection-string configuration.
2. Synchronize quickstart.md with the actual .NET 10 implementation, migration/seed commands, and frontend startup.
3. Resolve the Swagger environment mismatch between docker-compose.yml, the API hosts, and README.md. Keep production security appropriate.
4. Prevent a delayed HTTP 401 from an old bearer token from invalidating a newer authenticated session. Add a regression test following RED-GREEN-REFACTOR.
5. Update the outdated implementation status in docs/assessment/submission-requirements.md without changing the original requirements.
6. Verify a fresh-checkout installation, backend tests, frontend tests, and the full application journey. Record actual results and any limitations.
Keep changes minimal. No new frameworks, architecture changes, or GitHub Actions.

Do not commit or push until developer review.
```

## Relevant context

- Requirements or assessment sections: the original assessment
  (`docs/assessment/technical-exercise-v5.md`), the submission requirements
  (`docs/assessment/submission-requirements.md`), and `specs/001-task-manager/spec.md`
  (FR-004 logout, FR-010 complete journey, FR-011 user-visible states, NFR-004 clean-checkout
  reproducibility, PDC-07 authenticated session).
- Constitution: Principle III (RED, GREEN, REFACTOR), Principle IV (secure APIs, explicit
  authentication checks), Principle V (reproducible delivery, README content), Principle VI
  (evidence).
- Engineering standards: `testing-and-tdd.md` section 3, `delivery-and-ci.md`,
  `api-standards.md` (OpenAPI documentation), `security.md` (credential handling, no secret
  exposure), `frontend-standards.md` sections 2.4 and 2.7, `genai.md`.
- Files supplied as context by the workspace: `README.md`, `specs/001-task-manager/quickstart.md`,
  `docker-compose.yml`, `.env.example`, `backend/src/UserApi/Program.cs`,
  `backend/src/TaskApi/Program.cs`, `frontend/src/api/client.ts`,
  `frontend/src/api/authToken.ts`, `frontend/src/auth/AuthProvider.tsx`,
  `frontend/tests/flows/session.test.tsx`, `docs/assessment/submission-requirements.md`, `GENAI.md`,
  and `.specify/memory/constitution.md`.
- Repository state at the start: working tree clean at `b15483c`; PostgreSQL container
  `ballastlane-postgres` healthy on port 5432; frontend `node_modules` present.

## Original AI-generated output

Recorded below per correction, before any correction was applied.

### Correction 4 - delayed 401 for an old token (frontend)

The defect is in code produced in `EV-0025`, so the pre-correction excerpt is the existing
`request` function:

```ts
  if (!response.ok) {
    const failure = await toFailure(response);

    // One place decides that an authenticated credential is no longer usable, so every consumer
    // observes the same session end instead of each screen inventing its own redirect.
    if (failure.status === 401 && options.accessToken) {
      invalidateSession();
    }

    return { ok: false, failure };
  }
```

Any `401` carrying *any* token ended the session, including a token that had already been replaced
by a newer sign-in. New test code, added before the fix, generated as:

```tsx
  it("keeps the newer session when a delayed 401 answers the previous token", async () => {
    const stale = deferredResponse();
    installFetch({
      ...signInRoutes,
      // The manual call below consumes the deferred answer; the signed-in list request reads the task.
      "GET /api/tasks": [() => stale.promise, () => json(200, [task])],
    });

    setAccessToken("previous-token");
    const staleRequest = tasksApi.list();

    render(<App />);
    await signIn();

    await screen.findByRole("heading", { name: /your tasks/i });
    expect(getAccessToken()).toBe("issued-token");

    stale.release(unauthorized());
    await staleRequest;

    expect(await screen.findByRole("heading", { name: /your tasks/i })).toBeInTheDocument();
    expect(screen.queryByText(/session has expired/i)).not.toBeInTheDocument();
    expect(getAccessToken()).toBe("issued-token");
  });
```

### Correction 3 - OpenAPI exposure (backend)

The pre-correction hosts gated Swagger on the environment alone, which is why the container stack
served nothing at the `/swagger` address the README advertised:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

The first test written for this correction referenced a helper that did not exist, which produced a
compilation error rather than a behavioral failure:

```text
SwaggerAvailabilityTests.cs(16,21): error CS0103: The name 'SwaggerSetup' does not exist in the current context
```

A compilation failure is not valid RED evidence, so the current rule was first extracted verbatim
into the shared seam (a behavior-preserving refactor), and the RED was then observed behaviorally
against that extraction. The compile error above is recorded as an observed fact and is **not**
counted as the RED checkpoint.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | `SwaggerSetup` lives in `SharedWeb`, which both hosts already reference; no new dependency edge, no reference from `Domain` or `Application`. | `[VERIFIED]` - architecture tests pass |
| SOLID | The exposure rule has one owner instead of two copies that could drift; the frontend guard keeps session invalidation in the single transport it already belonged to. | `[VERIFIED]` |
| TDD | RED and GREEN observed for corrections 3 and 4; documentation-only corrections marked below. | `[VERIFIED]` - see Commands and observed results |
| Security | Swagger remains off in any non-Development environment unless explicitly enabled; the demo compose stack opts in on loopback ports only, and keeps `ASPNETCORE_ENVIRONMENT=Production` so no developer exception page is exposed. No secret is documented or committed. | `[VERIFIED]` - containers report `Hosting environment: Production` |
| Functional requirements | Session isolation (FR-004, PDC-07), reproducible setup (NFR-004), OpenAPI availability per contract. | `[VERIFIED]` |

## Commands and observed results

### Baseline before any change

```text
cd frontend; npm test
```

```text
Test Files  10 passed (10)
     Tests  40 passed (40)
```

### Correction 4 - RED

```text
cd frontend; npx vitest run tests/flows/session.test.tsx
```

```text
 ❯ tests/flows/session.test.tsx:156:25
    154|     await staleRequest;
    155|
    156|     expect(await screen.findByRole("heading", { name: /your tasks/i })…
       |                         ^
 Test Files  1 failed (1)
      Tests  1 failed | 6 passed (7)
```

The failure rendered the login page: the delayed `401` had ended the newer session. Failure observed
before the guard existed.

### Correction 4 - GREEN

```text
cd frontend; npx vitest run tests/flows/session.test.tsx
```

```text
 ✓ tests/flows/session.test.tsx (7 tests) 6760ms
   ✓ session handling > keeps the newer session when a delayed 401 answers the previous token 768ms
 Test Files  1 passed (1)
      Tests  7 passed (7)
```

### Correction 3 - behavioral RED

```text
dotnet test backend/tests/unit/BallastLane.Api.UnitTests --nologo
```

```text
  Failed BallastLane.Api.UnitTests.SwaggerAvailabilityTests.A_shipped_environment_honours_an_explicit_opt_in(configured: "true", expected: True)
  Error Message:
   Assert.Equal() Failure: Values differ
Expected: True
Actual:   False
Failed!  - Failed:     1, Passed:    22, Skipped:     0, Total:    23
```

### Correction 3 - GREEN

```text
dotnet test backend/tests/unit/BallastLane.Api.UnitTests --nologo
```

```text
Passed!  - Failed:     0, Passed:    23, Skipped:     0, Total:    23
```

```text
dotnet test backend/tests/http/BallastLane.UserApi.HttpTests --nologo
```

```text
Passed!  - Failed:     0, Passed:    24, Skipped:     0, Total:    24
```

```text
dotnet test backend/tests/http/BallastLane.TaskApi.HttpTests --nologo
```

```text
Passed!  - Failed:     0, Passed:    54, Skipped:     0, Total:    54
```

### Frontend checks after the fix

```text
cd frontend; npm run lint; npm run build; npm test
```

```text
eslint .   -> no output, no findings
✓ built in 1.73s
Test Files  10 passed (10)
     Tests  41 passed (41)
```

### OpenAPI in the container stack

```text
docker compose build migrator user-api task-api
docker compose up -d user-api task-api
```

```text
ballastlane-task-manager-migrator  Built
ballastlane-task-manager-user-api  Built
ballastlane-task-manager-task-api  Built
```

```text
user-api swagger: 200 application/json; charset=utf-8
task-api swagger: 200 application/json; charset=utf-8
user-api swagger ui: 200
```

`docker logs ballastlane-task-manager-user-api-1` reported `Hosting environment: Production`, which
confirms the Swagger opt-in is what makes the document available and not a Development environment.

### Documented host-run command

The README and quickstart now tell the reader to start a host with `dotnet run ... --urls
http://localhost:5081`. That forwarding was verified on a free port so the running container kept its
own:

```text
dotnet run --project backend/src/UserApi --urls http://localhost:5299
```

```text
public info: 200 {"service":"ballastlane-user-api","status":"ok"}
swagger on host run: 200
```

`--urls` is honoured, and a host-run instance is in Development, so it serves the OpenAPI document
with no configuration, as both guides now state. The process and its listener were terminated
afterwards, and the port was confirmed free.

### Application journey in the browser

Driven through the Chromium-based integrated browser against the container stack and the Vite dev
server:

| Step | Observed |
| --- | --- |
| Open `http://localhost:5173` anonymously | Redirected to `/login` |
| Log in with `demo.user@example.test` | `/tasks` shows the three seeded tasks |
| Create a task (title, due date) | Appears in the list without a page reload |
| Open the task and edit its status to `Completed` | Detail view shows `Completed` |
| Delete with confirmation | Confirmation dialog, then the list returns to the three seeded tasks |
| Server-side check afterwards | `task count: 3`, matching the seed |
| `GET /api/tasks` with no token | `401` |
| Log out, then navigate to `/tasks` | Redirected to `/login`; no task request is issued |
| Console during login, list, and logout | no messages, no page errors |

One incidental observation: the first login attempt against the freshly started container failed with
`net::ERR_CONNECTION_RESET`, and the interface reported "The server could not be reached. Check that
the API is running and try again." Retrying succeeded. This is the documented connection-failure
state behaving correctly, not a defect.

On both delete confirmations the browser reported a `requestFailed` event with `net::ERR_ABORTED` for
the `DELETE` request while the client navigated away from the detail route immediately after the
`204`. The row disappeared, and the server-side count afterwards was three tasks (the seeded
dataset), which proves the delete took effect. No error was shown to the user. The cause of the
event was not investigated further; it is recorded rather than explained.

### Fresh-checkout verification

A clone would not have contained the uncommitted corrections, so the fresh tree was produced from
exactly what a commit of this work would contain: every tracked file plus every untracked,
non-ignored file, copied to a temporary directory outside the repository, with no `.env`, no
`bin`, `obj`, or `node_modules`, and no prior containers or volumes (`docker compose down -v` was run
first).

```text
git ls-files -co --exclude-standard | <copy each file to the temporary tree>
copied files: 273
Test-Path .env -> False
```

Setup, following `README.md` literally:

```text
Copy-Item .env.example .env
docker compose build
docker compose up -d postgres
docker compose run --rm migrator --seed
docker compose up -d user-api task-api
```

```text
ballastlane-task-manager-user-api  Built
ballastlane-task-manager-task-api  Built
ballastlane-task-manager-migrator  Built
Migrations applied from '/app/db/migrations'.
Demonstration data seeded.
```

```text
GET http://localhost:5081/swagger/v1/swagger.json -> 200
GET http://localhost:5082/swagger/v1/swagger.json -> 200
seeded tasks for demo.user: 3
seeded tasks for reviewer: 2
```

Backend suites in the fresh tree, with `ConnectionStrings__TestDatabase` exported from that tree's own
`.env`:

```text
dotnet test BallastLane.sln -m:1
```

```text
BallastLane.TaskApi.HttpTests            54 passed
BallastLane.UserApi.HttpTests            24 passed
BallastLane.Migrations.Tests              9 passed
BallastLane.PostgreSql.Tests              7 passed
BallastLane.Api.UnitTests                23 passed
BallastLane.Application.Tests            44 passed
BallastLane.Architecture.Tests            9 passed
BallastLane.Domain.Tests                 31 passed
BallastLane.Infrastructure.UnitTests     18 passed
Failed: 0 - Total: 219
```

Frontend in the fresh tree:

```text
npm ci; npm run lint; npm run build; npm test
```

```text
npm ci -> installed from package-lock.json
eslint . -> no findings
✓ built in 3.52s
Test Files  10 passed (10)
     Tests  41 passed (41)
```

Application journey in the fresh tree, driven in the browser against its own stack and its own Vite
dev server, signed in as `reviewer@example.test`:

| Step | Observed |
| --- | --- |
| Log in | `/tasks` lists the two seeded tasks |
| Create "Fresh checkout task" | Third row appears without a reload |
| Delete it with confirmation | Back to the two seeded rows |
| Log out | `/login`; console has no messages and no page errors |

```text
npm audit --omit=dev
```

```text
react-router  6.0.0 - 7.17.0
Severity: moderate
2 moderate severity vulnerabilities
fix available via `npm audit fix --force` (react-router-dom@7.18.5)
```

Recorded as a known limitation in `README.md`. The advisory fix is a major-version upgrade and was
deliberately not performed: it is outside the requested scope, and the two described paths (a
backslash open redirect and SSR hydration) are not reachable in a browser-only client that navigates
only to literal routes and server-supplied task identifiers. The full `npm audit` also reports
moderate advisories in `vitest`, `vite`, and `esbuild`, all development-only tooling.

### Documentation consistency checks

```text
<relative link resolution over every changed Markdown file>
broken links: 0
```

Every non-ASCII character in the changed files was checked: the new evidence record contains only the
`❯`, `…`, and `✓` characters that appear inside preserved command output, and the two pre-existing em
dashes in `README.md` and `GENAI.md` are part of the documents' existing style.

### Restart persistence, as quickstart section 8 describes it

```text
docker compose down
docker compose up -d user-api task-api
```

```text
postgres   Up 29 seconds (healthy)
task-api   Up 18 seconds
user-api   Up 18 seconds

after restart, demo.user tasks: 3
GET http://localhost:5082/swagger/v1/swagger.json -> 200
```

Bringing up only the two API services also starts PostgreSQL and re-runs the migrator through the
declared dependencies, which is what step 4 of section 8 tells the reader to expect, and the seeded
data survives the restart through the named volume.

## Review findings

- `[VERIFIED]` The delayed-`401` defect was reproduced deterministically: a `401` for a replaced token
  ended the session that had replaced it, because the guard tested only that *a* token had been sent.
- `[AI-PROPOSED]` The OpenAPI RED is partially out of strict order and is reported as such: the test
  could not compile before the seam existed, and the constitution rejects a compilation failure as RED
  evidence, so the existing rule was first extracted unchanged into `SwaggerSetup`, and the behavioral
  RED was then observed against that extraction (`Expected: True, Actual: False`) before the opt-in was
  added. No behavior existed in the seam that the test had not already described.
- `[VERIFIED]` The README advertised `/swagger` on both containerised hosts while the hosts served it
  only in Development and the compose file set no environment at all, so the container default was
  Production and the address returned nothing. The claim was not merely unverified; it was wrong.
- `[VERIFIED]` `quickstart.md` claimed `docker compose up --build` seeds the demonstration data. It
  does not: the seed runs only with `--seed`, so section 3 of the same document could not succeed.
- `[VERIFIED]` `quickstart.md` still listed .NET SDK 8.0.x and did not mention the test connection
  string, which the four database-backed suites require; running them from a clean shell fails with a
  password error rather than skipping.
- `[VERIFIED]` `submission-requirements.md` section 4 still reported the backend, the tests, the
  README, and the GenAI answers as `PENDING`.
- `[AI-PROPOSED]` Choose an explicit opt-in flag over `ASPNETCORE_ENVIRONMENT=Development` in the
  compose file. Enabling Development in the container would also enable the developer exception page,
  which would leak stack traces on an unhandled error, and the request was to keep production security
  appropriate.
- `[AI-PROPOSED]` Extract the exposure rule into `SharedWeb` rather than duplicating the condition in
  two hosts, following the existing `AuthenticationSetup` precedent, which makes the rule unit-testable
  without a web host.
- `[VERIFIED]` `.github/copilot-instructions.md` also carries an outdated status paragraph ("No feature
  specification, plan, or task list exists yet", "the REST API generation exercise ... pending"). It is
  outside the six requested corrections, so it was left unchanged and is recorded here as a
  non-blocking documentation finding for the developer.
- `[VERIFIED]` `npm audit --omit=dev` reports two moderate `react-router` advisories. Correcting them
  requires a major upgrade, which the prompt excluded; the finding is documented in `README.md`
  instead of being silently dropped.

## Developer decisions and approval

The developer reviewed the diff and instructed the commit on 2026-11-17, which accepts the four
decisions below. The design alternatives that were rejected are recorded with the rationale that
rejected them.

| Decision | Rationale | Approval |
| --- | --- | --- |
| Guard session invalidation on the token still being the active one | A rejected credential only says that *that* credential is unusable, not that the session built on a different credential is over | `Approved` |
| Keep Swagger off in Production unless `Swagger__Enabled=true` | An unauthenticated document that enumerates every route should not be served implicitly; the local demo opts in without weakening the container environment. Rejected alternative: `ASPNETCORE_ENVIRONMENT=Development` in compose, which would also enable the developer exception page | `Approved` |
| Document both shells instead of assuming Bash | The assessment will be reviewed on an unknown platform, and the earlier guide's `cp` and `export` steps fail in Windows PowerShell | `Approved` |
| Leave the React Router major upgrade undone and document it | The prompt forbids scope expansion; a major dependency upgrade carries migration and regression risk into a submission-frozen repository | `Approved` |

Basis: the developer's commit instruction of 2026-11-17. No approval of this record's *text* is
implied, and the two non-blocking findings at the end of "Review findings" remain open.

## Corrections and improvements

| File | Change | Reason | Technical justification |
| --- | --- | --- | --- |
| `frontend/src/api/client.ts` | Invalidates the session only when `options.accessToken === getAccessToken()` | Correction 4 | The previous condition asked whether a request had carried a token, not whether that token was still the session's; a late `401` from a replaced token destroyed the current session |
| `frontend/tests/flows/session.test.tsx` | New deferred-response test plus a promise-capable responder table | Correction 4 | Only a delayed answer can reproduce the overlap between an old request and a new sign-in; the harness could not express that before |
| `backend/src/SharedWeb/SwaggerSetup.cs` | New shared rule: Development, or `Swagger:Enabled` parsed as true | Correction 3 | One owner for the exposure rule, unit-testable without a host, and no way for the two hosts to drift |
| `backend/src/UserApi/Program.cs`, `backend/src/TaskApi/Program.cs` | Gate `UseSwagger`/`UseSwaggerUI` on `SwaggerSetup.IsEnabled` | Correction 3 | Removes the duplicated environment check that produced the mismatch |
| `backend/tests/unit/BallastLane.Api.UnitTests/SwaggerAvailabilityTests.cs` | Matrix test for the rule | Correction 3 | Encodes the security property: default-off outside Development, opt-in explicit |
| `backend/tests/http/.../SwaggerDocumentTests.cs` (both suites) | Prove the document is served by the real pipeline, including a Production host that opts in | Correction 3 | A unit test on the rule cannot catch a host that forgets to call the middleware |
| `docker-compose.yml` | Explicit `ASPNETCORE_ENVIRONMENT=Production` and `Swagger__Enabled=true` on both hosts | Correction 3 | Makes the effective environment visible in the file that decides it, and keeps the README's `/swagger` claim true |
| `specs/001-task-manager/contracts/user-api.md`, `task-api.md` | OpenAPI section states the opt-in | Correction 3 | Prevents the contract from drifting from the implemented rule |
| `README.md` | Setup, tests, and known limitations rewritten for both shells with the test connection string and the Swagger rule | Corrections 1, 3, 6 | The previous text was wrong for containers, wrong for Windows, and silent about the variable the tests require |
| `specs/001-task-manager/quickstart.md` | .NET 10 versions, `migrator --seed` sequencing, both shells, test connection string | Correction 2 | Brings the run guide in line with what was actually executed |
| `docs/assessment/submission-requirements.md` | Section 4 status replaced | Correction 5 | Requirement sections 2 and 3 were left byte-for-byte unchanged; only the status summary was maintained |

TDD applicability: corrections 3 and 4 changed runtime behavior and have RED and GREEN evidence above.
The README, quickstart, contract, and status edits are documentation-only, so TDD is
`Not Applicable` for them; their validation is the fresh-checkout run, which followed the rewritten
instructions literally and observed the documented outcomes.

## Final references

- Implementation files: `frontend/src/api/client.ts`, `backend/src/SharedWeb/SwaggerSetup.cs`,
  `backend/src/UserApi/Program.cs`, `backend/src/TaskApi/Program.cs`, `docker-compose.yml`
- Test files: `frontend/tests/flows/session.test.tsx`,
  `backend/tests/unit/BallastLane.Api.UnitTests/SwaggerAvailabilityTests.cs`,
  `backend/tests/http/BallastLane.UserApi.HttpTests/SwaggerDocumentTests.cs`,
  `backend/tests/http/BallastLane.TaskApi.HttpTests/SwaggerDocumentTests.cs`
- Documentation: `README.md`, `specs/001-task-manager/quickstart.md`,
  `specs/001-task-manager/contracts/user-api.md`, `specs/001-task-manager/contracts/task-api.md`,
  `docs/assessment/submission-requirements.md`, `GENAI.md`
- Task identifiers: `T028`, `T034`, `T040`, `T097` to `T106`
- Commits: `0d6c2b9` (frontend guard and test), `02747ee` (OpenAPI exposure rule, hosts, compose,
  tests, contracts), `df7c374` (documentation and this record as first written)

## Evidence status rationale

`Reviewed`: the prompt, the pre-correction excerpts, the RED and GREEN observations, the review
findings, and the fresh-checkout results were captured while the work happened, and the developer
reviewed the diff and instructed the commit on 2026-11-17. It is not `Verified` because the record text
itself has not been reviewed and two non-blocking findings remain open: the outdated status paragraph in
`.github/copilot-instructions.md` and the deferred React Router major upgrade.

