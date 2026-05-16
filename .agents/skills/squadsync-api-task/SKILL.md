---
name: squadsync-api-task
description: Use for SquadSync API/backend implementation tasks under apps/api, including scaffold work, domain/application/API changes, tests, and validation.
---

# SquadSync API Task

Use this skill for scoped API/backend work in `apps/api/`.

## Required Context

Read first:

- `AGENTS.md`
- `PLANS.md` if the task is complex or multi-step
- `apps/api/README.md`
- `docs/architecture/system-overview.md`
- `docs/architecture/domain-model.md`
- `docs/agentic-workflow/workflow/coding-standards.md`
- `docs/agentic-workflow/workflow/testing-strategy.md`
- `docs/agentic-workflow/tools/codex-cli/skills/scaffold-backend.md` for Phase 1 scaffold work

## Process

1. Confirm the GitHub issue scope, non-goals, acceptance criteria, and validation.
2. Identify expected tests before or alongside behavior changes.
3. Preserve Clean Architecture boundaries.
4. Keep changes inside `apps/api/` unless docs or workflow updates are explicitly needed.
5. Run validation or document why it cannot be run.
6. Prepare PR notes with validation evidence and follow-ups.

## Stop Conditions

Stop if the task requires product scope changes, service boundary changes, unapproved infrastructure dependencies, or app architecture changes without an ADR.
