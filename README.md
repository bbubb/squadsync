# SquadSync

SquadSync is a soccer-focused team management and match-planning platform for coaches. The MVP demonstrates production-minded full-stack architecture, clean service boundaries, and an AI-assisted engineering workflow around a focused soccer use case.

The project is intentionally scoped to a practical vertical slice: team and roster management, match planning, manual lineup construction, and future integration points for lineup assistance and event-driven notifications.

## Current State

SquadSync is in active MVP development.

Phase 0 established architecture, planning, and agentic workflow standards. Phase 1 begins the backend foundation for the API.

Canonical planning state and active work are tracked in:

- [Project Roadmap](docs/planning/project-roadmap.md)
- [GitHub Issues](https://github.com/bbubb/squadsync/issues)
- [GitHub Project board](https://github.com/bbubb/squadsync/projects)

## Tech Stack

| Layer | Technology | Notes |
|---|---|---|
| API | ASP.NET Core Web API | Planned under `apps/api/` |
| API language | C# | Modular monolith / Clean Architecture style |
| Database | PostgreSQL | Local first, cloud-ready later |
| ORM | Entity Framework Core | Planned persistence layer |
| Validation | FluentValidation | Planned request/use-case validation |
| Logging | Serilog | Planned structured logging |
| API docs | Swagger / OpenAPI | Development API inspection |
| Web | Next.js + TypeScript | Planned under `apps/web/` |
| Styling | Tailwind CSS | Planned frontend styling baseline |
| Server state | TanStack Query | Planned API data access pattern |
| Local environment | Docker Compose | Local PostgreSQL and supporting services |
| Testing | xUnit / integration tests | TDD-oriented implementation workflow |
| CI | GitHub Actions | Restore/build/test once application scaffold exists |
| Cloud direction | AWS serverless where practical | Scale-to-zero preference for event-driven capabilities |

## Repo Layout

```text
apps/                 # Application surfaces
  api/                #   ASP.NET Core API / modular monolith
  web/                #   Next.js frontend
infra/                # Local and future cloud infrastructure notes/artifacts
  docker/             #   Local Docker Compose direction
  aws/                #   Future AWS/serverless direction
docs/                 # Architecture, planning, ADRs, integrations, diagrams, workflow docs
  architecture/       #   System and domain architecture
  planning/           #   Roadmap, MVP scope, implementation planning
  adr/                #   Architecture decision records
  integrations/       #   External service boundaries such as soccer-subber
  agentic-workflow/   #   AI-assisted workflow, tool profiles, rules, skills, hooks
.github/              # Issue templates, PR template, and future CI workflows
```

## Architecture

SquadSync is designed as a modular monorepo. The core API starts as a modular monolith with clear Domain, Application, Infrastructure, and API boundaries. External capabilities such as `soccer-subber` lineup assistance and AWS-based notifications are integrated through explicit contracts/ports instead of being embedded directly into the core platform.

Start here:

- [System Overview](docs/architecture/system-overview.md)
- [Domain Model](docs/architecture/domain-model.md)
- [Architectural Decisions](docs/adr/)
- [Soccer-Subber Integration](docs/integrations/soccer-subber/README.md)

## Agentic Workflow

SquadSync uses repository-owned planning, workflow, and tool-profile documents so AI-assisted development remains reviewable and durable. GitHub issues define executable work, pull requests provide review gates, and human review owns merge decisions.

Start here:

- [AGENTS.md](AGENTS.md)
- [Agentic Workflow Architecture](docs/agentic-workflow/README.md)
- [ChatGPT GitHub Tool Profile](docs/agentic-workflow/tools/chatgpt-github/README.md)
- [Codex CLI Tool Profile](docs/agentic-workflow/tools/codex-cli/README.md)
- [Branching Strategy](docs/agentic-workflow/workflow/branching-strategy.md)
- [Testing Strategy](docs/agentic-workflow/workflow/testing-strategy.md)
- [Coding Standards](docs/agentic-workflow/workflow/coding-standards.md)

## Getting Started

Application code has not been scaffolded yet. Phase 1 will create the API foundation under `apps/api/` with build and test validation.

Until then, begin with the documentation map below and the active GitHub issue/PR workflow.

## Contributing

This project uses trunk-based GitHub Flow:

- create short-lived branches from `main`
- link work to GitHub issues
- keep pull requests small and reviewable
- run or document validation before review
- do not merge without human approval

See:

- [Branching Strategy](docs/agentic-workflow/workflow/branching-strategy.md)
- [Pull Request Specification](docs/agentic-workflow/specs/pull-request-spec.md)
- [Validation Gates](docs/agentic-workflow/workflow/validation-gates.md)

## Links

### Planning

- [Project Roadmap](docs/planning/project-roadmap.md)
- [MVP Scope](docs/planning/mvp-scope.md)

### Architecture

- [System Overview](docs/architecture/system-overview.md)
- [Domain Model](docs/architecture/domain-model.md)
- [Architectural Decisions](docs/adr/)
- [Diagrams](docs/diagrams/README.md)

### Integrations and Infrastructure

- [Soccer-Subber Integration](docs/integrations/soccer-subber/README.md)
- [Infrastructure](infra/README.md)
- [Docker Direction](infra/docker/README.md)
- [AWS Direction](infra/aws/README.md)

### Agentic Workflow

- [Agentic Workflow Architecture](docs/agentic-workflow/README.md)
- [ChatGPT GitHub Tool Profile](docs/agentic-workflow/tools/chatgpt-github/README.md)
- [Codex CLI Tool Profile](docs/agentic-workflow/tools/codex-cli/README.md)
- [Codex Rules](docs/agentic-workflow/tools/codex-cli/rules/README.md)
- [Codex Skills](docs/agentic-workflow/tools/codex-cli/skills/README.md)
- [Codex Hooks](docs/agentic-workflow/tools/codex-cli/hooks/README.md)
- [Codex Subagents](docs/agentic-workflow/tools/codex-cli/subagents/README.md)

## License

No license has been selected yet.
