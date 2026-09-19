# Codex CLI Subagents

## Purpose

Subagents are specialized delegated roles that may be useful in future Codex or orchestration workflows.

This directory defines planned role profiles without enabling subagent automation prematurely.

## Current Status

No active subagents are configured for SquadSync.

Codex CLI remains the primary implementation profile. The human owner remains the approval and merge authority.

## Planned Role Profiles

- [API implementation](api-impl.md) — API/backend implementation role.
- [Web implementation](web-impl.md) — web/frontend implementation role.
- [Infrastructure implementation](infra-impl.md) — local/cloud infrastructure role.
- [QA review](qa-review.md) — test and validation review role.
- [Documentation maintainer](docs-maintainer.md) — documentation and sync role.
- [Spec verifier](spec-verifier.md) — source-of-truth consistency role.
- [Security review](security-review.md) — security and sensitive-data review role.
- [Planned subagents](planned-subagents.md) — high-level status and adoption notes.

## Role Profile Standard

Each role profile should define:

- purpose
- when to use
- required context, including the active issue and nearest local `AGENTS.md` when path-scoped
- allowed changes
- stop conditions
- validation expectations

## Adoption Rule

Only activate or formalize subagents when:

- one implementation agent is no longer enough for the workflow
- validation gates are strong enough to support delegation
- role boundaries are clear
- cost and complexity are justified
- human review remains explicit
