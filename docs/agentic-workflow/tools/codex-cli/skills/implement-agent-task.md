# Skill: Implement Agent Task

## Purpose

Use this skill when Codex is asked to implement a scoped `agent-ready` issue.

## Flow

1. Read required context from `context-loading.md`.
2. Confirm issue readiness using `issue-intake.md`.
3. Check relevant rules under `rules/`.
4. Identify expected tests or validation gates.
5. Make the smallest change that satisfies acceptance criteria.
6. Run validation or document why it cannot be run.
7. Prepare PR notes using `pr-reporting.md`.
8. Suggest follow-up issues instead of expanding scope.

## TDD-Oriented Guidance

For behavior work:

- identify expected tests before implementation
- add or update tests with the behavior change
- implement the minimum code needed
- refactor only within scope

For docs/scaffold work:

- use docs-only or build validation as appropriate
- document unavailable tests clearly

## Stop Conditions

Stop if the issue is not ready, architecture is unclear, or validation cannot be determined.
