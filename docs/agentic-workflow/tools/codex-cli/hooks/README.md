# Codex CLI Hooks

## Purpose

Hooks define lifecycle automation points for Codex CLI work in SquadSync.

The project should use this structure now so future executable hooks can be added without restructuring the harness.

## Current Status

No executable Codex hooks are active yet.

The hook structure is intentionally present as a production-shaped placeholder. Planned hooks are documented in `planned-hooks.md`.

## Hook Categories

Future hooks may cover:

- issue readiness checks
- documentation path/link checks
- backend restore/build/test validation
- PR readiness checks
- architecture boundary checks

## Adoption Rule

Only implement an executable hook when:

- the manual step has repeated enough to prove value
- the expected behavior is clear
- the hook reduces review burden
- the hook does not make the workflow harder for the human owner

## Relationship to Generic Validation

Generic validation concepts live in:

```text
docs/agentic-workflow/workflow/validation-gates.md
```

Codex-specific hook candidates live here.
