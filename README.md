# SquadSync

SquadSync is a soccer team management and match-planning platform for coaches. The project demonstrates production-minded full-stack architecture using ASP.NET Core, a modern React/Next.js frontend, relational persistence, and planned integration points for lineup assistance and event-driven notifications.

## Current Status

Sprint 0 establishes the project foundation: architecture docs, agent workflow guidance, architectural decision records, and repository instructions for AI-assisted development.

Application code will be added after the foundation documents are reviewed and accepted.

## Product Scope

The MVP focuses on:

- Team creation and roster management
- Player and coach profiles
- Team membership and role assignment
- Match planning
- Manual lineup construction
- A future integration contract for a separate `soccer-subber` lineup assistance service
- A future AWS event-notification path for lineup and roster events

## Proposed Architecture

```text
frontend/      Next.js + TypeScript dashboard
backend/       ASP.NET Core modular monolith
infra/         Local Docker and future AWS infrastructure artifacts
docs/          Architecture, planning, ADRs, diagrams, and agent workflows
.github/       CI, issue templates, and AI coding assistant instructions
```

The core platform will be built as a modular monorepo. Integrated services such as `soccer-subber` and AWS-based notifications are treated as separate deployable capabilities to demonstrate service boundaries, cloud integration, and scalable design.

## Planned Technical Stack

- Backend: C# / ASP.NET Core Web API
- Application architecture: modular monolith with Domain, Application, Infrastructure, and API boundaries
- Database: PostgreSQL
- ORM: Entity Framework Core
- Validation: FluentValidation
- Logging: Serilog
- API documentation: Swagger/OpenAPI
- Frontend: Next.js, TypeScript, Tailwind CSS, TanStack Query
- Local environment: Docker Compose
- Testing: xUnit, integration tests, and later architecture/dependency tests
- Cloud direction: AWS-aligned eventing, deployment, and service integration

## Documentation Map

Start here:

- [MVP Scope](docs/planning/mvp-scope.md)
- [System Overview](docs/architecture/system-overview.md)
- [Domain Model](docs/architecture/domain-model.md)
- [Codex Task Lifecycle](docs/agent-workflows/codex-task-lifecycle.md)
- [Architectural Decisions](docs/adr/)

AI coding agents and assistants should read [AGENTS.md](AGENTS.md) before modifying the repository.

## Development Philosophy

This project prioritizes:

- Clear architecture over clever abstractions
- Explicit domain relationships over premature generalization
- Small, reviewable changes
- Documentation-backed decisions
- AI-assisted development with human review gates
- A working vertical slice before broad feature expansion

## Scope Rule

Keep implementation aligned with the documented soccer MVP. New product areas, architectural patterns, or service responsibilities should be introduced through an issue and, when architectural, an ADR.
