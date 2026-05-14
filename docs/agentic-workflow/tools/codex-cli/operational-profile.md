# Codex CLI Operational Profile

## Purpose

This profile defines how Codex CLI should operate in SquadSync.

It translates the generic agentic workflow architecture into concrete Codex CLI behavior.

## Operating Rules

Codex CLI must:

- work from GitHub issues and repository docs
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

## Generic Concept Mapping

| Generic concept | Codex CLI expression |
|---|---|
| Project policy | `AGENTS.md` and `docs/agentic-workflow/policy/` |
| Task specification | GitHub issue and `docs/agentic-workflow/specs/agent-task-spec.md` |
| Workflow lifecycle | `docs/agentic-workflow/workflow/lifecycle.md` |
| Rules | This profile plus `AGENTS.md` and policy docs |
| Task playbooks / skills | `task-prompt.md` and issue-specific instructions |
| Hooks | Validation gates now; executable scripts later if needed |
| Stop conditions | `docs/agentic-workflow/workflow/stop-conditions.md` |
| PR contract | `pr-reporting.md` and `.github/PULL_REQUEST_TEMPLATE.md` |

## Branching

Use focused branch names such as:

```text
feature/backend-scaffold
fix/lineup-slot-validation
docs/phase-0-agentic-workflow-harness
```

## Completion Standard

A Codex CLI task is complete when:

- the issue acceptance criteria are addressed
- relevant validation has been run or limitations documented
- docs/ADRs are updated if needed
- scope drift has been avoided
- a PR summary can be written from repo-visible facts
