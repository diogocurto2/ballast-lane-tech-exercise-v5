---
name: genai-evidence
description: "Capture and maintain evidence of AI-assisted engineering for the Ballast Lane Technical Interview Exercise V5. Use for every meaningful specification, planning, implementation, testing, review, or documentation task: record the actual prompt and supplied context, preserve original AI-generated artifacts before corrections, log actual commands and observed results, document review findings and developer decisions, and update the matching evidence record plus GENAI.md. Use when starting or finishing Spec Kit phases or tasks, generating or correcting code, running tests, reviewing AI output, or writing project documentation. Never fabricate prompts, output, defects, corrections, or results."
compatibility: "Requires docs/engineering/genai.md, docs/evidence/genai/template.md, and GENAI.md at the repository root"
metadata:
  version: "1.0.0"
  source: "docs/engineering/genai.md"
---

# GenAI Evidence

Enforce the repository's Generative AI engineering standard while work is happening, so evidence is observed rather than reconstructed.

## When to use this skill

Use it for every meaningful activity: specification, clarification, planning, task generation, implementation, test authoring or execution, code review, refactoring, and documentation changes. Skip it only for trivial read-only lookups that produce no project artifact and no decision.

## Non-negotiable rules

- Never invent a prompt, generated artifact, defect, correction, command result, approval, or commit reference.
- Record the actual prompt as given. If the exact prompt is unavailable, ask the developer or record it as unavailable; do not paraphrase it to look verbatim.
- Preserve original AI-generated code before correcting it. Store corrections separately.
- Record the commands and observed results that matter for the activity, including failures. Do not log routine file reads, exploratory lookups, or trivial shell operations.
- Label content `[AI-PROPOSED]`, `[DEVELOPER DECISION]`, `[VERIFIED]`, or `[PENDING]`.
- Only `[VERIFIED]` may be reported as a completed outcome.
- Mark requirements that do not apply to an activity as `Not Applicable` with a one-line justification.
- Never store secrets, credentials, tokens, connection strings, or personal data. If redaction is required, say so explicitly instead of presenting redacted content as verbatim.
- Use repository-relative paths; do not record machine-specific absolute paths.

## Checkpoint 1 - Before execution

Complete this before producing artifacts.

1. Identify the current Spec Kit phase, task identifier, or development activity. The standard order is: constitution and standards, specify and clarify, plan, tasks, analyze (mandatory, before implementation), implement with TDD, review and converge, conclude. Analysis of specification, plan, tasks, and constitutional compliance is never skipped before implementation.
2. Record the actual user-provided prompt and the context supplied: specification, plan, tasks, constitution, engineering standards, assessment sections, and file paths.
3. Classify the activity type - documentation and specification, code generation, testing, or review - so the applicable evidence requirements are known in advance.
4. Identify the applicable requirements and standards (Clean Architecture, SOLID, TDD, security, functional requirements).
5. Create or open the corresponding evidence record from [template.md](../../../docs/evidence/genai/template.md). Reuse an existing record for a related group of tasks instead of duplicating it.
6. Set the record status to `Pending` or `Captured` as appropriate.

## Checkpoint 2 - During execution

Complete this while the work is in progress, not afterwards.

1. Preserve meaningful AI-generated artifacts exactly as produced.
2. For code generation, capture a representative original excerpt or diff before corrections are applied.
3. Record the commands that constitute real validation - builds, tests, schema checks, consistency checks - with their observed results, including failures. Omit routine reads and trivial operations.
4. Preserve TDD evidence when applicable: the failing test, the failure output, then the implementation, then the passing result.
5. Record significant design alternatives and the decisions taken, with technical rationale. State clearly which options were rejected and why.
6. Do not back-fill any of the above after the fact. If it was not captured when it occurred, mark it unavailable.

## Checkpoint 3 - Before task completion

Complete this before reporting the task as done.

1. Record actual review findings from evaluating the AI output.
2. Document corrections and improvements with technical justification, or justify acceptance of the AI output as-is.
3. Reference the final implementation files, test files, and task identifiers. Record commit references only after the relevant commit exists; an absent commit reference is not a blocker before that point.
4. Update the root [GENAI.md](../../../GENAI.md) with the verified evidence that belongs in the assessment response. Keep it a living assessment response, not a transcript.
5. Mark unavailable or incomplete evidence as `PENDING` with the reason, and requirements that do not apply as `Not Applicable` with a justification.
6. If mandatory evidence is missing, report it as a completion blocker and do not mark the task complete.
7. Do not claim that this skill was discovered, loaded, or invoked unless that was actually observed in the session.

## Status transitions

`Pending` -> `Captured` -> `Reviewed` -> `Verified`.

Advance a status only when the corresponding evidence exists. Downgrade a status if evidence is found to be inaccurate or unverifiable.

The status describes the evidence record, not the repository. Capture, developer review, observed validation, and committed artifacts are tracked as separate facts, so a record can be reviewed before its commit exists.

## References

- Standard: [docs/engineering/genai.md](../../../docs/engineering/genai.md)
- Evidence records: [docs/evidence/genai/](../../../docs/evidence/genai/README.md)
- Assessment: [docs/assessment/technical-exercise-v5.md](../../../docs/assessment/technical-exercise-v5.md)
