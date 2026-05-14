# Project Policy

## Purpose

This policy defines the durable rules that all human and AI contributors must follow when working on SquadSync.

It does not replace `AGENTS.md`. Instead, it organizes the workflow-specific policy used by agentic planning and implementation.

## Source of Truth

Before implementation work begins, agents should read:

- `AGENTS.md`
- `README.md`
- `docs/planning/project-roadmap.md`
- `docs/planning/mvp-scope.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- relevant ADRs in `docs/adr/`
- relevant files under `docs/agentic-workflow/`

## Human-Owned Architecture

The human owner retains authority over:

- product scope
- architecture direction
- ADR approval
- merge decisions
- toolchain changes
- agent workflow changes

Agents may recommend changes, draft docs, create issues, implement scoped work, and summarize trade-offs. Agents should not silently expand architecture or scope.

## Scope Boundaries

Agents must keep work aligned with the documented soccer MVP.

Agents must not:

- add unrelated product areas
- implement lineup optimization inside the SquadSync core platform
- add unapproved service responsibilities
- add new architectural patterns without an ADR
- begin Phase 1 implementation before Phase 0 operational harness hardening is complete

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
