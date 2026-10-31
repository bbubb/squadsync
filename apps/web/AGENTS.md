# Web Agent Instructions

## Status and Scope

Phase 4 web-area guidance, pending acceptance of [ADR 0007](../../docs/adr/0007-web-foundation-architecture.md) and a ready implementation issue. The application is not scaffolded yet.

These are **tool-agnostic** instructions for contributors and authorized implementation agents. Tool-specific behavior belongs under `docs/agentic-workflow/tools/` or the associated tool-native assets.

## Context Routing

Start with the active issue, root `AGENTS.md`, `CONTRIBUTING.md`, and this file. Read task-relevant sources rather than automatically loading every document:

- [Frontend ADR](../../docs/adr/0007-web-foundation-architecture.md): framework, state, API integration, security boundary, and quality strategy.
- [UX notes](../../docs/product/ux-notes.md) and [brand notes](../../docs/product/brand-notes.md): navigation, design intent, accessibility, and presentation.
- [MVP scope](../../docs/planning/mvp-scope.md), [system overview](../../docs/architecture/system-overview.md), and [domain model](../../docs/architecture/domain-model.md): business and architectural context when needed.
- [Coding standards](../../docs/agentic-workflow/workflow/coding-standards.md) and [testing strategy](../../docs/agentic-workflow/workflow/testing-strategy.md): shared engineering expectations.

Read `PLANS.md` for complex or cross-cutting implementation work.

## Engineering Conventions

- Follow accepted architecture and established feature-oriented structure. Build cohesive, reusable components when justified; custom UI is welcome when it improves experience and remains maintainable.
- Use generated OpenAPI types and the approved API client. Do not reproduce backend DTOs or enforce authoritative business rules only in UI code.
- Keep remote data in the approved query layer and transient interaction state local when appropriate. Avoid competing caches or unnecessary abstractions.
- Preserve semantic HTML, keyboard navigation, accessible names, readable error/empty/loading states, and responsive behavior. Refer to UX notes for the actual screen journey rather than restating it here.
- Make behavior testable; add relevant component/integration tests with changes. Document actual lint, format, type-check, test, and build commands in the web README once scaffolded, and report only validation that ran.
- Keep PRs issue-backed, focused, and reviewable. Explain architectural trade-offs when introducing new dependencies or reusable primitives.

## Security and Stop Conditions

- Respect the [ADR's local-only management API boundary](../../docs/adr/0007-web-foundation-architecture.md#local-development-boundary). Do not expose anonymous management capabilities through deployment, LAN binding, proxying, or other new routes without approval.
- Stop if the issue needs missing API contracts, unapproved security or architectural changes, unavailable functionality, or a major scope expansion. Ask the owner rather than guessing.
- Never commit credentials or personal/sensitive demo data.

Do not add tool-specific prompts, rules, or orchestration here. This document defines project expectations independently of the agent used.
