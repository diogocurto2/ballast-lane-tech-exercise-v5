# Database Standards

PostgreSQL persistence rules using Npgsql and ADO.NET directly.

## 1. Purpose and scope

This standard defines how the application talks to PostgreSQL: access mechanism, SQL construction, connection and transaction handling, schema constraints, migrations, seeding, and integration testing.

The assessment prohibits Entity Framework, Dapper, and Mediator/MediatR. PostgreSQL is the **proposed** technology, aligned with the job description; it is a recorded decision, not an assessment requirement.

It does not define the schema. Tables, columns, and migration files MUST be established in the Spec Kit planning phase.

## 2. Mandatory rules

### 2.1 Access mechanism

- Data access MUST use **Npgsql with ADO.NET** (`NpgsqlConnection`, `NpgsqlCommand`, `NpgsqlDataReader`).
- Entity Framework, EF Core, Dapper, MediatR, and Mediator MUST NOT be used anywhere in the solution, including as transitive dependencies, for any purpose. The prohibition is solution-wide, not limited to data access.
- Data access MUST live in the infrastructure layer behind an application-owned boundary; see [clean-architecture.md](./clean-architecture.md).

### 2.2 SQL construction

- Every value MUST be passed as a parameter. String interpolation, concatenation, or `string.Format` of values into SQL is prohibited.
- Table and column names MUST come from fixed code, never from user input. Any dynamic identifier MUST be validated against an allow-list.
- SQL SHOULD be readable: one statement per logical operation, explicit column lists. `SELECT *` MUST NOT be used in application queries.
- Queries MUST select only the columns the mapper consumes.

### 2.3 Connections and lifetime

- A connection MUST be opened as late as possible and disposed deterministically with `await using`.
- Connections MUST NOT be held open across application-layer calls or stored in fields for reuse across requests.
- Connection pooling MUST be left to the Npgsql pool; custom pooling MUST NOT be implemented.
- Connection strings MUST come from configuration or environment variables and MUST NOT be committed; see [security.md](./security.md).

### 2.4 Asynchronous operations

- All database I/O MUST be asynchronous (`ExecuteReaderAsync`, `ExecuteNonQueryAsync`, `ExecuteScalarAsync`).
- A `CancellationToken` MUST be passed to every asynchronous Npgsql call.
- Synchronous ADO.NET calls MUST NOT be used in request paths.

### 2.5 Constraints and schema integrity

- Every table MUST have a primary key. The assessment requires a unique identifier and at least two additional fields on the main data table.
- Foreign keys MUST be declared in the database for relationships, with a deliberate `ON DELETE` behavior.
- Uniqueness MUST be enforced by a database constraint, not only by application checks. Duplicate email registration relies on a unique constraint plus correct error translation.
- `NOT NULL` MUST be used for required columns. Nullability in the database MUST match the application model.
- Data types MUST fit the domain: `uuid` for identifiers where chosen, `timestamptz` for instants, `text` with a `CHECK` for bounded text, `numeric` or `integer` for quantities. Do not store dates or statuses as free text without a constraint.

### 2.6 Migrations

- Schema changes MUST be version-controlled as ordered SQL migration files committed to the repository.
- Migrations MUST be applied by a documented, repeatable mechanism; manual edits to a live database are prohibited as the source of truth.
- Migrations MUST be forward-only and MUST NOT be edited after being applied in a shared environment. Correct with a new migration.
- Every migration MUST be idempotent or paired with a documented apply-once runner that records applied versions.
- The schema MUST be reproducible from an empty database by running the documented sequence alone.

### 2.7 Seeding

- Seeded demo data MUST be idempotent: running initialization repeatedly MUST NOT create duplicates or fail.
- Seeding MUST be separate from migrations so that test environments can choose whether to seed.
- Seeded users MUST have hashed passwords created through the same hashing component used at runtime, never pre-computed literals of unknown origin.
- Seed data MUST be clearly labeled as demonstration data, per [delivery-and-ci.md](./delivery-and-ci.md).

### 2.8 Transactions

