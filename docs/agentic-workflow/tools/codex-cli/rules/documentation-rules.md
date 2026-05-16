# Codex Documentation Rules

## Purpose

These rules define how Codex CLI should handle documentation changes in SquadSync.

## Rules

Codex must:

- follow `docs/agentic-workflow/workflow/documentation-standards.md`
- check `docs/agentic-workflow/workflow/root-summary-sync.md` when changing root-level summaries or navigation
- check `docs/agentic-workflow/workflow/spec-consistency.md` when editing derivative docs
- update document headers when making meaningful changes to major docs
- remove stale references when moving, renaming, or superseding docs
- link to canonical docs instead of duplicating large sections

Codex must not:

- create parallel workflow structures without approval
- leave obsolete paths in root summaries or indexes
- silently resolve conflicting docs when the source of truth is unclear
- convert lightweight placeholder docs into full specs unless the issue explicitly scopes that work

## Stop Conditions

Stop when:

- documentation conflicts with an ADR
- root summaries disagree about current pathing or phase direction
- a doc appears superseded but no replacement is clear
- the change would alter product scope or architecture without approval

## Validation

For documentation PRs, report:

- paths/links checked
- root summaries checked
- stale references removed or intentionally retained
- unresolved discrepancies, if any
