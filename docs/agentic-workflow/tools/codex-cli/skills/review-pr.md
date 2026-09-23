# Skill: Review PR

## Purpose

Use this skill when Codex is asked to support pull request review.

Codex may assist review, but the human owner remains the approval and merge authority.

## Required Context

- the linked issue
- the pull request metadata, body, diff, changed files, available review comments, and check results
- `AGENTS.md`
- `CONTRIBUTING.md`
- `docs/agentic-workflow/specs/pull-request-spec.md`
- `docs/agentic-workflow/workflow/validation-gates.md`
- nearest local `AGENTS.md` and exact source-of-truth paths for changed areas

## Review Checklist

Check whether the PR:

- links to an issue
- stays inside scope
- satisfies acceptance criteria
- reports validation honestly
- states known limitations and remaining review risks
- updates docs/ADRs when needed
- preserves architecture boundaries
- captures follow-up work without silently implementing it

## Review Output

Codex should summarize:

- blocking issues
- non-blocking suggestions
- acceptance-criteria assessment
- validation evidence and limitations
- architecture concerns
- documentation / ADR impact
- follow-up issue suggestions
- recommended reviewer disposition

## Stop Conditions

Stop if the linked issue, PR diff, changed files, or claimed validation evidence cannot be obtained, or if review requires a human product or architecture decision.

Do not approve or merge on behalf of the human owner.
