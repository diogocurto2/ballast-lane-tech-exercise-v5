# EV-0021 - Consolidated evidence for the assessment's REST API generation exercise

## Identification

| Field | Value |
| --- | --- |
| Evidence ID | `EV-0021` |
| Date | `2026-11-16` |
| Development phase | 8 - Conclude (T107 consolidation) |
| Activity type | `Documentation and specification` |
| Related specification | `specs/001-task-manager/spec.md` |
| Related task identifiers | `T107`, assembling `T010`-`T096` |
| AI tool | GitHub Copilot (VS Code, Copilot SDK) |
| AI model | Not exposed in this session |
| Evidence status | `Captured` |
| Commit references | `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires. |

## Actual prompt

The prompt the assessment requires, verbatim:

```text
/speckit-implement
```

This record does not restate the prompt from a later recollection: it was reproduced in `EV-0010`
when the activity started, and every story record repeats it because it remained the only developer
instruction for the whole implementation.

## Relevant context

- Assessment obligations: `DO-02` (the Task Management REST API exercise) in
  [docs/assessment/technical-exercise-v5.md](../assessment/technical-exercise-v5.md).
- Source records consolidated here: `EV-0010` (setup and foundational), `EV-0011` (US1), `EV-0012`
  (US2), `EV-0013` (US3), `EV-0014` (US4), `EV-0015` (US5), `EV-0016` (US6), `EV-0017` (US7), and
  `EV-0018` (verification).
- Governing standard: [docs/engineering/genai.md](../../engineering/genai.md).

## Original AI-generated output

`Not Applicable` to create anew: this record consolidates output preserved elsewhere. The excerpts
below are copied from the records named beside them, which hold them in their original form.

| Excerpt | Record |
| --- | --- |
| `.csproj` files generated without `TargetFramework`, causing `NETSDK1013` | `EV-0010` |
| `Request DTOs` with `[Required]` producing `Email`/`Password` error keys | `EV-0011` |
| `AddJwtBearer` without a challenge handler, producing an empty `401` body | `EV-0011` |
| `AuthProvider` that could never authenticate | `EV-0011` |
| `ITaskWriter` generated with update and delete before their stories | `EV-0014` |
| `TaskValidator.Validate` treating an omitted status as `Pending` on update | `EV-0015` |
| Repository method region replaced by a duplicate `DeleteAsync` after an edit | `EV-0016` |

## Validation performed

| Area | Which was checked | Result |
| --- | --- | --- |
| Clean Architecture | Architecture test over the project files; `Infrastructure` referenced only by the three composition roots | `[VERIFIED]` |
| SOLID | Abstractions only where a seam exists; the writer interface was narrowed rather than pre-implemented | `[VERIFIED]` |
| TDD | RED observed for every story; four first-attempt passes were re-established by removing the protecting behaviour | `[VERIFIED]` |
| Security | Cross-user access, ownership tampering, injection payloads, token failures, credential secrecy | `[VERIFIED]` |
| Functional requirements | All 17 FRs and 6 NFRs trace to tests or executed quickstart steps | `[VERIFIED]` |

## Review findings

- `[VERIFIED]` Every claim consolidated in this record points to a source record containing the
  observed command output. No claim in `GENAI.md` is supported only by this record.
- `[VERIFIED]` Two assessment questions are answered incompletely and are labelled `[PENDING]` in
  `GENAI.md` rather than claimed: keyboard-only traversal and focus visibility, and an
  expired-token redirect observed in a browser, both of which the available harness could not perform.
- `[VERIFIED]` The developer has not reviewed any record in this set. Every record therefore remains
  `Captured`, including the four earlier records whose review is outstanding.

## Developer decisions and approval

| Decision | Rationale | Approval |
| --- | --- | --- |
| Consolidate rather than re-derive | Re-deriving output after the fact would be reconstruction, which the standard prohibits | `[AI-PROPOSED]` |
| Keep `[PENDING]` items visible in `GENAI.md` | Readiness must not be claimed while a mandatory check is unperformed | `[AI-PROPOSED]` |

## Corrections and improvements

| Change | Reason | Technical justification |
| --- | --- | --- |
| `GENAI.md` sections 2-10 rewritten from planned to observed | The document described pending work that now exists | `delivery-and-ci.md` 2.6 and the standard's prohibition on presenting planned work as complete |
| Evidence index and coverage table updated with `EV-0010` to `EV-0018` and `EV-0021` | The assessment questions must point at actual records | Traceability is checkable only if the pointer resolves |

## Commands and observed results

```text
dotnet build BallastLane.sln   -> Build succeeded. 0 Warning(s) 0 Error(s)
dotnet test BallastLane.sln    -> 211 passed, 0 failed, 0 skipped
cd frontend && npx vitest run  -> 8 files, 27 tests passed
```

These are the numbers cited in `GENAI.md`; they were observed in `EV-0018` immediately before this
record was written.

## Final references

- Documentation files: `GENAI.md`, `docs/evidence/genai/README.md`
- Source records: `EV-0010` to `EV-0018`
- Task identifiers: `T107`
- Commits: `2f0844b` - `feat(api): implement the task manager backend with Npgsql and ADO.NET`; `9cd6b3a` - `feat(web): implement the React task manager client`; `9dfae4d` - `docs: record the implementation evidence and the backend layout`. Recorded after these commits were created, as the standard requires.

## Evidence status rationale

Status is `Captured`. The consolidated statements are traceable to records with observed output, but
the developer review and the commits that would make this set `Verified` do not exist yet.
