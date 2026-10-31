# Web Agent Instructions

## Status

Proposed Phase 4 operating guidance. The web application is not yet scaffolded; follow this file only alongside an accepted [frontend ADR](../../docs/adr/0007-web-foundation-architecture.md) and an authorized, ready implementation issue. Do not infer implementation approval from this document.

## Purpose

`apps/web/` is SquadSync's coach-facing web client. These instructions apply to human contributors and **any** authorized implementation agent. Tool-specific commands, skills, hooks and orchestration belong to their respective profiles under `docs/agentic-workflow/tools/`, not here.

## Context Routing

Start with the active issue, root `AGENTS.md`, `CONTRIBUTING.md` and this scoped file. Then load only the sources relevant to the task:

- [Frontend architecture ADR](../../docs/adr/0007-web-foundation-architecture.md) for approved stack, client/API boundary, and local security conditions.
- [UX notes](../../docs/product/ux-notes.md) and [brand notes](../../docs/product/brand-notes.md) for user-facing layout, navigation and visual changes.
- [MVP scope](../../docs/planning/mvp-scope.md), [system overview](../../docs/architecture/system-overview.md) and [domain model](../../docs/architecture/domain-model.md) when business or API boundaries are involved.
- [Testing strategy](../../docs/agentic-workflow/workflow/testing-strategy.md) and [coding standards](../../docs/agentic-workflow/workflow/coding-standards.md) for shared quality expectations.

Consult `PLANS.md` for a genuinely complex or cross-cutting implementation, not every component. Do not preload all documents for a small UI edit.

## Architecture Rules

- Use Next.js App Router, React and strict TypeScript under a feature-oriented organization once the accepted ADR and scaffold establish the concrete structure.
- Keep routing/layout composition straightforward; implement interactive roster/forms in focused Client Components where needed.
- Consume the ASP.NET Core API through a single, typed HTTP client and generated OpenAPI types. Do not hand-copy backend DTOs or manually modify generated contracts.
- Use TanStack Query for server data and mutations; React/component/form state for transient UI. Do not create competing caches or broad state abstractions without a concrete need.
- Keep roster eligibility, membership constraints, and other authoritative rules in the API's Application/Domain. UI validation and availability of buttons must never stand in for backend enforcement.
- Share genuinely reusable presentation primitives; avoid premature libraries, monolithic dashboard components, and generic abstractions added merely for architectural appearance.

## UX and Accessibility

- Implement the approved local workspace → Team → roster → Add Player → Player details workflow first.
- Support both Select Existing Person and Create New Person; optional person-level PlayerProfile creation/editing is explicit, never automatic.
- Provide meaningful loading, empty, error, validation and success states, and preserve useful navigation on desktop and narrow screens.
- Use semantic controls, visible keyboard focus, accessible names, sufficient contrast, and touch-usable interactions. A desktop side panel must not make Player details inaccessible on a narrow screen.
- Maintain styling through reusable tokens and consistent components rather than hardcoded, one-off visual decisions.

## Local Security Boundary

- The existing management API is anonymous and **Development-only**, intended for trusted loopback use. Web code must not expose it through public hosting, a LAN-bound dev server, Next.js Route Handlers, Server Actions, proxy rewrites or other new access paths without an approved security design.
- Follow the ADR's explicit local API base URL and narrow CORS strategy once implemented. CORS is not authentication; a hidden or disabled frontend control is not authorization.
- Do not infer account identity, profile stewardship or owner permissions from `PersonId`, `TeamRole` or form input.
- Never commit credentials, personal demo data, or sensitive browser logs.

## Quality and Validation

- Scope tests to meaningful UI behavior, data error paths, and integration boundaries. Prefer readable tests over snapshots of internal implementation detail.
- Follow the scaffold's actually committed commands for formatting, lint, strict type checks, tests and production build; these commands must be documented in `apps/web/README.md` and CI before claiming they were run. This placeholder document intentionally does not invent commands.
- Review generated types when API contracts change and report whether the local end-to-end API smoke check was performed.
- Keep PRs issue-backed, focused, reviewable and explicit about validation evidence, missing coverage and deferred work.

## Stop Conditions

Stop and return to the owner/active issue when:

- the frontend ADR is unaccepted or the implementation issue is not ready;
- the workflow requires unavailable API contracts or unapproved backend changes;
- the local-only anonymous API boundary cannot be verified;
- the design requires authentication, authorization, account ownership, public hosting, organization management, or another deferred product area;
- a dependency or UI abstraction would materially expand scope without demonstrated need.

Tool-specific implementation guidance belongs only in its tool profile or native skill. These web rules remain agent-agnostic.
