# Subagent Profile: Security Review

## Purpose

Planned role for reviewing security-sensitive changes and sensitive-data handling.

## When to Use

Use this role for future issues involving:

- authentication or authorization
- secrets or configuration
- cloud deployment
- external service integrations
- user data handling
- logging changes

## Required Context

- active GitHub issue and pull request
- `AGENTS.md`
- `CONTRIBUTING.md`
- nearest local `AGENTS.md` for each changed area
- `docs/architecture/system-overview.md`
- exact ADR paths governing the change
- security-sensitive files or configuration docs

## Allowed Changes

- security review notes
- follow-up issue suggestions
- scoped documentation updates if explicitly requested

## Stop Conditions

Stop if review requires approving production security architecture, cloud IAM, secrets handling, or auth strategy without an ADR/human decision.

## Validation Expectations

Report risks, assumptions, missing controls, and recommended follow-up work.

## Status

Planned role profile only. No active subagent automation is configured.
