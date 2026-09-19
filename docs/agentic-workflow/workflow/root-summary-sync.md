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

## State Ownership and Update Cadence

| Information | Canonical home | Update when |
|---|---|---|
| Active phase and next planning action | `docs/planning/project-roadmap.md` | A phase or sprint transition is accepted |
| Executable task status | GitHub Issues, Project, and pull requests | Work is created, refined, blocked, reviewed, or completed |
| Public milestone summary | `README.md` | A meaningful public-facing milestone changes |
| Area status and operating guidance | Nearest area `README.md` and `AGENTS.md` | The area becomes active or its paths, commands, or rules change |
| Product and architecture truth | MVP, architecture, domain, and ADR docs | An accepted product or architecture decision changes |
| Workflow invariants | Generic policy, workflow, and spec docs | The project-wide process or governance rule changes |
| Current tool assignments and behavior | Agentic-workflow index and tool profiles | A supported tool or its role changes |
| Historical milestone record | Closeout guides and accepted ADRs | A historical error needs correction, not when current work advances |

Do not copy routine phase, sprint, or issue status into generic workflow documents or historical closeout guides. Link to the canonical current-state source instead.

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
