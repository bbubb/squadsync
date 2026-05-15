# Skill: Review PR

## Purpose

Use this skill when Codex is asked to support pull request review.

Codex may assist review, but the human owner remains the approval and merge authority.

## Review Checklist

Check whether the PR:

- links to an issue
- stays inside scope
- satisfies acceptance criteria
- reports validation honestly
- updates docs/ADRs when needed
- preserves architecture boundaries
- captures follow-up work without silently implementing it

## Review Output

Codex should summarize:

- blocking issues
- non-blocking suggestions
- validation concerns
- architecture concerns
- follow-up issue suggestions

## Stop Conditions

Stop if review requires a human product or architecture decision.

Do not approve or merge on behalf of the human owner.
