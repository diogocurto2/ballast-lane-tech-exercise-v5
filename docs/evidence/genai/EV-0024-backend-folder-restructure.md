# EV-0024 - Backend folder restructure and confirmation of the Docker build context

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0024` |
| Date | `2026-11-16` |
| Development phase | Post-implementation follow-up (after Phase 10 verification) |
| Activity type | `Code generation` (repository restructure and configuration) |
| Related specification | `specs/001-task-manager/spec.md` - no functional requirement changes; the work is structural |
| Related task identifiers | Not Applicable - raised by developer review after T001 to T107 were complete, so no task in `specs/001-task-manager/tasks.md` covers it |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `PENDING` - nothing has been committed for this change |

## Actual prompt

> nos tesmos uma pasta frontend, mas nao temos uma backend, certo? acho q isso precisa ser arrumado

The developer confirmed the scope in three follow-up answers:

- **Layout:** `backend/{src,tests,db}`, with `BallastLane.sln` remaining at the repository root.
- **Historical artifacts:** update living documents only; leave historical artifacts intact and add an
  explanatory note instead of rewriting them.
- **Verification:** yes, re-run the build and the full test suite after the move.

## Relevant context

- Requirement or assessment section: the assessment requires a complete, coherent repository that
  demonstrates engineering judgment; the structure was not itself a scored requirement, so this is a
  quality correction rather than a specification change.
- Engineering standard applied: [clean-architecture.md](../../engineering/clean-architecture.md)
  (project structure must express the architecture), [delivery-and-ci.md](../../engineering/delivery-and-ci.md)
  (builds must stay reproducible from documented commands), [genai.md](../../engineering/genai.md).
- Files or artifacts supplied as context: `README.md`, `specs/001-task-manager/plan.md`,
  `specs/001-task-manager/research.md`, `specs/001-task-manager/tasks.md`, `BallastLane.sln`,
  `docker-compose.yml`, the three `Dockerfile`s, `Directory.Build.props`, `.dockerignore`, and the
  test helpers that resolve `db/migrations` from disk.

## Original AI-generated output

Before the change, the backend was split across the repository root while only the client was grouped:

```text
C:\...\ballast-lane-tech-exercise-v5
├── src/        (UserApi, TaskApi, Migrator, Domain, Application, Infrastructure, SharedWeb)
├── tests/      (unit, integration, http)
├── db/         (migrations, docker-entrypoint-initdb.d)
├── frontend/   (React client)
├── docs/
├── specs/
├── BallastLane.sln
└── docker-compose.yml
```

The AI's first restructuring attempt failed because the directories had never been committed. The
error reported that the source was empty / a bad source rather than a rename conflict, which is why a
plain filesystem move was used instead:

```text
$ git mv src backend/src
<error: the source directory is empty / bad source, because src was untracked>
```

The AI's first rewrite of `BallastLane.sln` used a PowerShell single-quoted replacement string:

```powershell
$c = $c -replace '"src\\', '"backend\\src\'
```

That produced doubled separators, e.g. `backend\\src\BallastLane.Domain\BallastLane.Domain.csproj`.

## Validation performed

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | The move is purely positional. No project reference, namespace, `using` directive, or assembly name changed, so `Domain` still has no outgoing reference and `Infrastructure` is still referenced only by the three composition roots. The architecture test that reads `.csproj` files confirms it. | `[VERIFIED]` |
| SOLID | Not Applicable - no type or responsibility changed. | `[VERIFIED]` - no code semantics changed |
| TDD | Not Applicable - no behavior changed, so no RED/GREEN cycle applies. Existing tests are the regression net and all passed after the move. | `[VERIFIED]` by the full suite |
| Security | Not Applicable - no authentication, authorization, validation, or secret-handling code changed. `.env` stays ignored; nothing secret is written into configuration. | `[VERIFIED]` |
| Functional requirements | No functional requirement changed. The three containers still start, the migrator still applies migrations and seeds, and all backend and frontend suites pass. | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` **The asymmetry was real and worth fixing.** `frontend/` had a home while the backend
  had none, so every documented command had to name a root-level `src/` path that looked like it
  belonged to a different convention than the client.
