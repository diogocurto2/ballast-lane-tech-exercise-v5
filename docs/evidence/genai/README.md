# GenAI Evidence Records

Evidence of AI-assisted engineering for the Ballast Lane Technical Interview Exercise V5.

Each record captures what the developer actually asked, what the AI actually produced, what was validated, and what was decided. Records are the source material for [GENAI.md](../../../GENAI.md) and for the assessment's Generative AI section.

## How to use this folder

- Create one record per meaningful development activity, or one record for a related group of tasks.
- Copy [template.md](./template.md) and fill it in during the activity, not afterwards.
- Name records `EV-<number>-<short-slug>.md`, for example `EV-0002-task-api-scaffold.md`.
- Reference the record from the related task, commit, and GENAI.md section.
- Follow the standard in [docs/engineering/genai.md](../../engineering/genai.md).

## Record lifecycle

| Status | Meaning |
| --- | --- |
| `Pending` | Record created, evidence not yet captured. |
| `Captured` | Prompt, context, and artifacts recorded; developer review not yet complete. |
| `Reviewed` | Review findings and developer decisions recorded. |
| `Verified` | Final code, tests, and observed validation confirmed; commit references recorded once the commits exist. |

A record moves forward only when the evidence exists. If a later check invalidates an earlier claim, correct the record and its status. The status describes the record, not the repository: capture, developer review, observed validation, and committed artifacts are separate facts, and a commit reference is recorded only after that commit exists.

## What belongs in a record

- The actual prompt as provided by the developer, and the context supplied with it.
- Representative original AI-generated output, preserved before corrections.
- Validation performed: architecture, SOLID, TDD, security, and functional checks, scaled to the activity type.
- Review findings and developer decisions, with technical rationale.
- Corrections and improvements, or the justification for accepting output unchanged.
- The commands and observed results that constitute real validation, including failures.
- References to final files, tests, task identifiers, and commits once those commits exist.

## Proportionality

Record one activity, or one related group of tasks, per record. Do not create records per command or per trivial edit, and do not log routine file reads. Evidence expectations differ by activity:

| Activity type | Required evidence |
| --- | --- |
| Documentation and specification | Actual prompt, generated document or representative excerpt, review findings, relevant consistency checks |
| Code generation | Original generated code, TDD evidence, review findings, corrections, validation results |
| Testing | Actual commands, observed results, failures and their resolution |
| Review | Concrete findings, decisions, verified corrections |

Requirements that do not apply are marked `Not Applicable` with a one-line justification.

## What must never be recorded

- Secrets, credentials, tokens, API keys, connection strings, or personal data.
- Machine-specific absolute paths. Published references use repository-relative paths.
- Invented or reconstructed prompts, output, defects, corrections, results, commit references, or approvals.
- Full unrelated chat transcripts, or private model reasoning.
- Planned work presented as completed work.

If sensitive information must be removed, say explicitly that the content was redacted and what kind of information was removed. Redacted content is never presented as verbatim.

## Index

