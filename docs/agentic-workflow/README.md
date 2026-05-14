# Agentic Workflow Architecture

## Purpose

This directory defines SquadSync's repo-owned agentic workflow architecture.

The goal is to make AI-assisted development durable, reviewable, and repeatable by keeping the core workflow in repository files instead of hidden chat session context.

## Architecture

```text
docs/agentic-workflow/
  policy/      Durable project rules and boundaries
  workflow/    How work moves from planning to implementation to review
  specs/       Standard shapes for issues, pull requests, and task handoffs
  tools/       Tool-specific operational profiles
  evolution/   Harness friction, lessons learned, and future improvements
```

## Design Principle

The generic layers are intentionally modular and mostly tool-agnostic.

Tool-specific behavior belongs under `tools/`.

This lets SquadSync keep a stable policy/spec/workflow layer while supporting concrete tools such as Codex CLI today and possible orchestrators such as Symphony later.

## Current Tool Roles

- ChatGPT + human owner: architecture, planning, ADR drafting, issue creation, PR review support, and workflow refinement.
- GitHub: canonical source of truth for docs, issues, pull requests, ADRs, and workflow state.
- Codex CLI: primary implementation agent for scoped `agent-ready` issues.
- GitHub Copilot / GHCP: optional local/editor assistance only. It is not required for the formal workflow.

## Phase 0 Standard

Phase 0 is complete only when future implementation work can be driven from repository-owned policy, workflow, specs, validation gates, and tool profiles.

Phase 1 application code should not begin until the Codex CLI profile is usable for a scoped implementation task.
