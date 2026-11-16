# Testing and TDD Standard

Test-driven development and test-level expectations for this assessment.

## 1. Purpose and scope

This standard defines how behavior is tested and how TDD evidence is produced. It applies to every implementation task that changes runtime behavior: domain logic, application use cases, data access, API endpoints, and frontend components and flows.

It integrates with the Generative AI evidence workflow in [genai.md](./genai.md): TDD evidence is captured inside the existing evidence record for the implementation activity. Do **not** create separate evidence records per test or per cycle.

## 2. Mandatory rules

### 2.1 The TDD cycle

- **RED** - You MUST write a meaningful, executable test before implementing the behavior, run it, and observe the expected failure.
- **GREEN** - You MUST implement the minimum behavior required to satisfy the test, then rerun it and observe it pass.
- **REFACTOR** - You SHOULD improve the implementation while keeping all tests passing, and rerun the suite after refactoring.
- You MUST record the observed RED, GREEN, and REFACTOR evidence: the command, the relevant output, and the transition. A claimed cycle without observed output is not evidence.

### 2.2 Meaningful failure

- A RED step MUST fail because the behavior under test is missing or wrong, not because of a build error, a missing package, a wrong namespace, or a broken test fixture.
- You MUST distinguish the two in the evidence record. If the first failure is environmental, fix the environment, rerun, and capture the genuine behavior failure.
- A test that passes before the implementation exists indicates the test does not exercise the intended behavior. Correct the test.

### 2.3 Required test levels

- You MUST write **unit tests** for business logic, data access components, and API endpoints, as the assessment requires.
- You MUST write **integration tests against a real PostgreSQL instance** for SQL behavior, persistence, constraints, and migrations. Data access behavior is not considered tested by in-memory substitutes.
- You MUST write **HTTP integration tests** covering authentication, authorization, input validation, not-found and conflict cases, and the CRUD endpoints.
- You MUST write **frontend component and user-flow tests** for the implemented UI behavior.
- Security-relevant behavior MUST have the tests defined in [security.md](./security.md#5-required-security-tests).

### 2.4 Test doubles versus real dependencies

- You MAY use test doubles (fakes, stubs, spies) for business-logic tests where the dependency is an outbound boundary and the test asserts application behavior.
- You MUST use the real dependency when the test asserts SQL, schema constraints, transactions, migrations, or HTTP semantics. Doubling PostgreSQL does not validate SQL.
- Test doubles SHOULD be hand-written or minimal. Avoid mock frameworks that encourage asserting internal call sequences instead of observable behavior.
- A fake MUST NOT silently diverge from the real contract in a way that hides defects. Where a fake replaces a database, the corresponding integration test is still required.

### 2.5 Regression tests

- Every bug fix MUST include a regression test that fails before the fix and passes after it.
- The regression test MUST be recorded with the fix in the evidence record.

### 2.6 Coverage expectations

- Coverage MUST be evaluated by critical behavior, not by an arbitrary global percentage.
- Critical behaviors are: authentication and authorization decisions, ownership enforcement, validation rules, CRUD state transitions, persistence correctness, and error responses.
- Every critical behavior MUST have at least one test that fails if the behavior regresses.
- You MAY use a coverage tool to find untested critical paths. A coverage number is not, by itself, evidence of adequate testing.

### 2.7 Independence and reproducibility

- Tests MUST be independent: any test can run alone, in any order, without relying on state left by another test.
- Tests MUST be reproducible: the same command produces the same result on a clean checkout.
- Tests MUST NOT depend on execution order, wall-clock timing, network access to third-party services, or shared mutable fixtures.
- Each test MUST clean up or isolate the data it creates.

### 2.8 Applicability

- Documentation-only and infrastructure-only tasks MAY mark TDD as `Not Applicable`, with a one-line justification in the evidence record.
- `Not Applicable` for TDD does not exempt the task from validation. Documentation tasks still require link, consistency, and content checks; infrastructure tasks still require build, configuration, or health-check verification.

## 3. Practical examples

**RED, observed as a behavior failure**

```text
dotnet test tests/BallastLane.Application.Tests --filter "FullyQualifiedName~CreateTask"
Failed!  - Failed: 1, Passed: 0
Error Message: Assert.NotNull() Failure: Value is null
```

The failure is an assertion failure, so the test compiled and ran: the behavior is genuinely missing.

**Not a RED step**

```text
dotnet test
error CS0246: The type or namespace name 'CreateTaskHandler' could not be found
```

This is a build failure. Fix the compilation, then capture the real behavior failure.

**Integration test against real PostgreSQL** - the test seeds a row, runs the repository query, asserts the mapped fields, then removes the row in a finally block or transaction rollback.

**HTTP integration test** - use `WebApplicationFactory` to host the API in-process, call the endpoint with an authenticated and an unauthenticated client, and assert status codes and response bodies for both.

## 4. Prohibited practices and anti-patterns

- Writing implementation first and tests afterwards, then presenting it as TDD.
- Tests that assert nothing, or that assert only that no exception was thrown.
- Asserting on internal call sequences of mocks as a substitute for observable behavior.
- Tests coupled to execution order or to a shared database state.
- Disabling, skipping, or deleting a failing test to make the suite pass.
- Replacing a database integration test with an in-memory substitute and calling persistence verified.
- Chasing a coverage percentage by testing trivial accessors.
- Claiming RED without observed failing output.

## 5. Verification and review criteria

| Check | Question |
| --- | --- |
| RED evidence | Is there an observed failure for the behavior before the implementation? |
| Meaningfulness | Does the failure come from behavior rather than build or environment? |
| GREEN evidence | Is there an observed pass after the minimal implementation? |
| Levels | Do unit, PostgreSQL integration, HTTP integration, and frontend tests exist where behavior requires them? |
| Independence | Does each test pass when run alone? |
| Regression | Does every bug fix include a failing-then-passing regression test? |
| Critical coverage | Is every critical behavior covered by at least one meaningful test? |
| Evidence | Is TDD evidence recorded in the activity's existing record rather than a new one? |

## 6. References

- Generative AI evidence standard: [genai.md](./genai.md)
- Security test requirements: [security.md](./security.md)
- Database integration testing: [database-standards.md](./database-standards.md)
- API contract testing: [api-standards.md](./api-standards.md)
- Frontend testing: [frontend-standards.md](./frontend-standards.md)
- Delivery pipeline test execution: [delivery-and-ci.md](./delivery-and-ci.md)
- Assessment testing requirement: [technical-exercise-v5.md](../assessment/technical-exercise-v5.md)
