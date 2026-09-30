# Contributing

SquadSync uses issue-backed, reviewable changes. This guide applies to humans and AI-assisted workflows.

## Source of Truth

Use the active issue and the root `AGENTS.md` to route task context. This guide is the repository-wide baseline for issue scope, branches, pull requests, and validation; it is not a checklist to preload every linked project document.

Read the nearest scoped `AGENTS.md` and applicable skill or playbook for the affected area. Follow their references to planning, architecture, product, workflow, or validation sources when the issue, affected path, or a concrete dependency, ambiguity, conflict, or architecture question requires them. Keep `PLANS.md` conditional on work that is complex, cross-cutting, risky, or multi-step. `README.md` is available for repository orientation when needed.

## Workflow

```text
Issue -> short-lived branch -> scoped change -> validation -> pull request -> human review -> merge
```

## Contribution Rules

- Start from a GitHub issue unless the human owner explicitly approves an emergency direct fix.
- Keep each branch and PR scoped to one focused change.
- Use short-lived branches from `main`.
- Use `PLANS.md` for complex, cross-cutting, risky, or multi-step work.
- Preserve documented architecture boundaries.
- Update docs when behavior, architecture, workflow, or scope changes.
- Run relevant validation or document why validation cannot be run.
- Do not merge without human approval.

## Branch Naming

Use clear branch names:

```text
docs/<short-topic>
feature/<short-topic>
fix/<short-topic>
chore/<short-topic>
```

## Pull Requests

Each PR should include:

- linked issue
- scope and non-goals
- acceptance criteria status
- validation performed
- documentation/ADR impact
- known limitations
- follow-up work, if any

## Testing and Validation

For docs-only changes, validate paths, links, terminology, and scope.

For future API code, expected baseline validation will be:

```bash
cd apps/api
dotnet restore
dotnet build
dotnet test
```

## AI-Assisted Work

ChatGPT GitHub and Codex CLI workflows are documented under:

- [ChatGPT GitHub Tool Profile](docs/agentic-workflow/tools/chatgpt-github/README.md)
- [Codex CLI Tool Profile](docs/agentic-workflow/tools/codex-cli/README.md)

Repo-native Codex skills live under:

- [.agents/skills](.agents/skills)

AI tools may assist planning, documentation, implementation, and review, but the human owner retains final authority over architecture, scope, and merge decisions.