- Operations requiring atomicity MUST run inside an explicit transaction (`BeginTransactionAsync`), committed on success and rolled back on failure.
- A transaction MUST NOT span an HTTP request boundary implicitly, and MUST NOT be stored in a static or singleton field.
- Isolation level MUST be the PostgreSQL default unless a specific requirement justifies otherwise; the choice MUST be documented.
- Multi-statement operations that mutate related rows MUST be transactional.

### 2.9 Indexing

- Indexes SHOULD be created to support the queries that actually exist, in particular ownership-scoped lookups such as `(owner_id, created_at)`.
- Every foreign key column SHOULD be indexed when it is used for filtering or joins.
- Speculative indexes on unused columns MUST NOT be added.
- An index MUST be justified by a query in the code.

### 2.10 Integration testing

- SQL behavior MUST be tested against a **real PostgreSQL instance**, not an in-memory substitute. See [testing-and-tdd.md](./testing-and-tdd.md).
- Tests MUST cover: insert and read-back mapping, constraint violations (unique, foreign key, not-null), transaction rollback, and migrations applied to an empty database.
- Tests MUST isolate their data and MUST NOT depend on execution order.

## 3. Practical examples

**Parameterized command with cancellation**

```csharp
await using var command = new NpgsqlCommand(
    "INSERT INTO tasks (id, owner_id, title, status, due_date) " +
    "VALUES (@id, @ownerId, @title, @status, @dueDate)", connection, transaction);

command.Parameters.AddWithValue("id", id);
command.Parameters.AddWithValue("ownerId", ownerId);
command.Parameters.AddWithValue("title", title);
command.Parameters.AddWithValue("status", status);
command.Parameters.Add(new NpgsqlParameter("dueDate", NpgsqlDbType.TimestampTz) { Value = (object?)dueDate ?? DBNull.Value });

await command.ExecuteNonQueryAsync(cancellationToken);
```

**Translation of constraint violations**

```csharp
catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
{
    throw new ConflictException("A user with this email already exists.", ex);
}
```

**Migration file naming**

```text
backend/db/migrations/0001_create_users.sql
backend/db/migrations/0002_create_tasks.sql
backend/db/migrations/0003_add_task_due_date_index.sql
```

**Idempotent seed**

```sql
INSERT INTO users (id, email, password_hash)
VALUES ('....', 'demo@example.test', '....')
ON CONFLICT (email) DO NOTHING;
```

## 4. Prohibited practices and anti-patterns

- Entity Framework, EF Core, Dapper, MediatR, or Mediator.
- String-interpolated SQL, including `$"... WHERE id = '{id}'"`.
- `SELECT *` in application queries.
- A single `SqlHelper` god-class owning every query and every business rule.
- Keeping a connection open in a singleton for performance.
- Schema changes applied by hand with no migration file.
- Editing an applied migration.
- Non-idempotent seeds that duplicate rows on restart.
- Testing persistence only with fakes or an in-memory store and calling it verified.
- Catching a `PostgresException` and swallowing it.
- Indexes added without a query that uses them.

## 5. Verification and review criteria

| Check | Method |
| --- | --- |
| No prohibited libraries | Inspect package references in every project for EF, Dapper, MediatR, Mediator. |
| Parameterization | Review every SQL string; confirm no interpolation of values. |
| Async and cancellation | Review for synchronous calls and missing tokens. |
| Disposal | Confirm `await using` on connections, commands, readers, transactions. |
| Constraints | Integration tests for unique, FK, and not-null violations. |
| Migrations | Apply migrations to an empty database and confirm the schema; confirm files are version-controlled. |
| Seed idempotency | Run seeding twice; confirm no duplicates and no failure. |
| Transactions | Test a failing multi-statement operation and confirm rollback. |
| Index justification | Confirm each index maps to a query in the code. |

## 6. References

- Persistence boundary ownership: [clean-architecture.md](./clean-architecture.md)
- SQL injection prevention and secrets: [security.md](./security.md)
- Integration test requirements: [testing-and-tdd.md](./testing-and-tdd.md)
- `CancellationToken` and async conventions: [coding-standards.md](./coding-standards.md)
- Seeded demo data and reproducibility: [delivery-and-ci.md](./delivery-and-ci.md)
- Prohibited technologies source: [technical-exercise-v5.md](../assessment/technical-exercise-v5.md)
