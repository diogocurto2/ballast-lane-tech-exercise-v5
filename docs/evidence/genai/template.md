# EV-<number> - <activity title>

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-<number>` |
| Date | `<YYYY-MM-DD>` |
| Development phase | `<Spec Kit phase or activity>` |
| Activity type | `Documentation and specification` / `Code generation` / `Testing` / `Review` |
| Related specification | `<spec path, or PENDING with reason>` |
| Related task identifiers | `<task IDs, or PENDING with reason>` |
| AI tool | `<tool>` |
| AI model | `<model, or "not available">` |
| Evidence status | `Pending` / `Captured` / `Reviewed` / `Verified` |
| Commit references | `<recorded after the commit exists; not required before>` |

## Actual prompt

> Paste the developer-provided prompt verbatim. If the exact prompt is unavailable, state that and record it as unavailable. Do not paraphrase.

## Relevant context

- Requirement or assessment section:
- Engineering standard applied:
- Files or artifacts supplied as context:

## Original AI-generated output

Preserve a representative excerpt or diff exactly as produced, before corrections.

```text
<paste the original generated output>
```

## Validation performed

Record only the areas that apply to this activity. Mark the rest `Not Applicable` with a one-line justification.

| Area | What was checked | Result |
| --- | --- | --- |
| Clean Architecture | | |
| SOLID | | |
| TDD | | |
| Security | | |
| Functional requirements | | |

## Review findings

- `<finding>` - `[AI-PROPOSED]` / `[DEVELOPER DECISION]` / `[VERIFIED]`

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| | | `Pending` / `Approved` / `Rejected` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| | | |

Record `None - accepted as generated` with justification when no correction was needed.

## Commands and observed results

Record the commands that constitute real validation for this activity type - builds, tests, consistency checks - with their observed output. Do not log routine file reads or trivial shell operations.

```text
<command>
```

```text
<observed output>
```

## Final references

Use repository-relative paths. Record a commit reference only after that commit exists; an empty commit field before then is expected, not a blocker.

- Implementation files:
- Test files:
- Task identifiers:
- Commits:

## Evidence status rationale

Explain why the record has its current status and what remains before it can advance.
