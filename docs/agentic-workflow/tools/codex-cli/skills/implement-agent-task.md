# Skill: Implement Agent Task

## Purpose

Use this skill when Codex is asked to implement a scoped `agent-ready` issue.

## Flow

1. Read root `AGENTS.md` and immediately retrieve the active issue using authenticated repository-aware GitHub tooling, or `gh issue view <number> --repo bbubb/squadsync` as the CLI fallback.
2. Read `CONTRIBUTING.md` for the repository-wide issue, branch, PR, and validation rules.
3. Confirm issue readiness using `docs/agentic-workflow/tools/codex-cli/issue-intake.md`; do not rely on the label alone.
4. Use `docs/agentic-workflow/tools/codex-cli/context-loading.md` to resolve only the task-specific context required by the issue and affected paths.
5. Check the task-applicable rules routed through `docs/agentic-workflow/tools/codex-cli/rules/README.md`.
6. Before editing any file, inspect and verify Git state, confirm the working tree is understood, and create or switch to the short-lived issue branch from `main`. Verify the active branch name and clean/expected working tree before proceeding. If repository state cannot be established or the issue branch cannot be created, stop without modifying files.
7. Identify expected tests or validation gates.
8. Make the smallest change that satisfies acceptance criteria.
9. Run validation or document why it cannot be run.
10. Commit and push the issue branch, then create a draft PR using `docs/agentic-workflow/tools/codex-cli/pr-reporting.md`. These are required parts of normal task completion. If a documented blocker prevents commit, push, or draft-PR creation, report the handoff as incomplete and identify the blocker; do not describe the task as complete.
11. Suggest follow-up issues instead of expanding scope.

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
