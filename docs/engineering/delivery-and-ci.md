# Delivery and CI Standard

Reproducible local execution, continuous integration, and submission readiness.

## 1. Purpose and scope

This standard defines how the solution is built, run, tested, packaged, and delivered. It distinguishes **mandatory assessment requirements** from **project quality improvements** so neither is confused with the other.

It applies to containerization, configuration, automation, documentation deliverables, repository hygiene, and the submission checklist.

## 2. Mandatory rules

### 2.1 Reproducible local execution

- The full stack MUST run locally from a documented sequence of commands on a clean checkout.
- Docker and Docker Compose MUST be used to provide PostgreSQL and any runtime service in a reproducible way.
- Compose MUST define a health check for each service that others depend on, and dependent services MUST wait for it.
- Local execution MUST NOT require manual database creation beyond running the documented migrations and seed.
- Ports and service names MUST be documented and MUST NOT conflict with common defaults in unexpected ways.

### 2.2 Configuration

- Configuration MUST be versioned for non-secret values, and supplied by environment variables for secrets.
- An `.env.example` MUST be committed with placeholder values and a comment for each variable.
- A `.env` file MUST be git-ignored, along with local overrides such as `appsettings.Local.json`.
- The README MUST list every required variable and its purpose.
- Defaults MUST be safe: no real credential may appear as a default value.

### 2.3 Continuous integration

- GitHub Actions MUST build and test the backend and MUST fail the workflow on any failure.
- GitHub Actions MUST build and lint the frontend and MUST fail on error.
- Integration tests requiring PostgreSQL MUST run in CI against a PostgreSQL service, not be skipped.
- The workflow MUST be triggered on push and pull request to the main branch.
- Workflow logs MUST NOT print secrets. Secrets MUST come from repository or environment secrets.
- A red build MUST be fixed before proceeding; it MUST NOT be bypassed by disabling tests.

### 2.4 Seeded demonstration data

- The application MUST provide seeded demo users and records sufficient to demonstrate the user story without manual setup.
- Seed credentials MUST be documented in the README and labeled demonstration-only.
- Seeding MUST be idempotent and MUST be safe to run repeatedly; see [database-standards.md](./database-standards.md).

### 2.5 README content

`README.md` is a submission deliverable. It MUST contain:

- Project description and the user story driving the implementation.
- Architecture overview with the layer responsibilities and dependency direction.
- Prerequisites and exact setup steps.
- How to run the backend, the frontend, and the database.
- How to run the tests, including integration tests.
- Seeded demo credentials, clearly labeled.
- The API surface summary and where to find OpenAPI documentation.
- Engineering rationale: the significant decisions and trade-offs.
- A pointer to [GENAI.md](../../GENAI.md) and the evidence records.
- Known limitations stated honestly.

The README MUST NOT describe functionality that is not implemented.

### 2.6 GENAI.md and evidence

- `GENAI.md` MUST exist at the repository root and answer every Generative AI question in the assessment, per [submission-requirements.md](../assessment/submission-requirements.md).
- `GENAI.md` MUST be backed by evidence records under [docs/evidence/genai/](../evidence/genai/README.md), following [genai.md](./genai.md).
- Unverified sections MUST remain explicitly pending rather than described as complete.

### 2.7 Public repository hygiene

- The repository MUST be public, with a single canonical URL used for submission.
- No secrets, credentials, tokens, private keys, or personal data MUST be committed, including in history.
- No machine-specific absolute paths MUST appear in committed documentation.
- No symbolic links MUST be tracked; the submission prohibits them.
- `.gitignore` MUST cover build output, dependencies, local settings, test results, coverage, environment files, and OS artifacts.

### 2.8 Submission checklist

Before sending the submission link:

1. Clean checkout builds and tests pass.
2. Docker Compose brings the stack up from scratch, with migrations and seed applied.
3. The user story is demonstrable end to end through the UI.
4. Both APIs are reachable and documented; protected endpoints reject unauthenticated access.
5. README and GENAI.md are accurate and free of unimplemented claims.
6. No secrets, personal data, absolute paths, or symbolic links are present.
7. Backend build has no warnings; frontend lint and type check pass.
8. The repository is public and reachable while signed out.

### 2.9 Freeze after submission

- After the submission link is sent, the repository MUST NOT be modified until the interviewer authorizes changes or the interview completes.
- Preparation notes outside the repository MAY continue.

## 3. Mandatory versus quality improvements

| Item | Classification | Basis |
| --- | --- | --- |
| Full stack CRUD demonstration, two APIs, seeded data | Mandatory | Assessment requirements |
| README with setup and rationale; root GENAI.md | Mandatory | Assessment and submission requirements |
| Public single repository, no symbolic links | Mandatory | Submission requirements |
| Unit, integration, and frontend tests | Mandatory | Assessment testing requirement |
| Docker Compose for reproducible local run | Mandatory | Delivery requirement for reproducibility |
| CI build and test automation | Quality, strongly recommended | Demonstrates engineering practice; not explicitly required |
| OpenAPI documentation | Quality, required by [api-standards.md](./api-standards.md) | Improves contract clarity |
| Architecture enforcement test | Quality | Makes dependency rules durable |
| Structured logging with correlation ids | Quality, MAY | Useful but not required for the assessment |

- AWS, Redis, Kubernetes, service meshes, and message brokers MUST NOT be introduced. They have no functional need in this assessment, and adding them increases cost and risk without demonstrating the required skills.
- A technology MAY be added only when a named requirement cannot be met without it, and the justification MUST be recorded in the plan.

## 4. Practical examples

**Compose with health dependency**

```yaml
services:
  db:
    image: postgres:16
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      retries: 10
  api:
    build: ./src/Api
    depends_on:
      db:
        condition: service_healthy
```

**CI job outline**

```yaml
jobs:
  backend:
    steps:
      - run: dotnet build --configuration Release
      - run: dotnet test --configuration Release
  frontend:
    steps:
      - run: npm ci
      - run: npm run lint
      - run: npm run build
      - run: npm test
```

## 5. Prohibited practices and anti-patterns

- Documented setup steps that do not work on a clean machine.
- Committing `.env`, connection strings, or signing keys.
- CI that skips integration tests to stay green.
- A README describing features that do not exist.
- Demo credentials that are valid on any real system.
- Symbolic links in tracked files.
- Adding infrastructure that no requirement justifies.
- Tagging a release or editing the repository after submission.
- Treating a quality improvement as a release blocker at the cost of the deadline.

## 6. Verification and review criteria

| Check | Method |
| --- | --- |
| Clean-clone run | Clone to a fresh directory, follow the README, confirm the stack starts. |
| Compose health | Confirm dependent services wait for the database health check. |
| CI status | Confirm the workflow passes on the main branch and on a pull request. |
| Secrets scan | Search tracked files and history for credentials and tokens. |
| Symlink scan | Confirm no tracked symbolic links. |
| README accuracy | Compare every claimed feature against implemented behavior. |
| GENAI.md | Confirm all assessment questions are answered or explicitly pending. |
| Checklist | Execute section 2.8 in order before submitting. |

## 7. References

- Submission requirements and deadline: [submission-requirements.md](../assessment/submission-requirements.md)
- Test commands and levels: [testing-and-tdd.md](./testing-and-tdd.md)
- Configuration and secret handling: [security.md](./security.md)
- Migration and seed rules: [database-standards.md](./database-standards.md)
- Frontend build and lint: [frontend-standards.md](./frontend-standards.md)
- Evidence workflow: [genai.md](./genai.md)
- Original assessment: [technical-exercise-v5.md](../assessment/technical-exercise-v5.md)
