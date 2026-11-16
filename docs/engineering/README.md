# Engineering Standards Index

Index of the engineering standards that govern this assessment, and where each applies.

## 1. Purpose and scope

This index is the entry point for engineering documentation. It maps each standard to its purpose, to the Spec Kit phases where it applies, and to a proposed constitutional principle.

All standards use **MUST** for mandatory rules, **SHOULD** for recommendations, and **MAY** for optional practices. Documents are concise by intent: this is a small Senior .NET assessment, not an enterprise platform.

## 2. Standards

| Standard | Purpose |
| --- | --- |
| [genai.md](./genai.md) | Mandatory AI-assisted development workflow: evidence capture, provenance labels, lifecycle, and completion gate. |
| [testing-and-tdd.md](./testing-and-tdd.md) | TDD cycle with observed RED/GREEN/REFACTOR evidence, required test levels, doubles versus real dependencies, and coverage by critical behavior. |
| [solid-principles.md](./solid-principles.md) | The five SOLID principles applied to controllers, use cases, entities, persistence adapters, and tests; plus the limit between design and unnecessary abstraction. |
| [clean-architecture.md](./clean-architecture.md) | Layer responsibilities, allowed dependency directions, boundary ownership, DTO mapping, composition roots, and multi-host sharing. |
| [coding-standards.md](./coding-standards.md) | C# and TypeScript conventions, async and cancellation, error propagation, DI, logging, warnings, comments, and commit hygiene. |
| [api-standards.md](./api-standards.md) | REST resource modeling, verbs and status codes, DTOs, validation separation, ProblemDetails, OpenAPI, auth boundaries, and thin controllers. |
| [security.md](./security.md) | Password hashing, token validation, explicit public/protected endpoints, resource-level authorization, injection prevention, secrets, demo credentials, and required security tests. |
| [database-standards.md](./database-standards.md) | Npgsql and ADO.NET access, parameterized SQL, connection and transaction handling, constraints, migrations, idempotent seeds, indexing, and real PostgreSQL tests. |
| [frontend-standards.md](./frontend-standards.md) | Component responsibilities, UI/state/API separation, authentication state, responsiveness, accessibility, forms, UI states, CRUD flows, and frontend testing. |
| [delivery-and-ci.md](./delivery-and-ci.md) | Docker Compose reproducibility, configuration and example env files, CI workflows, seeded demo data, README and GENAI.md deliverables, repository hygiene, and the submission checklist. |

## 3. Assessment documents

| Document | Purpose |
| --- | --- |
| [technical-exercise-v5.md](../assessment/technical-exercise-v5.md) | Original assessment. Authoritative and unmodified. |
| [submission-requirements.md](../assessment/submission-requirements.md) | Additional delivery requirements and the submission deadline, recorded without private correspondence. |
| [GENAI.md](../../GENAI.md) | Living assessment response for the Generative AI section. |
| [docs/evidence/genai/](../evidence/genai/README.md) | Evidence records backing `GENAI.md`. |

## 4. Applicability by Spec Kit phase

`P` primary - the standard drives the phase. `S` supporting - consulted and applied. `-` not applicable.

| Standard | 1 Constitution | 2 Specify | 3 Plan | 4 Tasks | 5 Analyze | 6 Implement | 7 Review | 8 Conclude |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| genai.md | P | P | P | P | P | P | P | P |
| testing-and-tdd.md | S | S | P | P | S | P | P | S |
| solid-principles.md | S | S | P | S | S | P | P | - |
| clean-architecture.md | S | S | P | S | P | P | P | S |
| coding-standards.md | - | - | S | S | S | P | P | - |
| api-standards.md | - | P | P | S | S | P | P | S |
| security.md | S | P | P | P | P | P | P | S |
| database-standards.md | - | P | P | S | S | P | P | S |
| frontend-standards.md | - | P | P | S | S | P | P | S |
| delivery-and-ci.md | - | S | S | S | S | S | P | P |

Notes:

- Phase 1 establishes these standards and defers the constitution; `genai.md` is primary because it governs evidence for every phase.
- Phase 5 (analyze) uses `clean-architecture.md` and `security.md` as concrete checks against the specification, plan, and tasks.
- Phase 7 (review) uses `testing-and-tdd.md`, `coding-standards.md`, and `security.md` as the review rubric.
- Phase 8 (conclude) uses `delivery-and-ci.md` for README, GENAI.md, and submission readiness.

