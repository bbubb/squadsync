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
- the linked GitHub issue, PR body, diff, changed files, available review comments, and check results

## Review Focus

Check whether the PR:

- stays inside scope
- satisfies acceptance criteria
- reports validation honestly
- states known limitations and remaining review risks
- preserves architecture boundaries
- updates docs when needed
- captures follow-up work without expanding scope

## Output

Report blocking issues first, then non-blocking suggestions, acceptance-criteria status, validation evidence and limitations, architecture concerns, documentation impact, follow-up issue suggestions, and a recommended reviewer disposition.

Do not approve or merge on behalf of the human owner.

Stop and report the limitation if the linked issue, PR diff, changed files, or claimed validation evidence cannot be obtained.
