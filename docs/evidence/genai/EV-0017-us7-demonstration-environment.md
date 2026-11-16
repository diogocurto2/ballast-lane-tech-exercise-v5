# EV-0017 - User Story 7: seeded demonstration environment (001-task-manager)

> **Revision note (2026-11-16, after `EV-0024`).** Paths in this record reflect the repository layout
> at the time of the activity, when the backend lived at the root as `src/`, `tests/`, and `db/`. After
> the `backend/` restructure recorded in [EV-0024](./EV-0024-backend-folder-restructure.md), those
> locations are `backend/src/`, `backend/tests/`, and `backend/db/`. Nothing else in this record was
> altered, so it remains an accurate account of what was observed when it was written.

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0017` |
| Date | `2026-11-16` |
| Development phase | Phase 6 - Implement (User Story 7, T091 to T096) |
| Activity type | `Code generation` and `Testing` |
| Related specification | `specs/001-task-manager/spec.md` (US7, FR-012, NFR-004, NFR-005, SC-006, SC-007) |
| Related task identifiers | `T091` to `T096` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `PENDING` |

## Actual prompt

Continuation of the developer instruction that opened the session:

```text
/speckit-implement
```

`tasks.md` T091 to T096 and `quickstart.md` sections 3 and 8 defined the behavior.

## Relevant context

- Requirements: US7 acceptance scenarios 1 to 4, FR-012, NFR-004, NFR-005, SC-006, SC-007.
- Standards: `database-standards.md` 2.7, `delivery-and-ci.md` 2.4, `security.md` 2.6.
- Preceding record: `EV-0016`.

## Original AI-generated output

The demonstration dataset and idempotent seeder were produced during the foundational phase
(`EV-0010`, T020/T092), so the three US7 tests passed at their first run. That is not a RED
checkpoint. Two defects were introduced temporarily to observe the failures the tests exist to catch:

```csharp
// Defect A: only the last documented account is seeded.
foreach (var account in DemonstrationData.Accounts.TakeLast(1))
```

```sql
-- Defect B: the duplicate guard is removed from the task insert.
INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at)
SELECT @id, @ownerId, @title, @description, @status, @dueDate, @createdAt, @createdAt
```

## Validation performed

| Area | Which was checked | Result |
| --- | --- | --- |
| Clean Architecture | The seeder is an Infrastructure adapter consuming the application-owned hasher and time provider boundaries | `[VERIFIED]` |
| SOLID | The dataset is data (`DemonstrationData`) and the behavior is the seeder, so a dataset change cannot alter the idempotency logic | `[VERIFIED]` |
| TDD | RED observed with the two temporary defects: 4 of 9 migration-suite tests failed, including both demonstration tests | `[VERIFIED]` |
| Security | Seeded passwords are hashed with `PasswordHasher<T>`, never stored as plaintext, and each stored hash verifies against its documented password | `[VERIFIED]` |
| Functional requirements | FR-012, NFR-004, NFR-005, SC-006, SC-007 verified by automated tests plus the executed quickstart sections | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` The seeder writes in one explicit transaction and guards task inserts with
  `WHERE NOT EXISTS`, so a second run neither duplicates nor fails.
- `[VERIFIED]` The user insert targets `ON CONFLICT (lower(email))`, matching the unique expression
  index, so repeated seeding cannot raise a constraint error.
- `[VERIFIED]` A connection opened after the seeder completes can read the rows, proving the data was
  committed rather than held in an open transaction.
- `[VERIFIED]` The first containerized seed attempt failed with
  `Migration run failed: Failed to connect to 127.0.0.1:5432`. The local `.env` had
  `ConnectionStrings__Database` pointing at `localhost`, which inside the container is the container
  itself. The documented `.env.example` value `Host=postgres` is correct; the local file was corrected.
