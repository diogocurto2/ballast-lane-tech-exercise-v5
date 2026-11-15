# Repository Copilot Instructions

This repository is the solution to the Ballast Lane Senior .NET technical assessment.

## Required reading

- Read [docs/assessment/technical-exercise-v5.md](../docs/assessment/technical-exercise-v5.md) before making relevant project decisions.
- Read the project constitution and, when present, the active specification, plan, and tasks under `specs/` before implementation work.

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
- If mandatory evidence is missing, stop and report it as a completion blocker instead of marking the task complete.
- Do not modify the original assessment document.

## Status

Spec Kit 1.4.0 is installed and configured with the Copilot integration. The constitution at `.specify/memory/constitution.md` is still the unratified template; no specification, plan, or task list exists. The REST API generation exercise and its evidence are pending. Only the documentation and configuration work recorded in [GENAI.md](../GENAI.md) has been performed so far.
