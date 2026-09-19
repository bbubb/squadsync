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

- `README.md`
- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/planning/project-roadmap.md`
- `docs/agentic-workflow/README.md`
- `docs/agentic-workflow/tools/chatgpt-github/README.md`
- the active issue or pull request when the work is bounded

For architecture/product questions, also read:

- `docs/planning/mvp-scope.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- the specific ADRs governing the question, selected from [docs/adr/](../../../adr/)

## Canonical Thread Starts

- Use the [Standard Main Thread Prompt](main-thread-workflow.md#standard-main-thread-prompt) for orientation, roadmap work, phase transitions, and sprint planning.
- Use the [Standard Branch Thread Prompt](branch-thread-workflow.md#standard-branch-thread-prompt) only after a sprint or task is confirmed.
- Derive current phase, sprint, issue, and PR state from GitHub during startup. Do not encode those values into the stable prompt templates.

## Routing Table

| User intent | Route to | Output |
|---|---|---|
| Orientation / “where are we?” | [Main thread workflow](main-thread-workflow.md) | Current state summary and next action |
| Phase or sprint planning | [Main thread workflow](main-thread-workflow.md) | Sprint plan, issues, non-goals, validation |
| Focused sprint or issue execution | [Branch thread workflow](branch-thread-workflow.md) | Focused execution/review plan |
| GitHub issue creation | [Issue orchestration](../../workflow/issue-orchestration.md), the applicable issue template/spec, and [main thread workflow](main-thread-workflow.md) | Issue(s) with scope and acceptance criteria; `agent-ready` only after review |
| PR review | [PR review prompt](../../../prompt-library/pr-review.prompt.md), [pull request spec](../../specs/pull-request-spec.md), and relevant issue | Review summary or requested changes |
| Documentation edit | [Documentation standards](../../workflow/documentation-standards.md) and [spec consistency](../../workflow/spec-consistency.md) | Scoped docs change with sync checks |
| Implementation support | [Codex task prompt](../codex-cli/task-prompt.md), [Codex CLI profile](../codex-cli/README.md), and the task-relevant [repo-native skill](../../../../.agents/README.md) | Codex-ready prompt or implementation guidance |
| Closeout | [Branch-thread closeout template](branch-thread-workflow.md#closeout-summary-template) | Closeout summary and follow-ups |
| Ad hoc project question | [Required first reads](#required-first-reads), then relevant canonical docs | Concise answer with source-of-truth caveats |

Open the linked route target before acting, then follow its explicit references and the nearest scoped `AGENTS.md`. If a required task-specific source cannot be named by exact repository path, stop and ask for clarification instead of guessing.

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
