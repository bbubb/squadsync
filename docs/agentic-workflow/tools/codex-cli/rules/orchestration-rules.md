# Codex Orchestration Rules

## Purpose

These rules define how Codex CLI should align with SquadSync's generic issue orchestration model.

## Rules

Codex must:

- follow `docs/agentic-workflow/workflow/issue-orchestration.md`
- identify task type and affected path before implementation
- load role-relevant context before making changes
- keep work inside the active issue scope
- suggest follow-up issues when work expands

Codex must not:

- route itself into unrelated paths without issue scope
- mix API, web, infra, and docs work unless the issue explicitly scopes it
- activate planned subagents without human/tooling approval
- treat planned role profiles as active automation

## Routing Guidance

For work under:

- `apps/api/`, use API implementation guidance.
- `apps/web/`, use web implementation guidance.
- `infra/`, use infrastructure guidance.
- `docs/`, use docs-maintainer or spec-verifier guidance.
- `.github/`, treat the task as repository automation and validate carefully.

## Stop Conditions

Stop when:

- the issue touches multiple paths without clear scope
- the task requires a role that is only planned and not active
- implementation needs architecture approval
- validation expectations differ by path and are unclear
