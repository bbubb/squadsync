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

## Decision and Deferral Handoff

Capture material architectural constraints, UX intent, risks, and design questions **when they surface** in planning, implementation, mentoring, or PR review. Do not wait for sprint closeout or rely on chat memory.

1. **Classify:** distinguish current behavior, accepted decisions, proposals needing owner approval, deferred work, and intentionally private exploration. An idea mentioned in chat is not automatically an approved requirement.
2. **Choose one durable home:** accepted architecture or UX guidance belongs in its relevant canonical doc/ADR; actionable work or unresolved design gates belong in GitHub issues; local implementation choices belong in PRs. Keep private, potentially proprietary concepts in an access-controlled private reference, not the public repo.
3. **Preserve the consequence:** for a material deferral record its rationale, what must remain true until then, the decision or implementation **trigger**, and the issue/phase responsible for reconsidering it.
4. **Link the consumer:** reference the constraint or gate from the future issue that could violate it. A note only in an old sprint tracker or closed PR is insufficient. If the downstream issue does not exist yet, link it when planning that work.
5. **Reconcile before handoff:** during issue readiness and PR review, check whether new material decisions or risks have been linked to their canonical home and downstream consumer. Sprint closeout checks for omissions; it is not the first capture point.

Example: defer a production-scale Person search contract during a local Team screen, but record the server-bounded, authorization-sensitive query requirement and link it to the later **Select Existing Person** readiness gate. Do not let that later implementation default to loading all People.

Avoid logging passing ideas as mandatory issues, copying whole conversations, exposing private designs, or creating speculative ADRs.

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

For Codex issue execution, read the root `AGENTS.md` as the landing/router document, retrieve the active issue immediately, then read `CONTRIBUTING.md` for repository-wide implementation rules. Check readiness next, then resolve the scoped instructions and source documents from the issue and affected paths. The Codex [context-loading guide](../tools/codex-cli/context-loading.md) defines retrieval and fallback details.

`README.md` remains the public orientation/current-state document and should be read when that context is needed; it is not a mandatory reread for every already well-scoped issue.

For other human or agent work, begin with the source that identifies the active task (issue, PR, or requested change), then use `AGENTS.md` and repository indexes to resolve the relevant context. Read `PLANS.md` when the work meets its complexity threshold. Do not treat linked indexes as mandates to load every document.

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
