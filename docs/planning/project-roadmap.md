# Project Roadmap

## Status

Draft for ongoing planning.

## Purpose

This roadmap keeps SquadSync aligned across branched conversations, future sprints, GitHub issues, and AI-assisted implementation work. It defines the major project phases at a high level so each sprint can be planned without losing the larger direction.

## Product Direction

SquadSync is a soccer team management and match-planning platform for coaches. The first implementation should produce a useful vertical slice before expanding into advanced services, cloud deployment, or AI-assisted features.

## Working Strategy

Use the roadmap as the stable source of direction. Each implementation thread should begin from the current phase and sprint context, then produce narrow GitHub issues or PRs.

Recommended workflow:

```text
Roadmap defines phase direction.
Sprint context defines immediate goals.
GitHub issues define executable work.
Branches/PRs contain implementation.
Docs/ADRs preserve decisions.
Agentic workflow docs define how tools execute work.
```

## Phase 0: Agent-Ready Project Foundation + Operational Harness

### Goal

Create a professional foundation for architecture, planning, and AI-assisted development before application code is generated.

Phase 0 is not complete until the repository contains both:

- strategic project documentation
- a repo-owned agentic workflow architecture with usable ChatGPT GitHub and Codex CLI tool profiles

### Deliverables

- README
- MVP scope document
- System overview
- Domain model
- ADRs for core decisions
- Agent instructions
- Agentic workflow architecture under `docs/agentic-workflow/`
- Generic policy, workflow, and task specification docs
- ChatGPT GitHub tool profile
- Codex CLI operational profile
- Symphony future/reference profile
- Agent-ready issue template
- Pull request review template
- Project roadmap

### Operational Harness Outcomes

The current Phase 0 hardening work adds:

- layered `docs/agentic-workflow/` structure
- generic policy/workflow/spec layers
- ChatGPT GitHub tool profile
- Codex CLI tool profile
- validation gates and stop conditions
- agent-ready issue template
- PR template alignment
- future harness friction log

### Completion Criteria

- Repository purpose is clear.
- MVP scope is defined.
- Architecture direction is documented.
- Domain model is documented.
- AI-assisted workflow is documented.
- GitHub issues can act as executable task records.
- ChatGPT GitHub can support planning, issue/PR creation, review, and closeout from repository-owned context.
- Codex CLI can load repository-owned context before implementation.
- Validation gates and stop conditions are documented.
- Tool-specific behavior is nested under `docs/agentic-workflow/tools/`.
- Phase 1 can begin without relying on hidden ChatGPT session context.

## Phase 1: Backend Foundation

### Goal

Create the backend solution scaffold and local development baseline.

### Primary Outcomes

- ASP.NET Core solution under `backend/`
- Modular projects: API, Application, Domain, Infrastructure
- Unit and integration test projects
- Health endpoint
- Swagger/OpenAPI
- Serilog console logging
- PostgreSQL local development path
- Docker Compose baseline
- GitHub Actions build/test workflow

### Example Sprints

- Sprint 1: backend scaffold
- Sprint 2: local database and infrastructure baseline
- Sprint 3: CI/build/test hardening

### Completion Criteria

- Backend builds locally.
- Health endpoint works.
- Swagger is available in development.
- Project references enforce intended dependency direction.
- Basic CI validates restore/build/test.

## Phase 2: Core Domain and Persistence

### Goal

Implement the MVP domain model and persistence layer.

### Primary Outcomes

- User entity/model
- Team entity/model
- TeamMembership entity/model
- Role model
- PlayerProfile model
- CoachProfile model if needed
- EF Core DbContext
- Entity configurations
- Initial migrations
- Seed data for development/demo
- Basic domain/application tests

### Example Sprints

- Sprint 4: team and user domain model
- Sprint 5: membership and role model
- Sprint 6: player profile and roster persistence

### Completion Criteria

- Core entities exist and persist.
- Relationships match `docs/architecture/domain-model.md`.
- Basic CRUD or use-case paths can be tested.
- Seed data supports a simple demo scenario.

## Phase 3: Roster Management API

### Goal

Expose usable backend behavior for team and roster management.

### Primary Outcomes

- Team endpoints/use cases
- Roster endpoints/use cases
- Player profile endpoints/use cases
- Membership role assignment
- Validation
- Standard API responses/errors
- Integration tests for core flows

### Example Sprints

- Sprint 7: team management API
- Sprint 8: roster/player profile API
- Sprint 9: membership role API and validation

### Completion Criteria

- A coach can create a team.
- A coach can add/manage roster members.
- Player profile data can be created and updated.
- API behavior is validated by tests.

## Phase 4: Frontend Foundation

### Goal

Create the frontend application shell and connect it to backend APIs.

### Primary Outcomes

