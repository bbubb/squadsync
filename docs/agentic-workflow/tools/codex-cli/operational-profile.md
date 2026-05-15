# Codex CLI Operational Profile

## Purpose

This profile defines how Codex CLI operates in SquadSync.

It maps SquadSync's generic agentic workflow architecture into Codex-native concepts: rules, skills, hooks, subagents, and AGENTS.md.

## Operating Model

```text
Generic workflow layer -> Codex CLI profile -> issue-scoped implementation -> validation -> PR review
```

## Operating Rules

Codex CLI must:

- work from GitHub issues and repository docs
- read `AGENTS.md` before making changes
- keep each task scoped to one focused change
- preserve documented architecture boundaries
- update docs when behavior, architecture, or workflow changes
- run available validation gates before reporting completion
- document validation limitations honestly
- stop instead of guessing when scope or architecture is unclear

Codex CLI must not:

- add unrequested product scope
- implement soccer-subber optimization logic inside SquadSync core
- add new service responsibilities without an ADR
- begin broad refactors outside issue scope
- invent new architecture without approval
- auto-merge its own work

## Generic-to-Codex Mapping

| Generic concept | Codex CLI expression |
|---|---|
| Root operating map | `AGENTS.md` |
| Project policy | `docs/agentic-workflow/policy/` and `rules/` |
| Task specification | GitHub issue and `docs/agentic-workflow/specs/agent-task-spec.md` |
| Workflow lifecycle | `docs/agentic-workflow/workflow/lifecycle.md` and `rules/workflow-rules.md` |
| Rules | `rules/` |
| Skills / task playbooks | `skills/` |
| Hooks / lifecycle automation points | `hooks/` |
| Subagents / specialized roles | `subagents/` |
| Stop conditions | `docs/agentic-workflow/workflow/stop-conditions.md` |
| PR contract | `pr-reporting.md` and `.github/PULL_REQUEST_TEMPLATE.md` |

## Branching

Follow `docs/agentic-workflow/workflow/branching-strategy.md`.

Use focused branch names such as:

```text
feature/backend-scaffold
fix/lineup-slot-validation
docs/finalize-phase-0-standards
```

## Testing

Follow `docs/agentic-workflow/workflow/testing-strategy.md`.

Behavior changes should normally define or update tests before or alongside implementation.

## Completion Standard

A Codex CLI task is complete when:

- issue acceptance criteria are addressed
- relevant validation has been run or limitations documented
- tests are added/updated when behavior changes
- docs/ADRs are updated if needed
- scope drift has been avoided
- a PR summary can be written from repo-visible facts