## 5. Cross-references

```text
genai.md ────────────── governs evidence for every activity
   │
   ├── testing-and-tdd.md ──┬── security.md (required security tests)
   │                        ├── database-standards.md (real PostgreSQL tests)
   │                        └── frontend-standards.md (component/flow tests)
   │
clean-architecture.md ─┬── solid-principles.md (abstraction boundaries)
                       ├── api-standards.md (presentation layer rules)
                       ├── database-standards.md (persistence adapter rules)
                       └── coding-standards.md (layer conventions)

api-standards.md ──┬── security.md (authn/authz and ownership)
                   └── clean-architecture.md (thin controllers, DTOs)

security.md ──┬── database-standards.md (parameterized SQL, credentials)
              ├── frontend-standards.md (session handling)
              └── delivery-and-ci.md (secrets, demo credentials)

delivery-and-ci.md ── testing-and-tdd.md, frontend-standards.md, submission-requirements.md
```

## 6. Proposed mapping to constitutional principles

The constitution will be created separately with `/speckit-constitution` after these standards are reviewed. This is a **proposal**, not a ratified mapping.

| # | Proposed principle | Source standards |
| --- | --- | --- |
| I | Evidence-Based AI-Assisted Engineering (non-negotiable) | [genai.md](./genai.md), [GENAI.md](../../GENAI.md) |
| II | Test-First Development (non-negotiable) | [testing-and-tdd.md](./testing-and-tdd.md) |
| III | Clean Architecture and Dependency Discipline | [clean-architecture.md](./clean-architecture.md), [solid-principles.md](./solid-principles.md) |
| IV | Security and Data Protection by Default | [security.md](./security.md), [database-standards.md](./database-standards.md) |
| V | Contract-First, Testable APIs | [api-standards.md](./api-standards.md) |
| VI | Readable, Consistent Code | [coding-standards.md](./coding-standards.md), [frontend-standards.md](./frontend-standards.md) |
| VII | Reproducibility and Delivery Readiness | [delivery-and-ci.md](./delivery-and-ci.md), [submission-requirements.md](../assessment/submission-requirements.md) |
| VIII | Proportional Simplicity (no unjustified complexity) | [solid-principles.md](./solid-principles.md) section 6, [delivery-and-ci.md](./delivery-and-ci.md) section 3, [clean-architecture.md](./clean-architecture.md) section 13 |

Principle VIII exists because several standards deliberately forbid abstraction and infrastructure that a small assessment does not need. Making this explicit prevents "best practice" drift toward unnecessary enterprise complexity.

## 7. Established requirements versus proposed choices

| Category | Status | Examples |
| --- | --- | --- |
| Assessment requirements | **Established** - authoritative, unmodified | Clean Architecture, TDD, no EF/Dapper/Mediator, CRUD API plus user API, unit tests, frontend, README with seeded data |
| Submission requirements | **Established** - recorded from recruiter | One public repository, single link, presentation, root `GENAI.md`, no symbolic links, deadline, post-submission freeze |
| Technology choices | **Proposed** - decided here, revisable in the plan | PostgreSQL with Npgsql, React with TypeScript, Docker Compose, GitHub Actions |
| Architecture specifics | **Deferred** - planning phase | Project structure, project names, class design, schema, API paths and contracts, .NET version |

The .NET version is deliberately undecided. The job description references .NET 6, 7, and 8, and the assessment mandates no specific version; the choice MUST be recorded as a planning decision.

## 8. Verification of these documents

Run these checks after any change to the standards:

1. Internal links resolve.
2. Every standard contains the sections: purpose and scope, mandatory rules, examples, prohibited practices, verification criteria, references.
3. No standard contradicts another on dependency direction, testing level, or prohibited technology.
4. No standard requires infrastructure without a functional need.
5. No secrets, personal data, or absolute machine paths are present.
6. Provenance is honest: proposed choices are labeled proposed, deferred decisions are listed, and no implementation is described as done.

## 9. Status

| Item | Status |
| --- | --- |
| Engineering standards | Established in this task; pending developer review |
| Constitution | `PENDING` - unratified template at `.specify/memory/constitution.md` |
| Feature specification, plan, tasks | `PENDING` - not created |
| Application code, tests, schema | `PENDING` - not created |
| REST API GenAI exercise | `PENDING` - required by the assessment and not yet performed |