- `[VERIFIED]` **`git mv` cannot move an untracked directory.** The implementation had never been
  committed, so both `src` and `tests` were untracked and `git mv` refused them. `Move-Item` performed
  the move; because nothing was committed, no rename history was lost.
- `[VERIFIED]` **A single-quoted PowerShell replacement string does not process `\` escapes.** The
  first `.sln` rewrite inserted literal doubled separators. The `.Replace()` method with literal
  strings corrected it, and the following build proved the whole solution loaded.
- `[VERIFIED]` **The blast radius was larger than the folder move.** Sixteen solution project entries,
  nine test project references, three Dockerfiles, the compose file, four test helpers that locate
  `db/migrations` on disk, and seven architecture-test path constants all encoded the old layout.
  Reading `db/migrations` from a relative path is what makes this class of change fail late, at test
  time rather than build time.
- `[VERIFIED]` **Historical artifacts must not be rewritten to match a later decision.** `tasks.md`
  and the pre-move evidence records describe what was executed and observed. Editing their paths would
  have made them look as though the tasks always ran against `backend/`, which would be false. Each
  carries a dated revision note pointing here instead.
- `[AI-PROPOSED]` Keeping `BallastLane.sln`, `docker-compose.yml`, `Directory.Build.props`,
  `.gitignore`, and `.dockerignore` at the root, because they describe the whole repository rather than
  one half of it.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Move the backend to `backend/{src,tests,db}` and keep the solution file at the root | Makes the repository symmetric while preserving the documented root-level `dotnet build` and `dotnet test` commands | `[DEVELOPER DECISION]` |
| Update living documents only and annotate historical artifacts | Keeps the executed record truthful instead of retro-fitting it | `[DEVELOPER DECISION]` |
| Re-run the build and the full test suite after the move | A structural change must be proven, not assumed | `[DEVELOPER DECISION]` |
| Final approval of this record | Developer review of the evidence is still outstanding | `Pending` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| `Move-Item` instead of `git mv` | `git mv` failed on untracked directories (`fatal: bad source`) | Nothing was committed, so the move is a plain filesystem relocation with no history to preserve |
| `.Replace('backend\\src\', 'backend\src\')` instead of `-replace '"src\\', '"backend\\src\'` | The `-replace` form produced doubled separators in all 16 `.sln` entries | A single-quoted PowerShell replacement string is literal, so `\\` inserts two backslashes; `String.Replace` with an exact literal is unambiguous |
| Test `.csproj` reference paths changed from `..\..\..\src\` to `..\..\..\..\backend\src\` | The projects moved one level deeper | Relative `ProjectReference` paths are resolved from the project directory, so the extra level must be explicit |
| Migration-directory lookups changed to `Path.Combine(Root, "backend", "db", "migrations")` | Four test helpers resolved `db/migrations` relative to the repository root | The path is read at test-run time; a stale value fails only when the fixture runs |
| Architecture-test constants changed to `backend/src/...` | The test reads `.csproj` declarations from disk | Path constants are the test's subject, so they must track the real layout or the test would silently read nothing |
| Dockerfiles now `COPY backend/src/ ./backend/src/` and publish `backend/src/...`; the migrator copies `backend/db/migrations` | The build context is the repository root, so the copy sources moved | Keeping `context: .` was preferred to adding a second build context, and `.dockerignore` already keeps the context small |
| `README.md`, `plan.md`, `research.md`, and the two engineering standards updated; `tasks.md` and the pre-move evidence records annotated instead of rewritten | Living documents must stay accurate; historical artifacts must stay truthful | See the reasoning above for historical artifacts |

## Commands and observed results

Move and defect correction:

```text
Move-Item src backend\src ; Move-Item tests backend\tests ; Move-Item db backend\db
```

```text
=== root ===
.github
.gitignore
.specify
backend
docs
frontend
specs
=== backend ===
db
src
tests
```

```text
$ git mv src backend/src
<refused: the untracked source was treated as empty / bad source>
```

```text
$ Select-String -Path BallastLane.sln -Pattern 'backend'
"backend\\src\BallastLane.Domain\BallastLane.Domain.csproj"   # defective, corrected with .Replace()
```

Backend build and full suite, from the repository root:

```text
dotnet build BallastLane.sln -v q --nologo
```

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

```text
dotnet test BallastLane.sln --no-build -m:1 -v q      (ConnectionStrings__TestDatabase set; postgres healthy)
```

```text
Passed!  - Failed:     0, Passed:    53, Skipped:     0, Total:    53 - BallastLane.TaskApi.HttpTests.dll (net10.0)
Passed!  - Failed:     0, Passed:    22, Skipped:     0, Total:    22 - BallastLane.UserApi.HttpTests.dll (net10.0)
Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9 - BallastLane.Migrations.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:     7, Skipped:     0, Total:     7 - BallastLane.PostgreSql.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18 - BallastLane.Api.UnitTests.dll (net10.0)
Passed!  - Failed:     0, Passed:    44, Skipped:     0, Total:    44 - BallastLane.Application.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:     9, Skipped:     0, Total:     9 - BallastLane.Architecture.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:    31, Skipped:     0, Total:    31 - BallastLane.Domain.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18 - BallastLane.Infrastructure.UnitTests.dll (net10.0)

total passed: 211 ; total failed: 0
```

Container build and the migrator, which prove the Dockerfile and compose paths:

```text
docker compose build migrator
docker compose run --rm migrator --seed
```

```text
#5 transferring context: 900B 0.0s done
#6 transferring context: 6.30kB 0.0s done
 ballastlane-task-manager-migrator  Built
Migrations applied from '/app/db/migrations'.
Demonstration data seeded.
```

An earlier run of the same build, before the cache was warm, reported
`#6 transferring context: 108.19kB`. Both observations are recorded; they supersede the
`[PENDING]` computed estimate in `EV-0018` and `GENAI.md`, which had put the expected context near
0.5 MB.

Frontend, unaffected by the move but re-run to confirm the repository as a whole:

```text
cd frontend && npm run lint && npx tsc --noEmit && npm run build && npx vitest run
```

```text
> eslint .
tsc exit code: 0
dist/assets/index-CNh0cJOt.js   181.75 kB │ gzip: 57.90 kB
✓ built in 1.14s
 Test Files  8 passed (8)
      Tests  27 passed (27)
```

The exact command documented in `quickstart.md` section 7 was also run verbatim from the repository
root, because the solution file is the only build entry point there:

```text
dotnet test -m:1
```

```text
assemblies reporting: 9
total passed with bare 'dotnet test -m:1': 211
```

Ignore rules still hold after the move:

```text
git add -An . | Select-String -Pattern '/(bin|obj|node_modules|dist)/|\.trx$|coverage'
```

```text
none
```

202 paths would be added; all are source, configuration, or documentation.

## Final references

- Implementation files: `BallastLane.sln`, `docker-compose.yml`,
  `backend/src/Migrator/Dockerfile`, `backend/src/UserApi/Dockerfile`,
  `backend/src/TaskApi/Dockerfile`, `backend/src/**/*.csproj`, `backend/tests/**/*.csproj`,
  `backend/tests/integration/BallastLane.Migrations.Tests/TestInfrastructure.cs`,
  `backend/tests/integration/BallastLane.PostgreSql.Tests/PostgreSqlDatabase.cs`,
  `backend/tests/http/BallastLane.UserApi.HttpTests/TestDatabase.cs`,
  `backend/tests/http/BallastLane.TaskApi.HttpTests/TestDatabase.cs`,
  `backend/tests/unit/BallastLane.Architecture.Tests/DependencyDirectionTests.cs`
- Documentation files: `README.md`, `specs/001-task-manager/plan.md`,
  `specs/001-task-manager/research.md`, `specs/001-task-manager/tasks.md`,
  `docs/engineering/database-standards.md`, `docs/engineering/testing-and-tdd.md`, `.dockerignore`,
  `GENAI.md`, `docs/evidence/genai/README.md`
- Test files: the nine backend suites listed above and `frontend/tests` were re-run; no test file
  changed behavior, and the only test edits were the path constants and migration-directory lookups.
- Task identifiers: Not Applicable - post-implementation follow-up outside the task list.
- Commits: `PENDING`

## Evidence status rationale

Status is `Captured`: the prompt, the developer's scope decisions, the defective intermediate output,
the corrections, and the observed validation results are all recorded. The record stays short of
`Reviewed` because developer review of this evidence is outstanding, and it has no commit reference
because nothing has been committed.
