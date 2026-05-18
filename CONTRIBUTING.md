# Contributing

SquadSync uses issue-backed, reviewable changes. This guide applies to humans and AI-assisted workflows.

## Source of Truth

Before changing the repository, read:

- [README.md](README.md)
- [AGENTS.md](AGENTS.md)
- [PLANS.md](PLANS.md)
- [Project Roadmap](docs/planning/project-roadmap.md)
- [Agentic Workflow Architecture](docs/agentic-workflow/README.md)
- [Branching Strategy](docs/agentic-workflow/workflow/branching-strategy.md)
- [Pull Request Specification](docs/agentic-workflow/specs/pull-request-spec.md)
- [Validation Gates](docs/agentic-workflow/workflow/validation-gates.md)

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
