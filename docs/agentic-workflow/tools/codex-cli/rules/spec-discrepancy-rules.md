# Codex Spec Discrepancy Rules

## Purpose

These rules define how Codex CLI should respond when repository docs conflict.

## Rules

Codex must:

- follow `docs/agentic-workflow/workflow/spec-consistency.md`
- pause when source-of-truth conflict affects implementation
- report the discrepancy clearly
- recommend a docs fix, ADR, or human decision path

Codex must not:

- silently choose between conflicting docs when the source of truth is unclear
- implement against a stale derivative doc
- override an accepted ADR with roadmap or README language
- continue implementation when app pathing, MVP scope, or architecture boundaries conflict

## Required Report Format

```markdown
## Spec Discrepancy

Conflicting docs:

Conflict:

Likely source of truth:

Recommended fix:

Can implementation continue safely?
```

## Common Examples

- README says one app path, system overview says another.
- Roadmap phase scope conflicts with MVP scope.
- Codex skill references old scaffolding path.
- Architecture docs and ADR disagree.
- Product notes imply scope not approved in MVP scope.

## Validation

After resolving a discrepancy, verify affected root summaries and derivative docs are aligned.
