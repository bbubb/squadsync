# Planned Hooks

## Purpose

This document maps intended Codex hook points before executable hooks exist.

The structure should remain stable as the project matures. Individual planned hooks can later become scripts, configuration, or CI checks.

## Planned Hook Points

### Issue Readiness Hook

Goal:

- verify objective, scope, non-goals, acceptance criteria, validation, and stop conditions exist

Possible future implementation:

```text
scripts/check-agent-ready-issue.sh
```

### Documentation Validation Hook

Goal:

- verify required docs exist
- catch stale path references
- check common workflow links

Possible future implementation:

```text
scripts/validate-docs.sh
```

### Backend Validation Hook

Goal:

- run backend restore/build/test commands once backend scaffold exists

Possible future implementation:

```text
scripts/validate-backend.sh
```

Expected commands:

```bash
dotnet restore
dotnet build
dotnet test
```

### PR Readiness Hook

Goal:

- check linked issue, acceptance criteria status, validation section, ADR consideration, and follow-up capture

Possible future implementation:

```text
scripts/check-pr-readiness.sh
```

### Architecture Boundary Hook

Goal:

- detect dependency direction violations once backend projects exist

Possible future implementation:

```text
scripts/check-architecture-boundaries.sh
```

## Promotion Criteria

Promote a planned hook to executable only after:

- a real sprint exposes repeated friction
- the expected check is deterministic
- the script can run locally and in CI
- the added automation reduces review burden
