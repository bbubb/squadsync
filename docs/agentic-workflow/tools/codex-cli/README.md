# Codex CLI Tool Profile

## Purpose

This directory defines the Codex CLI operational profile for SquadSync.

Codex CLI is the first supported implementation tool for the agentic workflow architecture.

## Role

Codex CLI should implement scoped, `agent-ready` GitHub issues by reading repository-owned policy, workflow, specs, rules, skills, hooks, and relevant architecture docs.

Codex CLI should not depend on private ChatGPT session context to understand what to do.

## Profile Files

- `operational-profile.md` — how Codex CLI fits into the SquadSync workflow.
- `context-loading.md` — required context loading order before work begins.
- `issue-intake.md` — how Codex should interpret and validate issue readiness.
- `task-prompt.md` — reusable task prompt pattern for Codex CLI.
- `validation.md` — Codex-specific validation reporting expectations.
- `pr-reporting.md` — pull request summary and review expectations.

## Codex-Native Structure

- `rules/` — persistent Codex behavior constraints.
- `skills/` — reusable Codex task playbooks.
- `hooks/` — lifecycle automation points and planned validation hooks.
- `subagents/` — planned specialized roles for future orchestration.

## Relationship to Generic Workflow

Generic concepts live in:

- `docs/agentic-workflow/policy/`
- `docs/agentic-workflow/workflow/`
- `docs/agentic-workflow/specs/`

This profile maps those concepts into Codex CLI usage.

## Non-Goals

This profile does not:

- implement application code
- define Claude/Cursor behavior
- create a universal adapter layer
- automate merges
- replace human architectural review
