# Documentation Agent Instructions

## Status

Active for documentation and workflow maintenance.

This file provides local guidance for agents working under `docs/`.

## Purpose

`docs/` contains SquadSync's canonical planning, architecture, product, integration, and agentic workflow documents.

Documentation changes should clarify the project and reduce future ambiguity. Do not add documents that are vague, duplicative, or disconnected from active project goals.

## Documentation Rules

- Follow `docs/agentic-workflow/workflow/documentation-standards.md`.
- Use document headers on major canonical docs when required by the documentation standard.
- Keep summaries concise and linked to deeper canonical docs.
- Mark placeholder documents clearly.
- Update root summaries when canonical direction changes.
- Check derivative docs when source docs change.

## Quality Bar

A document should be:

- accurate;
- concise;
- professionally written;
- clear about active vs planned status;
- linked to its canonical upstream/downstream context;
- useful to a human reviewer or implementation agent.

## Stop Conditions

Agents should stop before changing docs if:

- two canonical docs conflict and the correct source of truth is unclear;
- the change would alter product scope, architecture, or phase direction without an ADR or planning issue;
- the requested doc appears to duplicate an existing canonical doc;
- the content would be filler rather than meaningful guidance.

## Required Context

Before changing documentation, review:

- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/agentic-workflow/workflow/documentation-standards.md`
- `docs/agentic-workflow/workflow/root-summary-sync.md`
- `docs/agentic-workflow/workflow/spec-consistency.md`
- `.agents/skills/squadsync-docs-maintenance/SKILL.md`
