# Agentic Workflow Architecture

## Purpose

This directory defines SquadSync's repo-owned agentic workflow architecture.

The goal is to make AI-assisted development durable, reviewable, and repeatable by keeping the core workflow in repository files instead of hidden chat session context.

## Structure

```text
docs/agentic-workflow/
  policy/      Durable project rules and boundaries
  workflow/    Work lifecycle, engineering standards, documentation governance, validation gates, and stop conditions
  specs/       Standard issue and pull request contracts
  tools/       Tool-specific operational profiles
  evolution/   Harness friction, lessons learned, and future improvements
```

## Design Principle

The policy, workflow, and spec layers are modular and mostly tool-agnostic.

Tool-specific behavior belongs under `tools/`.

This lets SquadSync keep stable project rules while supporting ChatGPT GitHub for planning/workflow tasks, Codex CLI for implementation tasks, and future orchestrators such as Symphony later.

## Current Tool Roles

- ChatGPT GitHub: planning, issue generation, documentation edits, PR setup, PR review support, and closeout summaries.
- GitHub: canonical source of truth for docs, issues, pull requests, ADRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.

Local editor assistants may be used by a human contributor, but they are not part of the canonical workflow.

## Core Workflow Docs

- [Lifecycle](workflow/lifecycle.md)
- [Context Management](workflow/context-management.md)
- [Prompting Standards](workflow/prompting-standards.md)
- [Branching Strategy](workflow/branching-strategy.md)
- [Testing Strategy](workflow/testing-strategy.md)
- [Coding Standards](workflow/coding-standards.md)
- [Documentation Standards](workflow/documentation-standards.md)
- [Root Summary Sync](workflow/root-summary-sync.md)
- [Spec Consistency](workflow/spec-consistency.md)
- [Issue Orchestration](workflow/issue-orchestration.md)
- [Validation Gates](workflow/validation-gates.md)
- [Stop Conditions](workflow/stop-conditions.md)

## Tool Profiles

- [ChatGPT GitHub](tools/chatgpt-github/README.md)
- [Codex CLI](tools/codex-cli/README.md)
- [Symphony](tools/symphony/README.md)

## Phase 0 Foundation

Phase 0 established the repository-owned policy, workflow, specifications, validation gates, documentation governance, engineering standards, scoped agent guidance, and tool profiles needed to begin implementation without hidden chat context.

For a historical orientation to that foundation, see the [Phase 0 Closeout Guide](../planning/phase-0-closeout-guide.md).

Current phase, sprint direction, and the next planning action belong in the [Project Roadmap](../planning/project-roadmap.md). Executable work status belongs in GitHub issues, Project, and pull requests. Future workflow changes should be driven by observed implementation friction rather than speculative harness expansion.
