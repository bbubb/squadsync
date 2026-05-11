# SquadSync

SquadSync is a public, portfolio-safe rebuild of a soccer team management and match-planning platform. The project demonstrates production-minded full-stack architecture using ASP.NET Core, a modern React/Next.js frontend, relational persistence, and a planned service boundary for lineup optimization and event-driven notifications.

This repository is intentionally scoped to a narrow soccer-focused vertical slice. The goal is to show practical engineering depth without exposing broader private platform/IP concepts.

## Current Status

Sprint 0 is establishing the project foundation: architecture docs, agent workflow guidance, architectural decision records, and repo instructions for AI-assisted development.

Application code will be added after the foundation documents are reviewed and accepted.

## Product Scope

The public MVP focuses on:

- Team creation and roster management
- Player and coach profiles
- Explicit team membership and role assignment
- Match planning
- Manual lineup construction
- A future integration contract for a separate `soccer-subber` optimization service
- A future AWS event-notification path for lineup and roster events

The public MVP does not attempt to expose the broader long-term competition-platform concept or proprietary substitution algorithm internals.

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
- Database: PostgreSQL for the public rebuild target
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
- Public-safe scope boundaries
- Small, reviewable changes
- Documentation-backed decisions
- AI-assisted development with human review gates

## Repository Safety Rule

Do not add proprietary algorithm details, generalized competition-platform abstractions, or private strategic IP to this public repository. Keep implementation soccer-focused and portfolio-safe.
