# Codex CLI Issue Intake

## Purpose

This document defines how Codex CLI should evaluate a GitHub issue before beginning work.

## Intake Checklist

Before implementation, Codex should confirm:

- the issue has a clear objective
- the issue has enough context
- scope is narrow enough for one PR
- non-goals are explicit
- relevant docs/ADRs are named or discoverable
- acceptance criteria are reviewable
- validation steps are defined
- stop conditions are understood

## Agent-Ready Decision

An issue is ready for Codex when it can be implemented without:

- inventing product scope
- making an unapproved architecture decision
- guessing validation expectations
- changing unrelated files
- depending on private chat context

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
