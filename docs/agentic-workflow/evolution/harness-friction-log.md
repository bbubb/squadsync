# Harness Friction Log

## Purpose

This log captures friction discovered while using SquadSync's agentic workflow.

The goal is to improve the harness based on real usage instead of prematurely adding scripts, hooks, or tool abstractions.

## When to Add an Entry

Add an entry when:

- Codex CLI repeatedly misses context
- issue templates are unclear
- validation steps are repetitive or error-prone
- PR summaries lack needed information
- stop conditions are missing
- tool-specific behavior causes confusion
- a manual step should become a script or hook

## Entry Template

```markdown
## YYYY-MM-DD - Short title

### Context

What task or sprint exposed the friction?

### Friction

What was confusing, repetitive, brittle, or error-prone?

### Impact

How did it affect implementation, review, cost, or confidence?

### Proposed Change

What workflow, doc, script, hook, or template change might help?

### Status

Open / Accepted / Deferred / Closed
```

## Initial Notes

- Do not add executable hooks until repeated friction proves they are useful.
- Keep the Codex CLI profile as the first operational tool profile.
- Treat Symphony as a future orchestration profile until Codex CLI has completed at least one implementation sprint.

## 2026-09-28 - Retrieve the active issue before task context

### Context

The first Sprint 2 Codex CLI runs for #58 and the initial attempt at #59 exercised the documented implementation workflow.

### Friction

Codex loaded broad repository and workflow context before reliably retrieving the active issue. In the elevated Windows sandbox, `gh issue view <number>` also depended on Git repository discovery and encountered safe-directory checks, leading to less direct fallback paths.

### Impact

The task started with unnecessary context, and canonical issue retrieval was slower and less predictable.

### Proposed Change

Retrieve the issue immediately with authenticated repository-aware GitHub tooling; if using GitHub CLI, pass `--repo bbubb/squadsync`. Treat root `AGENTS.md` as the router, retain `CONTRIBUTING.md` as the small repository-wide implementation baseline, and then load only documents selected by issue readiness and affected paths. Keep `README.md` available as orientation context when needed rather than forcing a reread on every task. Preserve all readiness, validation, draft PR, and human merge safeguards.

### Status

Accepted in PR #65. Codex context loading, issue intake, task prompt, implementation playbook, and generic context-order guidance follow this sequence.