| ID | Activity | Phase | Status | Record |
| --- | --- | --- | --- | --- |
| EV-0001 | Repository GenAI engineering standard and evidence workflow | 1 - Constitution and engineering standards | Verified | [EV-0001-repository-genai-standards.md](./EV-0001-repository-genai-standards.md) |
| EV-0002 | Refinement of the GenAI standard per developer review feedback | 1 - Constitution and engineering standards | Verified | [EV-0002-standard-refinement.md](./EV-0002-standard-refinement.md) |
| EV-0003 | Engineering standards, submission requirements, and documentation index | 1 - Constitution and engineering standards | Verified | [EV-0003-engineering-standards.md](./EV-0003-engineering-standards.md) |
| EV-0004 | TDD enforcement lifecycle across Spec Kit phases | 1 - Constitution and engineering standards | Verified | [EV-0004-tdd-enforcement-lifecycle.md](./EV-0004-tdd-enforcement-lifecycle.md) |
| EV-0005 | Separation of engineering governance and submission process rules | 1 - Constitution and engineering standards | Verified | [EV-0005-governance-separation.md](./EV-0005-governance-separation.md) |
| EV-0006 | Constitution ratification and Spec Kit enforcement integration | 1 - Constitution and engineering standards | Reviewed | [EV-0006-constitution-ratification.md](./EV-0006-constitution-ratification.md) |
| EV-0007 | Targeted governance corrections and feature specification `001-task-manager` | 2 - Specify | Captured | [EV-0007-specification-001-task-manager.md](./EV-0007-specification-001-task-manager.md) |
| EV-0008 | Implementation plan, research, and Phase 1 design | 3 - Plan | Captured | [EV-0008-implementation-plan-001-task-manager.md](./EV-0008-implementation-plan-001-task-manager.md) |
| EV-0009 | Task generation with RED-first ordering | 4 - Tasks | Captured | [EV-0009-task-generation-001-task-manager.md](./EV-0009-task-generation-001-task-manager.md) |
| EV-0020 | Pre-implementation analysis of `001-task-manager` (4 blocking findings) | 5 - Analyze | Captured | [EV-0020-analysis-001-task-manager.md](./EV-0020-analysis-001-task-manager.md) |
| EV-0022 | Remediation of the CRITICAL findings and re-analysis | 5 - Analyze | Captured | [EV-0022-critical-remediation-and-reanalysis.md](./EV-0022-critical-remediation-and-reanalysis.md) |
| EV-0023 | Final pre-implementation corrections and second re-analysis | 5 - Analyze | Captured | [EV-0023-final-corrections-and-second-reanalysis.md](./EV-0023-final-corrections-and-second-reanalysis.md) |
| EV-0010 | Setup scaffolding and foundational layer | 6 - Implement | Captured | [EV-0010-setup-and-foundational-tdd.md](./EV-0010-setup-and-foundational-tdd.md) |
| EV-0011 | US1 registration and authentication | 6 - Implement | Captured | [EV-0011-us1-registration-authentication.md](./EV-0011-us1-registration-authentication.md) |
| EV-0012 | US2 public and protected access boundary | 6 - Implement | Captured | [EV-0012-us2-access-boundary.md](./EV-0012-us2-access-boundary.md) |
| EV-0013 | US3 list and view personal tasks | 6 - Implement | Captured | [EV-0013-us3-task-read.md](./EV-0013-us3-task-read.md) |
| EV-0014 | US4 create a task | 6 - Implement | Captured | [EV-0014-us4-task-create.md](./EV-0014-us4-task-create.md) |
| EV-0015 | US5 update a task | 6 - Implement | Captured | [EV-0015-us5-task-update.md](./EV-0015-us5-task-update.md) |
| EV-0016 | US6 delete a task | 6 - Implement | Captured | [EV-0016-us6-task-delete.md](./EV-0016-us6-task-delete.md) |
| EV-0017 | US7 seeded demonstration environment | 6 - Implement | Captured | [EV-0017-us7-demonstration-environment.md](./EV-0017-us7-demonstration-environment.md) |
| EV-0018 | Verification and convergence | 7 - Review | Captured | [EV-0018-review-and-convergence.md](./EV-0018-review-and-convergence.md) |
| EV-0021 | Consolidated evidence for the REST API generation exercise | 8 - Conclude | Captured | [EV-0021-genai-api-exercise.md](./EV-0021-genai-api-exercise.md) |
| EV-0024 | Backend folder restructure and confirmation of the Docker build context | 9 - Post-implementation follow-up | Captured | [EV-0024-backend-folder-restructure.md](./EV-0024-backend-folder-restructure.md) |
| EV-0025 | Remediation of the second code review findings (session handling, login consistency, styling) | 10 - Post-review remediation | Captured | [EV-0025-review-remediation.md](./EV-0025-review-remediation.md) |
| EV-0026 | Final delivery-readiness corrections (README and quickstart guides, OpenAPI exposure, stale-token session isolation, submission status) | 11 - Final delivery readiness | Reviewed | [EV-0026-delivery-readiness-corrections.md](./EV-0026-delivery-readiness-corrections.md) |

The mandatory REST API generation exercise has been executed; its evidence is consolidated in
`EV-0021` from the records above. Every record created during implementation now carries its commit
references: `2f0844b` for the backend, `9cd6b3a` for the frontend client, and `9dfae4d` for the
records and documentation themselves. Developer review of the records remains `PENDING`, which is why
none has advanced past `Captured`.

`EV-0024` was raised after implementation, at developer request, and required no application behavior
change. Records `EV-0010` to `EV-0018`, `EV-0020`, `EV-0022`, and `EV-0023` describe the repository
layout as it was at the time of their activity, when the backend lived at the root as `src/`,
`tests/`, and `db/`; each carries a dated revision note pointing at `EV-0024` for the current paths.
`EV-0021` needed no note because it does not cite those locations.

`EV-0025` is a post-review remediation activity raised by the developer against commit `a42d631`. It
changed frontend behavior (session invalidation, login consistency), the stylesheet, and the wording of
`EV-0015`, `EV-0018`, and `GENAI.md` where a post-implementation mutation check had been described as a
test-first RED cycle. The corrections to those earlier records are dated and preserve the observed
output; no historical result was changed to match a later one.

`EV-0026` covers the final delivery-readiness pass. Besides the session and OpenAPI corrections, it
rewrote the setup and test guidance in `README.md` and `quickstart.md`, refreshed the status summary in
`docs/assessment/submission-requirements.md` without touching the recorded requirements, and verified
the whole repository from a fresh tree built from exactly what a commit of the work would contain. Its
code landed in `0d6c2b9` and `02747ee`; the developer reviewed the diff and instructed the commit on
2026-11-17, which is the basis recorded for the approved decisions.
