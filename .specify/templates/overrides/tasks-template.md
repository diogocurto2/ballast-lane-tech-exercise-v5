---

description: "Task list template for feature implementation (project override enforcing RED-first TDD)"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md,
data-model.md, contracts/

**Project override** of the installed tasks template. Governed by
`.specify/memory/constitution.md` Principle III.

**Tests are MANDATORY.** Every new or changed runtime behavior MUST have test tasks that precede
its implementation task. Do not treat testing as a final project-wide phase, and do not leave tests
out because the specification did not request them.

**Organization**: Tasks are grouped by user story so each story is independently implementable and
testable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks).
- **[Story]**: Which user story this task belongs to (US1, US2, US3...).
- Include exact file paths in descriptions.

## TDD Task Pattern (mandatory)

For each independently testable behavior, or coherent group of related behaviors, generate tasks in
this order:

1. Write the relevant tests.
2. Run them and observe the expected RED failure.
3. Implement the minimum behavior required.
4. Run them and observe GREEN.
5. Refactor if needed, then rerun.
6. Record TDD and GenAI evidence for the activity.

Rules that MUST hold in the generated list:

- The implementation task MUST depend on the RED checkpoint task.
- A test task and its corresponding implementation task MUST NOT both carry `[P]`, and MUST NOT be
  scheduled in parallel.
- Evidence capture MUST appear in the same activity as the work, not collected at the end.
- Each user story MUST carry its own test-first tasks.

Documentation-only and non-behavioral infrastructure tasks MAY be marked `Not Applicable` for TDD
with a written justification and an alternative validation task.

## Path Conventions

- **Web app**: `src/`, `tests/`, `frontend/src/`, `frontend/tests/` at repository root.
- Adjust to the structure recorded in plan.md.

<!--
  IMPORTANT: The tasks below are SAMPLE TASKS. /speckit-tasks MUST replace them with real tasks
  derived from spec.md user stories, plan.md requirements, data-model.md entities, and
  contracts/. Do not keep sample tasks in the generated file.
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Create solution and project structure per plan.md
- [ ] T002 Initialize backend projects with required dependencies
- [ ] T003 Initialize frontend project with required dependencies
- [ ] T004 [P] Configure formatting, linting, and nullable reference types
- [ ] T005 [P] Configure test projects and the integration test database harness

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure required before any user story

**CRITICAL**: No user story work begins until this phase completes.

- [ ] T006 Create versioned SQL migrations for the schema in plan.md
- [ ] T007 Implement connection handling and migration runner
- [ ] T008 Implement authentication and authorization wiring
- [ ] T009 Implement error handling and ProblemDetails mapping
- [ ] T010 Implement structured logging with sensitive-data exclusion
- [ ] T011 Implement idempotent seed for demonstration users and records
- [ ] T012 Write architecture verification test asserting dependency direction

**Checkpoint**: Foundation ready.

---

## Phase 3: User Story 1 - [Title] (Priority: P1)

**Goal**: [What this story delivers]

**Independent Test**: [How to verify this story alone]

### Tests for User Story 1 (MANDATORY)

- [ ] T013 [P] [US1] Write unit tests for [behavior] in tests/unit/[name].cs
- [ ] T014 [P] [US1] Write integration test against real PostgreSQL in tests/integration/[name].cs
- [ ] T015 [P] [US1] Write HTTP integration test for [endpoint] in tests/http/[name].cs
- [ ] T016 [US1] Run US1 tests and record the observed RED failure (depends on T013, T014, T015)

### Implementation for User Story 1

- [ ] T017 [US1] Implement [domain rule] in src/Domain/[file].cs (depends on T016 RED)
- [ ] T018 [US1] Implement [use case] in src/Application/[file].cs (depends on T017)
- [ ] T019 [US1] Implement [repository] in src/Infrastructure/[file].cs (depends on T018)
- [ ] T020 [US1] Implement [endpoint] in src/Api/[file].cs (depends on T019)
- [ ] T021 [US1] Run US1 tests and record observed GREEN (depends on T020)
- [ ] T022 [US1] Refactor US1 code, rerun tests, record result (depends on T021)
- [ ] T023 [US1] Record US1 TDD and GenAI evidence in the activity evidence record

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [What this story delivers]

**Independent Test**: [How to verify this story alone]

### Tests for User Story 2 (MANDATORY)

- [ ] T024 [P] [US2] Write unit tests for [behavior] in tests/unit/[name].cs
- [ ] T025 [P] [US2] Write security tests for [authz case] in tests/http/[name].cs
- [ ] T026 [US2] Run US2 tests and record the observed RED failure (depends on T024, T025)

### Implementation for User Story 2

- [ ] T027 [US2] Implement [behavior] in src/[layer]/[file].cs (depends on T026 RED)
- [ ] T028 [US2] Run US2 tests and record observed GREEN (depends on T027)
- [ ] T029 [US2] Refactor US2 code, rerun tests, record result (depends on T028)
- [ ] T030 [US2] Record US2 TDD and GenAI evidence in the activity evidence record

**Checkpoint**: User Stories 1 and 2 work independently.

---

[Add further user story phases using the same test-first pattern.]

---

## Phase N: Verification and Convergence

**Purpose**: Cross-cutting verification that is not behavior of a single story

- [ ] TXXX [P] Verify architectural dependency rules by inspecting project references
- [ ] TXXX [P] Verify no prohibited libraries are referenced, directly or transitively
- [ ] TXXX [P] Verify security behaviors: unauthorized, cross-user, and validation cases
- [ ] TXXX Verify full suite passes from a clean checkout
- [ ] TXXX Verify frontend lint, type check, and tests pass
- [ ] TXXX Verify README and GENAI.md claims match implemented behavior
- [ ] TXXX Record final evidence and commit references

---

## Dependencies and Execution Order

### Phase dependencies

- **Setup**: no dependencies.
- **Foundational**: depends on Setup; blocks all user stories.
- **User stories**: depend on Foundational.
- **Verification**: depends on the stories being complete.

### Within each user story

```text
write tests -> observe RED -> implement -> observe GREEN -> refactor -> record evidence
```

- Tests MUST be written and observed failing before implementation.
- The implementation task MUST list its RED checkpoint as a dependency.
- Models before services, services before endpoints.
- A story MUST be independently testable before the next priority begins.

### Parallel opportunities

`[P]` applies to independent test files, independent models, and independent stories. It MUST NOT
be applied across a test/implementation pair, and MUST NOT allow implementation before its RED
checkpoint is observed.

---

## Notes

- `[P]` tasks touch different files and have no incomplete dependencies.
- `[Story]` maps each task to a user story for traceability.
- `Not Applicable` for TDD requires a written justification and a substitute validation task.
- Commit after each logical group; do not commit without the evidence that supports it.
- Avoid vague tasks, same-file conflicts, and cross-story dependencies that break independence.
