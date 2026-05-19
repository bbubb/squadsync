# Context Management

## Purpose

This document defines how SquadSync keeps ChatGPT, Codex CLI, GitHub issues, PRs, and project docs aligned without relying on hidden chat memory.

## Core Rule

The repository is the canonical source of truth. Chat threads are working context, not durable project memory.

When a decision, plan, task, or constraint must survive the current session, capture it in one of:

- a GitHub issue;
- a pull request description;
- an ADR;
- a planning document;
- an agentic workflow document;
- `PLANS.md` or a task-specific ExecPlan when the work is complex.

## Thread Types

### Main Planning Thread

Use for:

- roadmap direction;
- phase transitions;
- sprint planning;
- cross-cutting architecture questions;
- reviewing whether follow-up work should become issues.

### Branch Thread

Use for:

- one confirmed sprint;
- one issue;
- one PR;
- one debugging or review task;
- closeout summaries.

Branch threads should not redefine roadmap direction. If broader work appears, record it as a follow-up and return it to the main planning thread.

### Codex Session

Use for:

- implementing a scoped issue;
- running validation;
- iterating on code or docs within branch scope.

Codex sessions should start from repo instructions, issue scope, and relevant skills, not from hidden ChatGPT context.

## Context Loading Order

Default order for human or agent work:

1. `README.md`
2. `AGENTS.md`
3. `CONTRIBUTING.md`
4. `PLANS.md` if the work is complex
5. active issue or PR
6. relevant planning, architecture, product, and workflow docs
7. relevant local `AGENTS.md` file, such as `apps/api/AGENTS.md`
8. relevant native skill under `.agents/skills/`

## When to Summarize or Close a Thread

Summarize and close out a thread when:

- the sprint, issue, or PR is complete;
- the thread is long enough that key decisions are hard to find;
- the discussion has branched into multiple topics;
- the next step requires a new issue or PR.

The summary should include:

- what changed;
- what was decided;
- what remains open;
- links to issues and PRs;
- validation status;
- follow-up issues created or needed.

## When to Use an ExecPlan

Use `PLANS.md` when the task is:

- complex;
- multi-hour;
- cross-cutting;
- risky;
- unclear enough that a self-contained plan is safer than a normal issue.

Do not use an ExecPlan for small docs edits, routine issue cleanup, or one-file fixes.

## Anti-Patterns

Avoid:

- relying on ChatGPT memory instead of repo files;
- creating PRs without issues;
- adding new docs without linking them from a discoverable index;
- repeating the same project summary in many places;
- leaving placeholder docs unlabeled;
- letting branch threads expand scope without a follow-up issue.
