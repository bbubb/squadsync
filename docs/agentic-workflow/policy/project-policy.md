# Project Policy

## Status

Supervised execution is active. The Sprint 12 delegated pilot requires a separate owner activation.

## Purpose

This policy defines the durable rules that all human and AI contributors must follow when working on SquadSync.

It does not replace `AGENTS.md`. Instead, it organizes the workflow-specific policy used by agentic planning and implementation.

## Source of Truth

Follow the task-based startup and context routing in root `AGENTS.md` and `CONTRIBUTING.md`: for implementation, load the active issue before broad context, confirm readiness, then read the scoped documents and ADRs required by that task. Use `README.md`, roadmap, MVP scope and architecture sources when the question or affected code requires them; they are not a mandatory preload bundle.

## Human-Owned Architecture

The human owner retains authority over:

- product scope
- architecture direction
- ADR approval
- merge authority and any grant that delegates its exercise
- toolchain changes
- agent workflow changes

Agents may recommend changes, draft docs, create issues, implement scoped work, and summarize trade-offs. Agents should not silently expand architecture or scope.

## Execution Policy

Execution policy determines who may decide, implement, review, advance work and merge. At dispatch, resolve the work scope from the owner's assignment and approved GitHub issue or tracker. The assignment may cover an issue, sprint, phase or bounded work package. A prompt such as `Work Issue #224` supplies the scope through that issue. Readiness and dependencies still apply to each task. Ask the owner to resolve ambiguous boundaries.

Select the effective profile independently of scope and environment:

| Profile | Authority |
| --- | --- |
| Owner-led | The owner drives execution, makes decisions and merges. Agents assist with specifically assigned work. |
| Supervised (default) | Agents implement and validate ready issues and open draft PRs. The owner controls progression, reviews and merges. |
| Delegated | An explicit active owner grant permits bounded implementation, independent review and progression. A separate trusted gate may merge only explicitly eligible PRs after verifying the final head and every required condition. |
| Autonomous | Reserved for a future owner-approved design covering planning and design delegation. Unavailable for dispatch. |

Use supervised execution unless the owner specifies owner-led execution or a current grant covers the task. Child tasks inherit only the applicable portion of the parent grant. Only the owner may override a profile or change a grant. Agents cannot enlarge scope, select higher authority or pass along authority they lack. Local workspaces, hosted workspaces, CI, available credentials and application runtime environments confer no authority.

For a delegated grant, record the effective scope, profile, eligible merges, reviewer and gate, activation and expiration in the existing GitHub issue or tracker. Ordinary supervised assignments need no separate manifest. Authority expires at completion, cancellation, the stated deadline or the scope boundary, whichever comes first. A pause suspends dispatch and merge until the owner explicitly resumes within a valid grant.

Before a delegated merge, require independent positive review of the final PR head, applicable validation and required GitHub checks, and compliance with repository protections and the grant. The implementation agent cannot approve or merge its own work. The gate operates outside the implementation and review processes. Protect its credentials from PR code and honor formal GitHub approval requirements without self-approval or bypass. The owner retains product, architecture and phase or sprint acceptance decisions.

Apply [validation gates](../workflow/validation-gates.md) and [stop conditions](../workflow/stop-conditions.md) under every profile. Escalate consequential decisions and unresolved blockers to the owner. Record material changes, decisions, deviations, deferrals, evidence and revisit triggers in existing PRs and the tracker closeout when applicable.

The supervised profile remains active for the [#224 setup PR](https://github.com/bbubb/squadsync/issues/224) and project work without a valid grant. The [Sprint 12 pilot design](../tools/codex-cli/sprint-12-delegated-pilot.md) describes the first proposed instance and its activation gates.

## Scope Boundaries

Agents must keep work aligned with the documented soccer MVP.

Agents must not:

- add unrelated product areas
- implement lineup optimization inside the SquadSync core platform
- add unapproved service responsibilities
- add new architectural patterns without an ADR
- implement from an issue whose scope conflicts with current canonical documentation

## Tool Boundaries

Formal workflow roles:

- ChatGPT + human owner: planning, architecture, issue creation, PR review support.
- GitHub: canonical workflow and project record.
- Codex CLI: primary implementation agent for scoped issues.

Local editor assistants may be used by a human contributor, but they are not part of the canonical workflow and must not replace issue scope, validation gates, or human review.

Additional formal tools may be added later under `docs/agentic-workflow/tools/` when they have a defined operational role.

## Review Standard

Every completed task should be understandable to:

- the human owner
- a future implementation agent
- a technical reviewer
- a hiring manager reviewing the project as a portfolio artifact
