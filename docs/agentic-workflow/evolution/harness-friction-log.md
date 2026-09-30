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

## 2026-09-30 - Scoped context directives still preload broad bundles

### Context

The first follow-on API and infrastructure implementation runs after #65, including Issue #59, tested the issue-first context-loading sequence against repository-wide contribution guidance, path-scoped `AGENTS.md` files, and active native skills.

### Friction

Although the Codex context-loading profile described a minimal baseline and progressive disclosure, `CONTRIBUTING.md` and scoped API, web, infrastructure, and documentation guidance still listed broad document bundles as required before work. The API and documentation skills repeated similar unconditional lists. During #59, those directives led to reading roadmap, MVP, architecture/domain, broad workflow, and AWS context before or around issue-specific routing, even where the task was limited to narrower Docker and validation sources.

### Impact

The older directives defeated the issue-first model in actual use, adding unrelated context and making the intended task-specific source selection unclear. The workflow could not reliably keep simple infrastructure work focused or prepare #60 as a clean API context-loading validation run.

### Proposed Change

Keep the active issue, root `AGENTS.md`, and `CONTRIBUTING.md` as the universal baseline. Keep essential local rules in the nearest `AGENTS.md` and applicable skill, and route deeper sources conditionally based on issue scope, affected paths, and concrete dependencies, ambiguity, conflicts, or architecture questions. Preserve validation, stop conditions, human merge authority, and `PLANS.md` for genuinely complex work.

### Status

Accepted in Issue #68; scoped directives and active skills are reconciled with progressive context loading.
