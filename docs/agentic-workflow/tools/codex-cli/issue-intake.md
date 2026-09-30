# Codex CLI Issue Intake

## Purpose

This document defines how Codex CLI should evaluate a GitHub issue before beginning work.

## Intake Checklist

Retrieve the issue before loading broad task context. Prefer authenticated repository-aware GitHub tooling. For a CLI fallback, pass the repository explicitly:

```powershell
gh issue view <number> --repo bbubb/squadsync
```

Do not use public web search as the normal source for canonical issue state. See [Context Loading](context-loading.md) for the complete sequence and fallback behavior.

Read `CONTRIBUTING.md` before implementation so repository-wide issue, branch, PR, and validation rules remain part of every implementation task.

Before implementation, Codex should confirm:

- the issue has a clear objective
- the issue has enough context
- scope is narrow enough for one PR
- non-goals are explicit
- governing docs/ADRs are named by exact path or can be resolved through repository routing before any change
- acceptance criteria are reviewable
- validation steps are defined
- stop conditions are understood

Read only the additional project, architecture, and workflow sources needed to resolve those checks, following root `AGENTS.md` and the task's affected paths.

## Agent-Ready Decision

The `agent-ready` label is a routing signal, not proof that the current issue body is executable. Re-evaluate the issue against the current repository every time work begins.

An issue is ready for Codex when it can be implemented without:

- inventing product scope
- making an unapproved architecture decision
- guessing validation expectations
- changing unrelated files
- depending on private chat context

If an issue is labeled `agent-ready` but fails this check, treat it as not ready and report the stale label with the blocking discrepancy.

## If the Issue Is Not Ready

Codex should not implement the task.

Instead, Codex should report:

```markdown
## Issue Not Ready

Missing or unclear information:

Suggested clarification:

Suggested follow-up issue or ADR:
```

## Scope Control

Codex should prefer one narrow PR over one broad PR.

If a task naturally splits into multiple changes, Codex should implement only the requested scope and suggest follow-up issues.
