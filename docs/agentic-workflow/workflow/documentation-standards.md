# Documentation Standards

## Purpose

This document defines baseline documentation governance for SquadSync.

The goal is to keep docs useful for humans, ChatGPT GitHub, Codex CLI, and future agents without creating unnecessary process overhead.

## Documentation Header

Major planning, architecture, workflow, and product docs should include a lightweight document header near the top of the file.

Recommended format:

```markdown
## Document Header

| Field | Value |
|---|---|
| Version | v0.1.0 |
| Date | YYYY-MM-DD |
| Owner | Brandon Bunch |
| Status | Draft / Active / Accepted / Superseded |
| Change Summary | Concise summary of the latest meaningful change. |
```

## When Headers Are Required

Use document headers for:

- core planning docs
- architecture docs
- product docs
- major agentic workflow docs
- ADR-like governance docs when not already covered by ADR format

Document headers are not required for every small placeholder README.

## Status Values

Use these status values:

- Draft: still being shaped and may change significantly.
- Active: currently valid and used by humans/agents.
- Accepted: decision-bearing and stable unless superseded.
- Superseded: replaced by another document.
- Deprecated: intentionally retained but should not guide new work.

## Versioning

Use simple semantic-style document versions:

- Patch bump: typo, link, wording, or small clarification.
- Minor bump: new section, changed guidance, or meaningful scope expansion.
- Major bump: incompatible governance, architecture, or workflow change.

## Related Documents

When a document depends on other documents, include links in a References or Related section.

Agents should follow references before changing derivative docs.

## Change Discipline

When updating docs:

- update the header date/version when the change is meaningful
- keep change summaries concise
- remove stale references
- avoid duplicating canonical content
- link to canonical docs instead of copying large sections

## Agent Guidance

Agents should stop and request clarification when:

- a doc appears stale but no replacement is obvious
- two canonical docs conflict
- a change would require updating multiple root summaries or indexes
- a doc should be superseded rather than edited in place
