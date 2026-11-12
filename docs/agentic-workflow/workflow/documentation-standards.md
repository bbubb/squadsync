# Documentation Standards

## Purpose

This document defines baseline documentation governance for SquadSync.

The goal is to keep docs useful for humans, ChatGPT GitHub, Codex CLI, and future agents without creating unnecessary process overhead.

## Status Treatment

Major planning, architecture, workflow, and product documents should state their status near the top when the distinction between draft, active, historical, or placeholder materially affects how they should be used.

Use a simple status section by default:

```markdown
## Status

Active. This document is the current implementation baseline.
```

Use a fuller metadata header only when version, date, owner, and change-summary fields materially help govern the document. Do not add metadata solely for uniformity or update it for routine wording and link fixes.

Optional format:

```markdown
## Document Header

| Field | Value |
|---|---|
| Version | v0.1.0 |
| Date | YYYY-MM-DD |
| Owner | Responsible role |
| Status | Draft / Active / Accepted / Superseded |
| Change Summary | Concise summary of the latest meaningful change. |
```

Placeholder READMEs do not require full metadata, but they must clearly identify their planned or inactive status.

## Status Values

Use these status values:

- Draft: still being shaped and may change significantly.
- Active: currently valid and used by humans/agents.
- Accepted: decision-bearing and stable unless superseded.
- Superseded: replaced by another document.
- Deprecated: intentionally retained but should not guide new work.

## Versioning

When a document uses an explicit version field, use simple semantic-style versions:

- Patch bump: small clarification that changes guidance.
- Minor bump: new section, changed guidance, or meaningful scope expansion.
- Major bump: incompatible governance, architecture, or workflow change.

Routine typo, formatting, and link fixes do not require version or status churn.

## Related Documents

When a document depends on other documents, include links in a References or Related section.

Agents should follow references before changing derivative docs.

## Change Discipline

When updating docs:

- update the header date/version, when present, only when the change is meaningful
- keep change summaries concise
- remove stale references
- avoid duplicating canonical content
- link to canonical docs instead of copying large sections
- keep README and AGENTS guidance evergreen: document current behavior, setup, validation instructions, and lasting constraints; record dated agent-environment observations, one-off test results, and issue/PR execution history in the relevant GitHub issue or PR instead. Describe persistent limitations without tying them to a particular run
- before publishing, include only what is needed for the approved public project scope; omit speculative or unrelated future capabilities, including unnecessary non-goals and deferrals. When relevance or disclosure is uncertain, obtain owner approval first

## Agent Guidance

Agents should stop and request clarification when:

- a doc appears stale but no replacement is obvious
- two canonical docs conflict
- a change would require updating multiple root summaries or indexes
- a doc should be superseded rather than edited in place
