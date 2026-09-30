---
name: squadsync-api-task
description: Use for SquadSync API/backend implementation tasks under apps/api, including scaffold work, domain/application/API changes, tests, and validation.
---

# SquadSync API Task

Use this skill for scoped API/backend work in `apps/api/`.

## Context Routing

Use the active issue, root `AGENTS.md`, and `CONTRIBUTING.md` as the universal baseline, then read `apps/api/AGENTS.md` for local rules. Read `apps/api/README.md` when setup or current scaffold details matter. Load roadmap/MVP, system/domain architecture, coding/testing standards, and validation guidance when selected by the issue, affected behavior, or a concrete implementation question; consult applicable ADRs for potentially decided design choices. Read `docs/agentic-workflow/tools/codex-cli/skills/scaffold-backend.md` for initial API scaffold work. These are routed sources, not a mandatory bundle for every API task.

Follow references or investigate deeper context when a dependency, ambiguity, conflict, or architecture concern emerges. Read `PLANS.md` and use an ExecPlan when the task is complex, cross-cutting, risky, or multi-step.

## Process

1. Confirm the GitHub issue scope, non-goals, acceptance criteria, and validation.
2. Identify expected tests before or alongside behavior changes.
3. Preserve Clean Architecture boundaries.
4. Keep changes inside `apps/api/` unless docs or workflow updates are explicitly needed.
5. Run validation or document why it cannot be run.
6. Prepare PR notes with validation evidence and follow-ups.

## Stop Conditions

Stop if the issue is not ready, targets a location outside canonical `apps/api/` scope, conflicts with current docs, or requires product scope changes, service boundary changes, unapproved infrastructure dependencies, or app architecture changes without an ADR.
