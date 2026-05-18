---
name: squadsync-spec-verification
description: Use for checking SquadSync documentation consistency before implementation, including pathing, MVP scope, architecture boundaries, and stale references.
---

# SquadSync Spec Verification

Use this skill when an issue, PR, or implementation plan may depend on conflicting or stale docs.

## Required Context

Read first:

- `AGENTS.md`
- `docs/agentic-workflow/workflow/spec-consistency.md`
- `docs/agentic-workflow/workflow/stop-conditions.md`
- `docs/agentic-workflow/tools/codex-cli/rules/spec-discrepancy-rules.md`
- relevant planning, architecture, product, and ADR docs

## Process

1. Identify the affected paths and docs.
2. Compare root summaries and canonical docs.
3. Flag conflicts before implementation.
4. Recommend whether to update docs, create an ADR, or ask the human owner.

## Output

Use this structure:

```text
Conflicting docs:
Conflict:
Likely reference:
Recommended fix:
Can work continue safely?
```
