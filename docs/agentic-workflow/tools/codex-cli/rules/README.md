# Codex CLI Rules

## Purpose

Rules define persistent behavioral constraints for Codex CLI when working in SquadSync.

Rules are tool-specific expressions of the broader project policy, workflow, and architecture standards.

## Current Rule Sets

- `project-rules.md` — repository and workflow behavior Codex must follow.
- `architecture-rules.md` — application architecture boundaries Codex must preserve.
- `workflow-rules.md` — issue, branch, PR, validation, and stop-condition rules.
- `documentation-rules.md` — documentation governance, headers, sync, and stale-reference behavior.
- `spec-discrepancy-rules.md` — source-of-truth conflict handling.
- `orchestration-rules.md` — issue routing, path routing, and planned role behavior.

## Relationship to Generic Docs

Generic policy lives under:

```text
docs/agentic-workflow/policy/
docs/agentic-workflow/workflow/
docs/agentic-workflow/specs/
```

Codex-specific rule interpretation lives here.

## Status

These rules are active documentation constraints.

They are not tool-enforced yet. Future Codex setup can adapt these docs into tool-native config without changing the repository architecture.
