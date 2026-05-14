# Codex CLI PR Reporting

## Purpose

This document defines how Codex CLI should summarize completed work for pull request review.

## PR Summary Requirements

A Codex-generated PR should include:

- what changed
- why it changed
- related issue number
- files or areas changed
- acceptance criteria status
- validation performed
- validation limitations
- docs or ADR impact
- known follow-ups

## Acceptance Criteria Mapping

Codex should map each issue acceptance criterion to a status:

```markdown
## Acceptance Criteria Status

- [x] Criterion 1 — completed by ...
- [x] Criterion 2 — completed by ...
- [ ] Criterion 3 — not completed because ...
```

## Architecture Notes

If the task touches architecture, service boundaries, workflow structure, dependencies, or project conventions, Codex should explain the trade-off and whether an ADR is needed.

If no architecture impact exists, write `N/A`.

## Follow-up Work

Codex should suggest follow-up issues when it discovers related work outside the current scope.

Codex should not silently implement follow-up work unless the issue explicitly includes it.

## Review Tone

PR descriptions should be concise, factual, and understandable to a future reviewer.

Avoid vague claims such as "improved architecture" unless the PR explains exactly what changed.
