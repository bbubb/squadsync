# Root Summary Sync

## Purpose

Root summary sync prevents high-level project docs from drifting apart.

The root README, AGENTS.md, roadmap, and system overview each serve different purposes, but they must agree on core project state, pathing, and workflow direction.

## Root Summary Documents

Treat these as root summary documents:

- `README.md`
- `AGENTS.md`
- `docs/planning/project-roadmap.md`
- `docs/architecture/system-overview.md`
- `docs/agentic-workflow/README.md`

## Sync Rule

When a change affects project structure, phase status, tool workflow, app pathing, architecture direction, or canonical source-of-truth behavior, check whether root summary docs need updates.

Do not duplicate details across all root docs. Instead:

- keep README concise and public-facing
- keep AGENTS operational for humans/agents
- keep roadmap canonical for phase/sprint direction
- keep system overview canonical for architecture structure
- keep agentic workflow README canonical for workflow structure

## Common Sync Triggers

Check root summaries when changing:

- app pathing, such as `apps/api` or `apps/web`
- phase/sprint status
- agent/tool profiles
- architecture boundaries
- integration boundaries
- validation or branching standards
- root navigation links

## Agent Guidance

Agents should:

- identify affected root summary docs before editing
- update only the summaries that need the change
- link to canonical docs instead of copying large sections
- stop if two root summaries conflict and the source of truth is unclear

## Stop Condition

If root summaries disagree about phase, pathing, or architecture direction, pause work and reconcile the docs before implementation continues.
