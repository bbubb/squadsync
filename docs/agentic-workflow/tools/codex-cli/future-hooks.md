# Future Hooks

## Purpose

This document lists candidate automation hooks for future SquadSync harness improvements.

These hooks are not implemented yet. Add them only after real Codex CLI usage reveals repeated friction.

## Why Hooks Are Deferred

Phase 0 defines the workflow architecture and Codex CLI operational profile.

Executable hooks and scripts should wait until we know which manual steps are repetitive, error-prone, or valuable to automate.

## Candidate Hooks

### Documentation Validation

Possible script:

```text
scripts/validate-docs.sh
```

Potential checks:

- required docs exist
- known links resolve
- roadmap status is consistent
- issue/PR template references are current

### Backend Validation

Possible script:

```text
scripts/validate-backend.sh
```

Potential checks:

- `dotnet restore`
- `dotnet build`
- `dotnet test`

### Agent-Ready Issue Check

Possible script:

```text
scripts/check-agent-ready-issue.sh
```

Potential checks:

- objective exists
- scope exists
- non-goals exist
- acceptance criteria exist
- validation exists
- stop conditions exist

### PR Readiness Check

Possible script:

```text
scripts/check-pr-readiness.sh
```

Potential checks:

- linked issue exists
- acceptance criteria status is included
- validation section is filled
- ADR need is considered
- follow-ups are captured

## Adoption Rule

Only implement a hook when:

- the manual step has repeated at least twice
- the expected behavior is clear
- the hook reduces review burden
- the hook does not make the workflow harder for the human owner
