# Engineering Standards Index

Index of the engineering standards that govern this assessment, and where each applies.

## 1. Purpose and scope

This index is the entry point for engineering documentation. It maps each standard to its purpose, to the Spec Kit phases where it applies, and to a proposed constitutional principle.

All standards use **MUST** for mandatory rules, **SHOULD** for recommendations, and **MAY** for optional practices. Documents are concise by intent: this is a small Senior .NET assessment, not an enterprise platform.

## 2. Standards

| Standard | Purpose |
| --- | --- |
| [genai.md](./genai.md) | Mandatory AI-assisted development workflow: evidence capture, provenance labels, lifecycle, and completion gate. |
| [testing-and-tdd.md](./testing-and-tdd.md) | TDD cycle with observed RED/GREEN/REFACTOR evidence, required test levels, doubles versus real dependencies, coverage by critical behavior, and the mandatory TDD enforcement lifecycle across all Spec Kit phases (section 3). |
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
| testing-and-tdd.md | S | P | P | P | P | P | P | S |
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
- `testing-and-tdd.md` is primary from Phase 2 onward because TDD is enforced as a lifecycle, not only as an implementation practice. The specification must contain testable acceptance criteria, the plan must contain a test strategy, `tasks.md` must carry RED-first tasks with implementation depending on a RED checkpoint, and the pre-implementation analysis must reject missing or misordered test tasks. See [testing-and-tdd.md section 3](./testing-and-tdd.md#3-tdd-enforcement-across-spec-kit-phases).
- Phase 5 (analyze) uses `testing-and-tdd.md`, `clean-architecture.md`, and `security.md` as concrete checks against the specification, plan, and tasks.
- Phase 7 (review) uses `testing-and-tdd.md`, `coding-standards.md`, and `security.md` as the review rubric.
- Phase 8 (conclude) uses `delivery-and-ci.md` for README, GENAI.md, and submission readiness.

### TDD enforcement lifecycle

Missing TDD tasks or incorrect task dependencies are constitutional violations that MUST be resolved before implementation begins. The lifecycle runs:

```text
Specification   testable acceptance criteria incl. failure scenarios and edge cases
      |
Plan            test strategy across layers, PostgreSQL integration, scenarios mapped
      |         to acceptance criteria, tools/fixtures, test-first strategy
      |
Tasks           RED-first tasks; implementation depends on the RED checkpoint;
      |         no test/implementation pair parallelizable; per-user-story test tasks
      |
Analyze         verify coverage is planned, tests precede implementation, RED is
      |         explicit, no dependency allows implementing before RED is observed
      |
Implement       RED -> GREEN -> REFACTOR in order, with observed results recorded
      |
Review          confirm genuine test-first evidence; post-hoc tests are not TDD proof
```

The Spec Kit planning and task-generation templates and the pre-implementation Constitution Check MUST reflect this lifecycle once the constitution is ratified. Those edits are deferred to the constitution phase and are not made by the standards themselves.

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

## 6. Constitutional principles and their source standards

The constitution is **ratified**: [.specify/memory/constitution.md](../../.specify/memory/constitution.md),
version 1.0.0, ratified 2026-11-16. It establishes six non-negotiable principles. The standards
implement those principles and MUST NOT contradict them; where a standard is silent, the
constitution governs.

| # | Constitutional principle | Source standards |
| --- | --- | --- |
| I | Assessment-First and Specification-Driven Development | [genai.md](./genai.md), [testing-and-tdd.md](./testing-and-tdd.md) section 3, [assessment](../assessment/technical-exercise-v5.md) |
| II | Clean Architecture and SOLID | [clean-architecture.md](./clean-architecture.md), [solid-principles.md](./solid-principles.md), [coding-standards.md](./coding-standards.md) |
| III | Test-Driven Development (Non-Negotiable) | [testing-and-tdd.md](./testing-and-tdd.md) section 3, [genai.md](./genai.md) |
| IV | Secure REST APIs and Persistence | [api-standards.md](./api-standards.md), [security.md](./security.md), [database-standards.md](./database-standards.md) |
| V | Full-Stack Quality and Reproducible Delivery | [frontend-standards.md](./frontend-standards.md), [delivery-and-ci.md](./delivery-and-ci.md), [submission-requirements.md](../assessment/submission-requirements.md) |
| VI | Evidence-Based Generative AI Engineering | [genai.md](./genai.md), [.github/skills/genai-evidence/SKILL.md](../../.github/skills/genai-evidence/SKILL.md), [GENAI.md](../../GENAI.md) |

Earlier drafts proposed separate principles for proportional simplicity and readable code. Those are
now obligations inside Principles II, III, and V rather than standalone principles, so the
constitution stays concise: no abstraction without a recorded seam (II), no scope expansion without
a functional need (V), and consistent conventions enforced through the coding standards (II).

### Separation of engineering governance and submission process

The constitution governs **software engineering**: principles, architecture, testing, quality, security, and AI-assisted development. Assessment and submission **process** requirements are external obligations and are not constitutional principles.

- The submission deadline and the instruction to stop modifying the project after submission are process requirements owned by the developer. They are recorded for reference in [submission-requirements.md](../assessment/submission-requirements.md) and MUST NOT be ratified as engineering principles.
- The constitution MUST NOT contain a post-submission code freeze or any equivalent development restriction. It does not: Principle V records only the engineering-reviewable submission constraints and refers process obligations to the submission requirements document.
- The developer is responsible for submission timing and for complying with the recruiter's instructions. No hook, gate, workflow check, or agent restriction is added to enforce them.
- Engineering deliverables that support submission, such as tests, README content, and GENAI.md, remain engineering concerns and are covered by the standards above.

## 7. Established requirements versus proposed choices

| Category | Status | Examples |
| --- | --- | --- |
| Assessment requirements | **Established** - authoritative, unmodified | Clean Architecture, TDD, no EF/Dapper/Mediator, CRUD API plus user API, unit tests, frontend, README with seeded data |
| Submission requirements | **Established** - recorded from recruiter | One public repository, single link, presentation, root `GENAI.md`, no symbolic links, deadline. Process obligations such as the post-submission freeze are recorded in [submission-requirements.md](../assessment/submission-requirements.md) but are not engineering rules. |
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
7. Engineering documents impose only engineering rules. Assessment and submission process obligations, such as the post-submission freeze, remain in [submission-requirements.md](../assessment/submission-requirements.md) and are not restated as engineering requirements or enforced by tooling.

## 9. Status

| Item | Status |
| --- | --- |
| Engineering standards | Established in this task; pending developer review |
| Constitution | `PENDING` - unratified template at `.specify/memory/constitution.md` |
| Feature specification, plan, tasks | `PENDING` - not created |
| Application code, tests, schema | `PENDING` - not created |
| REST API GenAI exercise | `PENDING` - required by the assessment and not yet performed |
