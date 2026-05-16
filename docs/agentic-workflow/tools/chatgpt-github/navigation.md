# ChatGPT GitHub Navigation

## Purpose

This document defines how ChatGPT should navigate SquadSync when used with the GitHub Connector.

The goal is to reduce reliance on pasted prompts by giving ChatGPT a repository-owned routing model for common project interactions.

## Starting Rule

When entering a SquadSync discussion, ChatGPT should first identify the user’s intent:

```text
orientation | planning | issue creation | PR review | documentation edit | implementation support | closeout | ad hoc project question
```

Then route to the appropriate workflow document and source-of-truth docs.

## Required First Reads

For most project work, start with:

- `AGENTS.md`
- `README.md`
- `docs/planning/project-roadmap.md`
- `docs/agentic-workflow/README.md`
- `docs/agentic-workflow/tools/chatgpt-github/README.md`

For architecture/product questions, also read:

- `docs/planning/mvp-scope.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- relevant ADRs in `docs/adr/`

## Routing Table

| User intent | Route to | Output |
|---|---|---|
| Orientation / “where are we?” | `main-thread-workflow.md` | Current state summary and next action |
| Phase or sprint planning | `main-thread-workflow.md` | Sprint plan, issues, non-goals, validation |
| Focused sprint or issue execution | `branch-thread-workflow.md` | Focused execution/review plan |
| GitHub issue creation | `agent-task-spec.md` and `main-thread-workflow.md` | Issue(s) with scope and acceptance criteria |
| PR review | `pull-request-spec.md` and relevant issue | Review summary or requested changes |
| Documentation edit | `documentation-standards.md` and `spec-consistency.md` | Scoped docs change with sync checks |
| Implementation support | Codex skill/profile docs | Codex-ready prompt or implementation guidance |
| Closeout | `branch-thread-workflow.md` closeout template | Closeout summary and follow-ups |
| Ad hoc project question | Relevant canonical docs | Concise answer with source-of-truth caveats |

## Main Thread vs Branch Thread

Use the main thread for:

- project control-room work
- phase/sprint planning
- issue sequencing
- roadmap decisions
- PR closeout review

Use branch threads for:

- focused sprint execution
- one issue/PR review
- debugging a bounded problem
- creating a Codex-ready implementation prompt

## Repo Change Rule

When ChatGPT GitHub creates repository changes, use:

```text
issue -> branch -> scoped changes -> PR -> human review
```

If a PR is created before an issue by mistake, create/link the issue before review.

## Stop Conditions

Stop and request clarification when:

- user intent is unclear
- source-of-truth docs conflict
- the request would change architecture without an ADR
- the request would bypass issue-backed workflow
- the request requires app code when the scope is docs-only
- the request requires a tool capability not available through the connector
