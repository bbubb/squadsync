# Agentic Workflow Architecture

## Purpose

This directory defines SquadSync's repo-owned agentic workflow architecture.

The goal is to make AI-assisted development durable, reviewable, and repeatable by keeping the core workflow in repository files instead of hidden chat session context.

## Structure

```text
docs/agentic-workflow/
  policy/      Durable project rules and boundaries
  workflow/    Work lifecycle, engineering standards, validation gates, and stop conditions
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

## Phase 0 Standard

Phase 0 is complete only when future work can be driven from repository-owned policy, workflow, specs, validation gates, engineering standards, and tool profiles.

Phase 1 application code should not begin until:

- ChatGPT GitHub and Codex CLI profiles are usable for their scoped roles
- Codex CLI has rules, skills, hooks, and subagent placeholders in its tool profile
- branching, testing, and coding standards are documented
- Phase 1 issues can be executed from repository-owned context without relying on hidden chat state
