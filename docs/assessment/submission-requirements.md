# Submission Requirements

Additional delivery requirements for the Ballast Lane Technical Interview Exercise V5.

## 1. Purpose and scope

The original assessment ([technical-exercise-v5.md](./technical-exercise-v5.md)) defines the technical work. This document records the additional **submission and delivery** requirements communicated by the recruiter, so they are tracked as verifiable requirements rather than remembered informally.

The original assessment document is preserved unmodified. Where a requirement below restates an assessment obligation, the assessment remains authoritative for the technical content.

**Provenance note:** the recruiter correspondence itself is not reproduced here. It may contain personal information and private communication, so these requirements are recorded as a summary only.

## 2. Requirements

| ID | Requirement | Type | Verification |
| --- | --- | --- | --- |
| SUB-01 | The solution MUST be delivered through one public GitHub repository. | Delivery | Repository is publicly reachable and contains the complete solution. |
| SUB-02 | Exactly one GitHub link MUST be sent for the completed code. Do not send multiple links or per-folder links. | Delivery | A single repository URL is the submission artifact. |
| SUB-03 | A presentation of the development process and engineering rationale MUST be included. A README or a text file is acceptable. | Documentation | [README.md](../../README.md) covers setup, execution, testing, architecture, user story, and engineering rationale; see [delivery-and-ci.md](../engineering/delivery-and-ci.md). |
| SUB-04 | The Generative AI portion of the assessment is mandatory. | Documentation | [GENAI.md](../../GENAI.md) answers every Generative AI question in the assessment, backed by records in [docs/evidence/genai/](../evidence/genai/README.md). |
| SUB-05 | The Generative AI question MUST be answered in a root-level `GENAI.md` file. | Documentation | `GENAI.md` exists at the repository root. |
| SUB-06 | Symbolic links MUST NOT be included. | Repository hygiene | No symbolic links in tracked files; verified before submission. |
| SUB-07 | The submission deadline is Monday, 16 November 2026, 08:00 Brazil time (UTC-03:00). | Schedule | Deadline is fixed and outside project control. |
| SUB-08 | After submission, the project MUST NOT be modified until further notice. | Process | Repository freeze after the submission link is sent. |

## 3. Derived obligations

These follow from the requirements above. Engineering deliverables are owned by the delivery standard; process obligations are owned by the developer.

- **SUB-01, SUB-02** - the repository must start from a clean public state: no secrets, no local absolute paths, no unrelated personal material. See [delivery-and-ci.md](../engineering/delivery-and-ci.md) section on public repository hygiene.
- **SUB-03, SUB-04, SUB-05** - `README.md` and `GENAI.md` are submission deliverables, not optional documentation. They must be accurate at submission time and must not describe unimplemented functionality.
- **SUB-06** - symbolic links are prohibited; directory traversal and tooling must not create them. Spec Kit is configured with `allow_symlinks: false` in `.specify/init-options.json`, which is consistent with this requirement.
- **SUB-07** - the deadline constrains scope. Engineering standards must remain practical for a small assessment; see the simplicity rules in [docs/engineering/README.md](../engineering/README.md).
- **SUB-08** - a **process obligation**, not an engineering rule. The repository is not modified after submission until notified. The developer owns timing and compliance. It is recorded here for reference only: it MUST NOT appear in the project constitution, in the engineering standards, or as an automated gate, hook, or agent restriction.

## 4. Status at the time of writing

| Item | Status |
| --- | --- |
| Backend, API, data layer, frontend | `PENDING` - not implemented |
| Unit, integration, and frontend tests | `PENDING` - no test project exists |
| `README.md` submission content | `PENDING` - currently a placeholder |
| `GENAI.md` REST API exercise answers | `PENDING` - required and explicitly not yet answered |
| Engineering standards | Established in this task; see [docs/engineering/README.md](../engineering/README.md) |
| Constitution | `PENDING` - `.specify/memory/constitution.md` is still the unratified template |

Nothing in this table is presented as complete.

## 5. Verification before submission

Run these checks before sending the submission link:

1. `SUB-01` - open the repository URL while signed out, or in a private window, and confirm it is reachable.
2. `SUB-06` - confirm no symbolic links are tracked in Git.
3. `SUB-03`, `SUB-04`, `SUB-05` - confirm `README.md` and `GENAI.md` exist at the required locations and contain no unverified claims.
4. Assessment requirements - confirm every requirement in [technical-exercise-v5.md](./technical-exercise-v5.md) is satisfied and traceable to code or documentation.
5. Public hygiene - confirm no secrets, credentials, tokens, personal data, or machine-specific absolute paths are present.
6. `SUB-08` - after sending the link, stop changing the repository.

## 6. References

- Original assessment: [technical-exercise-v5.md](./technical-exercise-v5.md)
- Delivery and CI standard: [delivery-and-ci.md](../engineering/delivery-and-ci.md)
- Security standard: [security.md](../engineering/security.md)
- Generative AI standard: [genai.md](../engineering/genai.md)
- Assessment response: [GENAI.md](../../GENAI.md)
