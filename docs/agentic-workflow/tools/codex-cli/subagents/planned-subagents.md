# Planned Subagents

## Purpose

This document identifies possible future subagent roles for SquadSync.

These are not active. They are placeholders for future orchestration if the project outgrows a single implementation agent.

## Candidate Roles

### Implementation Agent

Focus:

- scoped code changes
- tests
- validation
- PR preparation

Status: active as the general Codex CLI role, not a separate subagent.

### Review Agent

Focus:

- PR review support
- acceptance criteria checks
- validation review
- architecture boundary review

Status: planned.

### Documentation Agent

Focus:

- docs updates
- stale reference detection
- ADR drafting support
- closeout summaries

Status: planned.

### Test Agent

Focus:

- test gap analysis
- TDD assistance
- test project maintenance
- regression coverage suggestions

Status: planned.

### Orchestration Agent

Focus:

- routing issues to roles
- managing parallel work
- coordinating validation status

Status: future; likely only relevant if Symphony or another orchestrator is adopted.

## Activation Criteria

Do not activate subagents until:

- Phase 1 has working tests and CI
- role boundaries are documented
- the cost/complexity tradeoff is justified
- human review remains explicit
