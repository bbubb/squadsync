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

## 2026-09-30 - Docker-backed smoke validation repeatedly falls back to the human

### Context

Sprint 2 Issues #59–#61 exercised the local PostgreSQL Compose service and API readiness boundary.

### Friction

Codex could run .NET validation and detect Docker Compose, but its Windows sandbox could not reliably access Docker Desktop's user configuration. The full database-backed readiness smoke check therefore required manual human execution even after the workflow was already proven in #60.

### Impact

The validation itself is valuable, but repeating a multi-terminal manual sequence is slow, error-prone, and unnecessary for future work.

### Proposed Change

Create one repository-owned local smoke-validation command that starts PostgreSQL and the API, verifies the expected 200/200 -> 200/503 -> 200 readiness sequence, cleans up non-destructively, and exits nonzero on failure. Keep ordinary `dotnet test` Docker-independent.

Tracked in [Issue #72](https://github.com/bbubb/squadsync/issues/72).

### Status

Open.

## 2026-09-30 - Root AGENTS routing still triggers broad startup loading

### Context

Issues #60 and #61 were executed after #65/#66 and #68 had introduced issue-first progressive context loading.

### Friction

Codex still began the task by reading README, CONTRIBUTING, PLANS, roadmap, MVP, and multiple workflow documents before retrieving the active issue. Root `AGENTS.md` says to load only relevant sources, but its long "Before making changes" catalog remains easy to interpret as a preload checklist.

### Impact

The implementation agent still performs unnecessary project archaeology before it knows the task, reducing signal-to-noise and weakening the intended issue-first routing model.

### Proposed Change

Make root `AGENTS.md` explicitly establish the universal startup order and recast its document catalog as conditional routing/reference guidance without removing canonical sources or deeper-context escalation.

Tracked in [Issue #73](https://github.com/bbubb/squadsync/issues/73).

### Status

Open.

## 2026-09-30 - Codex lifecycle completion and post-merge issue state are not reliable enough

### Context

Sprint 2 exposed lifecycle misses across #68 and #61, plus repeated linked-issue closeout discrepancies after merged PRs.

### Friction

One Codex run stopped before commit/push/draft-PR handoff, and #61 edited files on `main` after Git ownership checks failed instead of stopping before modification. Separately, multiple squash-merged PRs contained closing keywords but their linked issues remained open even though repository auto-close was enabled.

### Impact

The repository can temporarily diverge from the intended issue -> branch -> validation -> draft PR -> human merge -> verified issue-close lifecycle, requiring manual correction and reducing confidence in durable workflow state.

### Proposed Change

Make branch/state verification a pre-edit invariant, make draft-PR creation part of the normal completion condition, and add an explicit post-merge check that the implementing issue actually closed rather than assuming closing keywords succeeded.

Tracked in [Issue #74](https://github.com/bbubb/squadsync/issues/74).

### Status

Open.

