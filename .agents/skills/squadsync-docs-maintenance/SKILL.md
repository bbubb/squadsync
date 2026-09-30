---
name: squadsync-docs-maintenance
description: Use for SquadSync documentation updates, stale link cleanup, documentation headers, root summary sync, and documentation consistency checks.
---

# SquadSync Docs Maintenance

Use this skill for documentation-focused tasks.

## Context Routing

Use the active issue or approved documentation task, root `AGENTS.md`, and `CONTRIBUTING.md` as the universal baseline, then read `docs/AGENTS.md` for local rules. Apply `documentation-standards.md` when changing canonical docs; consult `root-summary-sync.md` and `spec-consistency.md` when summaries, derivative docs, or source alignment are in scope. Load other planning, product, architecture, policy, or workflow sources when selected by the issue or needed to resolve a concrete dependency, ambiguity, conflict, or consistency question. Read `docs/agentic-workflow/tools/codex-cli/skills/update-docs.md` for the documentation update flow. This skill is not a mandatory preload bundle.

Read `PLANS.md` and use an ExecPlan when the task is complex, cross-cutting, risky, or multi-step.

## Process

1. Identify the main reference document before editing.
2. Check whether README, AGENTS, roadmap, or system overview need matching updates.
3. Update a document's status or existing metadata when the change materially affects it; do not add full metadata solely for uniformity.
4. Remove stale references when paths or terms change.
5. Link to canonical docs instead of duplicating long sections.
6. Report validation performed in the PR.

## Stop Conditions

Stop if docs conflict and the correct reference is unclear, or if the change would alter product scope, architecture, or workflow governance without approval.
