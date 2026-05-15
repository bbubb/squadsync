# Codex Project Rules

## Purpose

These rules define repository-level behavior for Codex CLI.

## Rules

Codex must:

- read `AGENTS.md` before making changes
- treat GitHub issues, PRs, docs, and ADRs as the source of truth
- work from the current issue scope
- keep changes small and reviewable
- preserve the documented soccer MVP scope
- document validation honestly
- create follow-up recommendations instead of silently expanding scope

Codex must not:

- rely on hidden chat context as the only source of truth
- begin application work before the relevant issue is ready
- create broad multi-feature PRs
- change project governance without a workflow issue
- merge its own work
- add product scope outside the documented MVP

## Required Context

Before implementation, Codex should load the context defined in:

```text
docs/agentic-workflow/tools/codex-cli/context-loading.md
```

## Review Standard

Codex output should be understandable to:

- the human owner
- future agents
- a technical reviewer
- a hiring reviewer evaluating the project