- `[VERIFIED]` The migrator image logged `libgssapi_krb5.so.2: cannot open shared object file`. Npgsql
  optionally uses GSSAPI, so `libgssapi-krb5-2` is now installed in the runtime stage of all three
  images. The rebuild produced clean output.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Install `libgssapi-krb5-2` in the runtime images | Removes a recurring startup warning that looks like a defect | `[AI-PROPOSED]` |
| Two demonstration accounts with five tasks | Enough to demonstrate ownership isolation as well as the seeded journey | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| Local `.env` uses `Host=postgres` for the runtime connection string | `localhost` inside a container is the container | Compose service-name resolution is the documented path |
| Added `libgssapi-krb5-2` to the runtime images | Missing optional GSSAPI library produced an error line | The library is a runtime prerequisite of Npgsql's optional GSSAPI support |

## Commands and observed results

### RED with the two temporary defects

```text
dotnet test tests\integration\BallastLane.Migrations.Tests
Failed!  - Failed: 4, Passed: 5, Skipped: 0, Total: 9
  DemoSeedTests.Every_seeded_password_verifies_against_its_stored_hash [FAIL]
  DemoSeedTests.Seeding_twice_neither_duplicates_nor_fails [FAIL]
  DemonstrationDatasetTests.The_documented_credentials_authenticate_and_list_the_seeded_tasks [FAIL]
  DemonstrationDatasetTests.A_second_seed_run_leaves_user_and_task_counts_unchanged [FAIL]
```

### GREEN after restoring the seeder

```text
dotnet test tests\integration\BallastLane.Migrations.Tests
Passed!  - Failed: 0, Passed: 9, Skipped: 0, Total: 9
```

### Quickstart section 3 - documented seed on a clean database

```text
docker exec ballastlane-postgres psql -U postgres -c "DROP DATABASE IF EXISTS taskmanager;"   -> DROP DATABASE
docker exec ballastlane-postgres psql -U postgres -c "CREATE DATABASE taskmanager;"            -> CREATE DATABASE
docker compose build migrator                                                                 -> Built
docker compose run --rm migrator --seed
    Migrations applied from '/app/db/migrations'.
    Demonstration data seeded.
psql: SELECT email FROM users  -> demo.user@example.test, reviewer@example.test
psql: SELECT count(*) FROM tasks -> 5
```

First attempt, before the `.env` correction:

```text
docker compose run --rm migrator --seed
    Migration run failed: Failed to connect to 127.0.0.1:5432
```

### Quickstart section 3 - idempotency

```text
docker compose run --rm migrator --seed      (second run)
    Migrations applied from '/app/db/migrations'.
    Demonstration data seeded.
psql: users = 2, tasks = 5      (unchanged)
```

### Quickstart section 8 - restart durability

```text
docker compose down             -> container and network removed
docker compose up -d postgres   -> started
docker inspect health           -> healthy
psql: users = 2, tasks = 5      (data intact after restart)
```

### Scenario automation split (T096)

| Scenario | Automated | Executed manually |
| --- | --- | --- |
| Documented credentials authenticate and see seeded tasks | Yes - `DemonstrationDatasetTests` | Yes - via `migrator --seed` and database inspection |
| Second seed run does not duplicate | Yes - two tests | Yes - second `docker compose run` |
| Seeded rows readable from a new connection | Yes | `Not Applicable` - covered by the automated check |
| Data intact after a restart | No - requires container restart | Yes - section 8 |

## Final references

- Implementation files: `src/Infrastructure/Persistence/{DemonstrationData,DemoSeeder}.cs`,
  `src/Migrator/Program.cs`, `src/Migrator/Dockerfile`, `docker-compose.yml`, `.env.example`,
  `db/docker-entrypoint-initdb.d/01-create-test-database.sql`
- Test files: `tests/integration/BallastLane.Migrations.Tests/{DemonstrationDatasetTests,DemoSeedTests}.cs`
- Task identifiers: `T091` to `T096`
- Commits: `PENDING`

## Evidence status rationale

Status is `Captured`. The manual quickstart results are recorded as executed, and the container
restart scenario is explicitly marked manual. Developer review and a commit reference remain
outstanding.
