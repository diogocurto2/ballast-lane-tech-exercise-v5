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

The mandatory REST API generation exercise from the assessment has not started. Its evidence records are pending and will be added when that work begins.
