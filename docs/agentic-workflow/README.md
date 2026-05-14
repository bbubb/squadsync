# Agentic Workflow Architecture

## Purpose

This directory defines SquadSync's repo-owned agentic workflow architecture.

The goal is to make AI-assisted development durable, reviewable, and repeatable by keeping the core workflow in repository files instead of hidden chat session context.

## Structure

```text
docs/agentic-workflow/
  policy/      Durable project rules and boundaries
  workflow/    Work lifecycle, validation gates, and stop conditions
  specs/       Standard issue and pull request contracts
  tools/       Tool-specific operational profiles
  evolution/   Harness friction, lessons learned, and future improvements
```

## Design Principle

The policy, workflow, and spec layers are modular and mostly tool-agnostic.

Tool-specific behavior belongs under `tools/`.

This lets SquadSync keep stable project rules while supporting Codex CLI today and future orchestrators such as Symphony later.

## Current Tool Roles

- ChatGPT + human owner: architecture, planning, issue creation, ADR drafting, PR review support, and workflow refinement.
- GitHub: canonical source of truth for docs, issues, pull requests, ADRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.

Local editor assistants may be used by a human contributor, but they are not part of the canonical workflow.

## Phase 0 Standard

Phase 0 is complete only when future implementation work can be driven from repository-owned policy, workflow, specs, validation gates, and the Codex CLI operational profile.

Phase 1 application code should not begin until the Codex CLI profile is usable for a scoped implementation task.
