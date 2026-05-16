# Planned Subagents

## Purpose

This document summarizes planned Codex subagent roles for SquadSync.

These roles are not active. They are structured placeholders for future orchestration if the project outgrows a single implementation agent.

## Current Status

No active subagent automation is configured.

Codex CLI remains the primary implementation profile. The human owner remains responsible for approval, merge decisions, phase transitions, and architecture decisions.

## Planned Role Profiles

| Role | Profile | Status |
|---|---|---|
| API Implementation | `api-impl.md` | Planned |
| Web Implementation | `web-impl.md` | Planned |
| Infrastructure Implementation | `infra-impl.md` | Planned |
| QA Review | `qa-review.md` | Planned |
| Documentation Maintainer | `docs-maintainer.md` | Planned |
| Spec Verifier | `spec-verifier.md` | Planned |
| Security Review | `security-review.md` | Planned |

## Activation Criteria

Do not activate subagents until:

- Phase 1 has working tests and CI
- role boundaries are validated through real issues
- the cost/complexity tradeoff is justified
- validation gates are strong enough to support delegation
- human review remains explicit

## Relationship to Issue Orchestration

Generic routing guidance lives in:

```text
docs/agentic-workflow/workflow/issue-orchestration.md
```

Codex-specific role profiles live in this directory.