- Next.js + TypeScript app under `frontend/`
- Feature-oriented structure
- Tailwind setup
- API service layer
- TanStack Query setup
- Dashboard layout
- Basic team/roster screens

### Example Sprints

- Sprint 10: frontend scaffold and app shell
- Sprint 11: team dashboard
- Sprint 12: roster management UI

### Completion Criteria

- Frontend runs locally.
- Frontend can call backend APIs.
- A reviewer can navigate the basic team/roster workflow.

## Phase 5: Match and Manual Lineup Planning

### Goal

Implement the first soccer-specific planning workflow.

### Primary Outcomes

- Match model/API/UI
- Player availability model/API/UI
- Formation selection
- Manual lineup model/API/UI
- Lineup slot assignment
- Save/load lineup workflow

### Example Sprints

- Sprint 13: match planning backend
- Sprint 14: player availability workflow
- Sprint 15: manual lineup builder

### Completion Criteria

- A coach can create a match.
- A coach can track availability.
- A coach can build and save a manual lineup.
- The workflow is visible in the frontend.

## Phase 6: Soccer-Subber Integration Boundary

### Goal

Add a clean service boundary for future lineup assistance.

### Primary Outcomes

- Lineup suggestion request contract
- Lineup suggestion response contract
- Application port/interface
- Mock adapter
- Failure handling behavior
- Integration documentation
- Optional local service call later

### Example Sprints

- Sprint 16: contract and application port
- Sprint 17: mock adapter and failure handling
- Sprint 18: first soccer-subber service integration

### Completion Criteria

- SquadSync can request a lineup suggestion through an interface.
- The MVP works even when the external service is unavailable.
- The integration path is documented and testable.

## Phase 7: Event and Notification Readiness

### Goal

Prepare the core platform for event-driven notifications and AWS integration.

### Primary Outcomes

- Domain/application event definitions
- Outbox design if needed
- Local event persistence/logging
- Notification event contracts
- AWS notification architecture doc
- Initial notification proof of concept later

### Example Sprints

- Sprint 19: event model and outbox design
- Sprint 20: local notification event pipeline
- Sprint 21: AWS notification proof of concept

### Completion Criteria

- Important business events are captured.
- Notification responsibilities are separated from core use cases.
- AWS implementation has a clear integration path.

## Phase 8: Cloud Deployment and Portfolio Hardening

### Goal

Deploy a credible cloud-ready version and improve portfolio presentation.

### Primary Outcomes

- Containerized backend
- Hosted frontend
- Managed or hosted database path
- Deployment documentation
- CI/CD improvements
- Observability/logging improvements
- Demo script
- Architecture diagrams polished for portfolio review

### Example Sprints

- Sprint 22: containerization and deployment prep
- Sprint 23: cloud-hosted backend/frontend
- Sprint 24: portfolio demo polish

### Completion Criteria

- A reviewer can run or view the application.
- Architecture and deployment story are documented.
- The project demonstrates full-stack, cloud-aware engineering judgment.

## Phase 9: AI-Assisted Planning Summary

### Goal

Add a future AI-assisted summary capability after the core workflow and service boundary are stable.

### Primary Outcomes

- Summary generation use case
- Safe prompt/context assembly
- Reviewable explanation output
- Optional AIF-aligned implementation path

### Example Sprints

- Sprint 25: summary use-case design
- Sprint 26: local mock summary workflow
- Sprint 27: AI integration proof of concept

### Completion Criteria

- Lineup or planning summaries can be generated from structured platform data.
- The feature is explainable, testable, and separated from core lineup persistence.

## Sprint Planning Rules

Each sprint should define:

- Goal
- Context
- Scope
- Non-goals
- Expected files/areas
- Acceptance criteria
- Validation steps
- Follow-up issues

Each sprint should avoid:

- broad product expansion
- unapproved architecture changes
- mixing backend, frontend, infra, and cloud work unless the sprint is explicitly integration-focused
- implementation without a reviewable branch/PR

## Branched Conversation Strategy

For each new ChatGPT thread, start from the ChatGPT GitHub profile:

```text
We are working on SquadSync.
Reference docs:
- README.md
- AGENTS.md
- docs/planning/project-roadmap.md
- docs/planning/mvp-scope.md
- docs/architecture/system-overview.md
- docs/architecture/domain-model.md
- docs/agentic-workflow/README.md
- docs/agentic-workflow/tools/chatgpt-github/README.md
- docs/agentic-workflow/tools/chatgpt-github/branch-thread-workflow.md

Current phase:
[phase name]

Current sprint:
[sprint name]

Goal:
[one clear goal]

Non-goals:
[what should not be changed]
```

This keeps each branch focused while preserving context from the main planning track.

## Current Next Step

Complete Phase 0 operational harness hardening.

Phase 1 begins only after the agentic workflow architecture and ChatGPT GitHub/Codex CLI operational profiles are merged.
