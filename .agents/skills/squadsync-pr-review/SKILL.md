---
name: squadsync-pr-review
description: Use for reviewing SquadSync pull requests against issue scope, acceptance criteria, validation evidence, architecture boundaries, and follow-up handling.
---

# SquadSync PR Review

Use this skill for pull request review support.

## Required Context

Read first:

- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/agentic-workflow/specs/pull-request-spec.md`
- `docs/agentic-workflow/workflow/validation-gates.md`
- `docs/agentic-workflow/tools/codex-cli/skills/review-pr.md`
- the linked GitHub issue and PR body

## Review Focus

Check whether the PR:

- stays inside scope
- satisfies acceptance criteria
- reports validation honestly
- preserves architecture boundaries
- updates docs when needed
- captures follow-up work without expanding scope

## Output

Report blocking issues first, then non-blocking suggestions, validation concerns, architecture concerns, and follow-up issue suggestions.

Do not approve or merge on behalf of the human owner.
