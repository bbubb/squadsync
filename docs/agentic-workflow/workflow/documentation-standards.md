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

## Public Relevance and Disclosure Gate

Before publishing **new or revised** content in a public repository document, issue, PR, ADR, example, non-goal, or agent prompt, apply both checks:

1. **Relevance — does this belong in this public project?** Include a concept only when it explains an actual MVP requirement or architectural constraint, records an owner-approved *deliberately public* future MVP direction, or is necessary to prevent a concrete foreseeable implementation error. **Public-safe does not imply public-relevant.** Speculative future scenarios and lists of excluded features do not pass merely because they are common knowledge.
2. **Disclosure — is it authorized for publication?** Even relevant information must not reveal confidential strategy, private explorations, or unapproved scope. If uncertain, **withhold the detail and ask the owner to classify it** before publishing.

Apply **minimum necessary wording**: state the current invariant without introducing adjacent hypothetical actors, workflows, exclusions or examples. Removing an irrelevant future concept is usually better than rephrasing it. Do not create a public backlog issue, placeholder, or negative-scope hint solely to retain a private idea.

When a consequential idea is withheld, preserve its intent, unresolved questions and revisit trigger in an authorized **private** source outside the public repository. Never reference private source files, confidential scenarios or their IDs in public issue/PR text or Codex prompts. Existing [context-management guidance](context-management.md#deferred-decision-handoff) governs public-safe decisions that genuinely affect the MVP.

**Review check:** During planning, issue readiness and PR review, examine newly introduced future-facing language (including `Non-Goals` and `Deferred` sections). If it fails either test, remove it from the public change before merge and verify that any consequential private context is retained separately. This is an editorial check within the existing workflow, not a new approval stage or parallel register.

## Change Discipline

When updating docs:

- update the header date/version, when present, only when the change is meaningful
- keep change summaries concise
- remove stale references
- avoid duplicating canonical content
- link to canonical docs instead of copying large sections
- keep README and AGENTS guidance evergreen: document current behavior, setup, validation instructions, and lasting constraints; record dated agent-environment observations, one-off test results, and issue/PR execution history in the relevant GitHub issue or PR instead. Describe persistent limitations without tying them to a particular run

## Agent Guidance

Agents should stop and request clarification when:

- a doc appears stale but no replacement is obvious
- two canonical docs conflict
- a change would require updating multiple root summaries or indexes
- a doc should be superseded rather than edited in place
