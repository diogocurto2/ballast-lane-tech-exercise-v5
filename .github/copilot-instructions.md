# Repository Copilot Instructions

This repository is the solution to the Ballast Lane Senior .NET technical assessment.

## Required reading

- Read [docs/assessment/technical-exercise-v5.md](../docs/assessment/technical-exercise-v5.md) before making relevant project decisions.
- Read [docs/assessment/submission-requirements.md](../docs/assessment/submission-requirements.md) for the assessment deliverables and delivery constraints, including the submission deadline.
- Read [docs/engineering/README.md](../docs/engineering/README.md) for the engineering standards index.
- Consult the relevant engineering standards **before** any specification, planning, implementation, testing, or review activity:
  - Testing and TDD: [testing-and-tdd.md](../docs/engineering/testing-and-tdd.md)
  - SOLID: [solid-principles.md](../docs/engineering/solid-principles.md)
  - Clean Architecture: [clean-architecture.md](../docs/engineering/clean-architecture.md)
  - Coding standards: [coding-standards.md](../docs/engineering/coding-standards.md)
  - API standards: [api-standards.md](../docs/engineering/api-standards.md)
  - Security: [security.md](../docs/engineering/security.md)
  - Database: [database-standards.md](../docs/engineering/database-standards.md)
  - Frontend: [frontend-standards.md](../docs/engineering/frontend-standards.md)
  - Delivery and CI: [delivery-and-ci.md](../docs/engineering/delivery-and-ci.md)
- Read the project constitution and, when present, the active specification, plan, and tasks under `specs/` before implementation work.

## Prohibited technologies

- Entity Framework, EF Core, Dapper, MediatR, and Mediator MUST NOT be used for data access.
- Do not add AWS, Redis, Kubernetes, or other infrastructure without a functional need recorded in the plan.

## Evidence obligations

- Follow [docs/engineering/genai.md](../docs/engineering/genai.md) for all AI-assisted work.
- Use the `genai-evidence` skill for every meaningful specification, planning, coding, testing, review, or documentation task.
- Capture the actual prompt and supplied context before generating artifacts.
- Capture original AI-generated artifacts, including representative code excerpts or diffs, before applying corrections.
- Record the validation commands and observed results that matter for the activity, including failures. Do not log routine file reads or trivial shell operations, and scale the evidence to the activity type.
- Update the relevant record in `docs/evidence/genai/` and the matching section of [GENAI.md](../GENAI.md) before concluding the task.
- Record commit references only after the relevant commit exists.

## Honesty rules

- Never fabricate prompts, generated code, defects, corrections, test results, approvals, commit references, or metrics.
- Never present pending or planned work as completed.
- Never claim that the skill was discovered, loaded, or invoked unless that was observed.
- Never claim application code, tests, or a database exist before they do.
- If mandatory evidence is missing, stop and report it as a completion blocker instead of marking the task complete.
- Do not modify the original assessment document.
- Do not commit or push without developer review.

## Status

Spec Kit 1.4.0 is installed and configured with the Copilot integration. The constitution at `.specify/memory/constitution.md` is still the unratified template; no specification, plan, or task list exists. Engineering standards are established and await developer review.

The REST API generation exercise and its evidence are pending. Only the documentation and configuration work recorded in [GENAI.md](../GENAI.md) has been performed so far.
