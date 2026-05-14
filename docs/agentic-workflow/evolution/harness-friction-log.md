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
