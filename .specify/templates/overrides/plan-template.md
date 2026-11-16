# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]

**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Project override** of the installed plan template. Governed by
`.specify/memory/constitution.md`. Read
`docs/engineering/README.md` and the standards relevant to this
feature before filling this plan.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

**Language/Version**: [e.g. .NET 8 and TypeScript 5.x, or NEEDS CLARIFICATION]

**Primary Dependencies**: [e.g. ASP.NET Core, Npgsql, React, or NEEDS CLARIFICATION]

**Storage**: [e.g. PostgreSQL via Npgsql and ADO.NET, or NEEDS CLARIFICATION]

**Testing**: [e.g. xUnit, WebApplicationFactory, real PostgreSQL, Vitest, or NEEDS CLARIFICATION]

**Target Platform**: [e.g. Linux containers and modern browsers, or NEEDS CLARIFICATION]

**Project Type**: [web application - backend API plus frontend]

**Performance Goals**: [domain-specific or NEEDS CLARIFICATION]

**Constraints**: [domain-specific or NEEDS CLARIFICATION]

**Scale/Scope**: [domain-specific, appropriate to a small technical assessment]

## Constitution Check

*GATE: MUST pass before implementation. Re-check after design.*

Every principle MUST be answered explicitly. A `MUST` violation is blocking: fix it, or stop and
amend the constitution.

| Principle | Required evidence in this plan | Status |
|---|---|---|
| I. Assessment-first | Every requirement maps to an assessment obligation; pre-implementation `/speckit-analyze` scheduled | |
| II. Clean Architecture and SOLID | Dependency-direction matrix completed below; layer responsibilities named; abstractions justified | |
| III. TDD | Test strategy completed below, including unit tests for data access and controllers; RED-first task ordering stated and validated here as *planned* checkpoints (observed RED results are recorded during implementation); no test/implementation pair parallelized | |
| IV. Secure APIs and persistence | Authn/authz boundaries; resource-level authorization; parameterized SQL; constraints; no prohibited libraries | |
| V. Full-stack delivery | Responsive UI approach; seeded demo data; reproducible setup; README and `GENAI.md` obligations | |
| VI. GenAI evidence | Evidence capture points listed for every activity type | |

### Clean Architecture dependency analysis

Name the layers and complete the matrix. Dependencies MUST point inward only.

| From \ To | Domain | Application | Infrastructure | API | Frontend |
|---|---|---|---|---|---|
| Domain | - | | | | |
| Application | | - | | | |
| Infrastructure | | | - | | |
| API | | | | - | |
| Frontend | | | | | - |

For each non-empty cell, state whether the dependency is allowed and why:

- `Application -> Domain`: allowed.
- `Infrastructure -> Application`: allowed, the adapter implements an application-owned boundary.
- `API -> Infrastructure`: allowed **only** in the composition root, for dependency registration.
- `Application -> Infrastructure` and `Domain -> anything`: prohibited.

Business rules MUST NOT reference ASP.NET Core, Npgsql, PostgreSQL, or frontend libraries.

### SOLID design verification

| Principle | How this design satisfies it | Justification for any abstraction added |
|---|---|---|
| Single responsibility | | |
| Open/closed | | |
| Liskov substitution | | |
| Interface segregation | | |
| Dependency inversion | | |

No interface, factory, or extra layer may be added without a recorded seam or testability reason.

## Test Strategy (Principle III)

Complete this before implementation. It drives `/speckit-tasks`.

| Layer | Test type | Framework or tool | Real dependency or double | Scenarios mapped to acceptance criteria |
|---|---|---|---|---|
| Domain | Unit | | | |
| Application | Unit | | | |
| Infrastructure (data access) | Unit | | | |
| Infrastructure | Integration | Real PostgreSQL required | | |
| API controllers | Unit | | | |
| API endpoints | HTTP integration | In-process test host | | |
| Frontend | Component and user flow | | | |

Unit tests are mandatory for Infrastructure/data-access components and API controllers, in addition
to Domain and Application unit tests, real PostgreSQL integration tests, and HTTP integration
tests (constitution Principle III).

Also record:

- **Test environment**: database provisioning, migrations, seed, and fixtures.
- **Test-first implementation strategy**: how RED is established and observed for each behavior.
- **Critical behaviors**: authentication, authorization, ownership, validation, CRUD transitions,
  and persistence correctness.

## Security and Persistence Design (Principle IV)

- **Authentication**: mechanism, token lifetime, validation parameters.
- **Authorization**: how resource-level ownership is enforced, and in which layer.
- **Validation**: input-shape rules, business-rule rules, and where each lives.
- **Persistence**: tables, constraints, migrations, transactions, and indexes with the query that
  needs each one.
- **Secrets**: configuration source and example environment file.
- **Prohibited libraries**: confirm EF, EF Core, Dapper, Mediator, and MediatR are absent.

## GenAI Evidence Capture Points (Principle VI)

| Activity | Evidence record | Checkpoint expectations |
|---|---|---|
| Specification | | Prompt, generated spec, review findings |
| Planning | | Prompt, design alternatives, decisions |
| Task generation | | Prompt, task-to-requirement mapping |
| Implementation | | Original generated code before correction, RED and GREEN results |
| Testing | | Commands and observed results |
| Review | | Findings, corrections |

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Produced by /speckit-tasks
```

### Source Code (repository root)

Replace the placeholder tree with the concrete layout for this feature. Do not keep unselected
options in the delivered plan.

```text
src/
├── Domain/
├── Application/
├── Infrastructure/
└── Api/

tests/
├── unit/
├── integration/
└── http/

frontend/
├── src/
└── tests/
```

**Structure Decision**: [Record the selected structure, the layer boundaries, and how each project
maps to the dependency analysis above.]

## Complexity Tracking

> Fill ONLY where a constitutional rule is approached but not violated, or where a MUST requires a
> deviation approved by the developer through a constitution amendment. An unexplained entry is a
> blocking finding.

| Deviation | Why needed | Simpler alternative rejected because | Approved by |
|---|---|---|---|
| | | | |
