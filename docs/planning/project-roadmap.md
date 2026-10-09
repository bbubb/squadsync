# Project Roadmap

## Status

Active. Phase 0, Phase 1, and Phase 2 are complete. Phase 2 concluded through Sprint 6 under [#109](https://github.com/bbubb/squadsync/issues/109), establishing the MVP core domain, PostgreSQL persistence, the first Application use case, and explicit Development demo data. Phase 3 — Roster Management API — is active in planning through [Sprint 7 tracker #116](https://github.com/bbubb/squadsync/issues/116). Its architecture gate [#117](https://github.com/bbubb/squadsync/issues/117) is accepted through [PR #123](https://github.com/bbubb/squadsync/pull/123); issue #118 applies the Person vocabulary.

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

Phase 0 required the repository to contain:

- strategic project documentation
- a repo-owned agentic workflow architecture with usable ChatGPT GitHub and Codex CLI tool profiles
- engineering workflow standards for branching, testing, coding, validation, and review

### Deliverables

- README
- MVP scope document
- System overview
- Domain model
- ADRs for core decisions
- Agent instructions
- Agentic workflow architecture under `docs/agentic-workflow/`
- Generic policy, workflow, and task specification docs
- Branching strategy
- TDD-oriented testing strategy
- Coding standards
- ChatGPT GitHub tool profile
- Codex CLI operational profile
- Codex rules, skills, hooks, and subagents structure
- Symphony future/reference profile
- Agent-ready issue template
- Pull request review template
- Project roadmap
- Placeholder project areas for `apps/api`, `apps/web`, `infra`, and integrations

### Operational Harness Outcomes

Phase 0 added:

- layered `docs/agentic-workflow/` structure
- generic policy/workflow/spec layers
- ChatGPT GitHub tool profile
- Codex CLI tool profile
- Codex-native `rules/`, `skills/`, `hooks/`, and `subagents/` structure
- branching, testing, and coding standards
- validation gates and stop conditions
- agent-ready issue template
- PR template alignment
- future harness friction log
- professional root README and project-area pathing

### Completion Criteria

- Repository purpose is clear.
- MVP scope is defined.
- Architecture direction is documented.
- Domain model is documented.
- AI-assisted workflow is documented.
- GitHub issues can act as executable task records.
- ChatGPT GitHub can support planning, issue/PR creation, review, and closeout from repository-owned context.
- Codex CLI can load repository-owned context before implementation.
- Codex CLI has stable rules, skills, hooks, and subagent placeholder structure.
- Branching, testing, coding, validation, and stop-condition standards are documented.
- Tool-specific behavior is nested under `docs/agentic-workflow/tools/`.
- App pathing is stabilized around `apps/api` and `apps/web`.
- Phase 1 can begin without relying on hidden ChatGPT session context.

## Phase 1: API Foundation

### Goal

Create the API solution scaffold and local development baseline.

### Primary Outcomes

- ASP.NET Core solution under `apps/api/`
- Modular projects: API, Application, Domain, Infrastructure
- Unit and integration test projects
- Health endpoint
- Swagger/OpenAPI
- Serilog console logging
- PostgreSQL local development path
- Docker Compose baseline
- GitHub Actions build/test workflow

### Example Sprints

- Sprint 1: API scaffold
- Sprint 2: local database and infrastructure baseline
- Sprint 3: CI/build/test hardening — example only; not instantiated because the Phase 1 completion criteria were already met

### Completion Criteria

- API builds locally.
- Health endpoint works.
- Swagger is available in development.
- Project references enforce intended dependency direction.
- Basic CI validates restore/build/test.

## Phase 2: Core Domain and Persistence

### Goal

Implement the MVP domain model and persistence layer.

### Primary Outcomes

- Represented `Person` entity/model (renamed from Phase 2 `User` under ADR 0006)
- Team entity/model
- TeamMembership entity/model
- Constrained `TeamRole` value on each `TeamMembership` (one per membership)
- PlayerProfile model
- `RosterEntry` attached to a player `TeamMembership` for team-context data such as jersey number and roster status
- EF Core DbContext
- Entity configurations
- Initial migrations
- Seed data for development/demo
- Basic domain/application tests

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- team and represented-user domain model
- membership and constrained team-role model
- player profile and roster persistence

Authentication/account identity, `CoachProfile`, and statistics models are deferred. See the [domain model](../architecture/domain-model.md) and [persistence ADR](../adr/0005-ef-core-npgsql-persistence.md).

### Completion Criteria

- Core entities exist and persist.
- Relationships match `docs/architecture/domain-model.md`.
- Basic CRUD or use-case paths can be tested.
- Seed data supports a simple demo scenario.

## Phase 3: Roster Management API

### Goal

Expose usable API behavior for team and roster management.

### Primary Outcomes

- Team endpoints/use cases
- Roster endpoints/use cases
- Player profile endpoints/use cases
- Membership role assignment
- Validation
- Standard API responses/errors
- Integration tests for core flows

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- team management API
- roster/player profile API
- membership team-role API and validation

### Completion Criteria

- A coach can create a team.
- A coach can add/manage roster members.
- Player profile data can be created and updated.
- API behavior is validated by tests.

## Phase 4: Web Foundation

### Goal

Create the web application shell and connect it to APIs.

### Primary Outcomes

- Next.js + TypeScript app under `apps/web/`
- Feature-oriented structure
- Tailwind setup
- API service layer
- TanStack Query setup
- Dashboard layout
- Basic team/roster screens

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- web scaffold and app shell
- team dashboard
- roster management UI

### Completion Criteria

- Web app runs locally.
- Web app can call backend APIs.
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

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- match planning backend
- player availability workflow
- manual lineup builder

### Completion Criteria

- A coach can create a match.
- A coach can track availability.
- A coach can build and save a manual lineup.
- The workflow is visible in the web app.

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

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- contract and application port
- mock adapter and failure handling
- first soccer-subber service integration

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

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- event model and outbox design
- local notification event pipeline
- AWS notification proof of concept

### Completion Criteria

- Important business events are captured.
- Notification responsibilities are separated from core use cases.
- AWS implementation has a clear integration path.

## Phase 8: Cloud Deployment and Portfolio Hardening

### Goal

Deploy a credible cloud-ready version and improve portfolio presentation.

### Primary Outcomes

- Containerized API if appropriate
- Hosted web app
- Managed or hosted database path
- Deployment documentation
- CI/CD improvements
- Observability/logging improvements
- Demo script
- Architecture diagrams polished for portfolio review

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- containerization and deployment preparation
- cloud-hosted API/web
- portfolio demo polish

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

### Descriptive Sprint Examples

These examples describe possible work areas only. They do not assign sprint numbers or establish chronology.

- summary use-case design
- local mock summary workflow
- AI integration proof of concept

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
- mixing API, web, infra, and cloud work unless the sprint is explicitly integration-focused
- implementation without a reviewable branch/PR

## Conversation Context Strategy

Use the [Standard Main Thread Prompt](../agentic-workflow/tools/chatgpt-github/main-thread-workflow.md#standard-main-thread-prompt) to start or re-orient the project control-room discussion.

After a sprint or task is confirmed, use the [Standard Branch Thread Prompt](../agentic-workflow/tools/chatgpt-github/branch-thread-workflow.md#standard-branch-thread-prompt) for bounded execution, review, debugging, or closeout work.

The ChatGPT tool-profile documents own the prompt templates. This roadmap owns current phase and sprint direction; GitHub issues, Project, and pull requests own executable work state. Populate prompt fields from those sources during handoff instead of duplicating current state or prompt wording here.

## Current Next Step

Phase 0 is complete through [Issue #40](https://github.com/bbubb/squadsync/issues/40). Phase 1 Sprint 1 is complete through [Issue #11](https://github.com/bbubb/squadsync/issues/11) and child issues #44–#48.

Phase 1 Sprint 2 — Local PostgreSQL & Infrastructure Baseline — is complete through umbrella [#57](https://github.com/bbubb/squadsync/issues/57) and child issues [#58](https://github.com/bbubb/squadsync/issues/58)–[#61](https://github.com/bbubb/squadsync/issues/61). Sprint 2 established:

- minimal GitHub Actions restore/build/test validation for the .NET 10 API;
- local PostgreSQL through Docker Compose with loopback-only host binding, health checking, and a persistent named volume;
- database-independent `GET /health` liveness and PostgreSQL-backed `GET /health/ready` readiness;
- successful local validation of readiness failure and recovery when PostgreSQL is stopped and restarted;
- documented local setup, connection-string, health-check, credential, and volume workflows;
- continued separation between connectivity readiness and Phase 2 persistence modeling.

Sprint 2 deliberately did not introduce EF Core, a `DbContext`, domain tables, migrations, seed data, production/cloud infrastructure, or API containerization.

Observed Sprint 2 workflow friction is preserved in follow-up issues [#72](https://github.com/bbubb/squadsync/issues/72)–[#74](https://github.com/bbubb/squadsync/issues/74). Issues #73 and #74 are complete. Issue #72 remains open as deferred backlog maintenance and can be reconsidered when Phase 2 persistence makes repeated database-backed validation more valuable.

Phase 1 is complete. A separate Phase 1 Sprint 3 was not created because Sprints 1–2 and the completed workflow follow-ups already satisfy the Phase 1 completion criteria.

Phase 2 is complete through Sprints 3–6.

- Sprint 3 ([#82](https://github.com/bbubb/squadsync/issues/82)) established EF-independent `User` and `Team` domain entities, Infrastructure-owned EF Core/Npgsql persistence, the initial migration, and real PostgreSQL round-trip validation.
- Sprint 4 ([#94](https://github.com/bbubb/squadsync/issues/94)) added explicit `TeamMembership` with one constrained `TeamRole`, relationship/uniqueness enforcement, and restrictive deletion.
- Sprint 5 ([#102](https://github.com/bbubb/squadsync/issues/102)) separated person-level `PlayerProfile` from team-context `RosterEntry`, persisted both with additive migrations, and validated their one-to-one structural constraints against PostgreSQL.
- Sprint 6 ([#109](https://github.com/bbubb/squadsync/issues/109)) added the first real Application workflow, `AddPlayerToRoster`, using an Application-owned persistence contract implemented by Infrastructure. It also added explicit, Development-only, idempotent demo seeding for the complete Phase 2 model.

Phase 2 completion criteria are satisfied: core entities persist, relationships match the canonical domain model, an Application use-case path is testable without HTTP, and seed data supports a simple demo scenario.

Open maintenance issues [#72](https://github.com/bbubb/squadsync/issues/72), [#89](https://github.com/bbubb/squadsync/issues/89), and [#100](https://github.com/bbubb/squadsync/issues/100) remain deferred backlog and do not block the phase transition.

Phase 3 Sprint 7 is planned under [#116](https://github.com/bbubb/squadsync/issues/116). The completed review gate, [#117](https://github.com/bbubb/squadsync/issues/117), accepted [ADR 0006](../adr/0006-represented-person-identity-api-contract.md) for the person/account distinction and initial Development-only API boundary. Dependent issues #118–#122 implement and validate the agreed slice under that accepted decision.
