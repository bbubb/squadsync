---
name: squadsync-docs-maintenance
description: Use for SquadSync documentation updates, stale link cleanup, documentation headers, root summary sync, and documentation consistency checks.
---

# SquadSync Docs Maintenance

Use this skill for documentation-focused tasks.

## Required Context

Read first:

- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/agentic-workflow/workflow/documentation-standards.md`
- `docs/agentic-workflow/workflow/root-summary-sync.md`
- `docs/agentic-workflow/workflow/spec-consistency.md`
- `docs/agentic-workflow/tools/codex-cli/skills/update-docs.md`

## Process

1. Identify the main reference document before editing.
2. Check whether README, AGENTS, roadmap, or system overview need matching updates.
3. Update document headers for meaningful changes to major docs.
4. Remove stale references when paths or terms change.
5. Link to canonical docs instead of duplicating long sections.
6. Report validation performed in the PR.

## Stop Conditions

Stop if docs conflict and the correct reference is unclear, or if the change would alter product scope, architecture, or workflow governance without approval.
