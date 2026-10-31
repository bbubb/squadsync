# ADR 0007: Web Foundation and Local API Boundary

## Status

Proposed for Phase 4 under [issue #158](https://github.com/bbubb/squadsync/issues/158). This ADR becomes accepted only after owner review and merge. It does not authorize frontend implementation by itself.

## Context

Phase 3 provides a tested ASP.NET Core API for represented People, Teams, Memberships, Rosters and optional person-level PlayerProfiles. Management controllers are anonymous and mapped only in Development. `apps/web/` is a placeholder. Phase 4 needs a professional, maintainable coach-facing UI without turning these local development endpoints into a publicly accessible application.

The web client consumes API contracts. It does not own persistent soccer business rules.

## Decision

### Framework and runtime

- Use Next.js App Router, React, and TypeScript with strict type checking in `apps/web/`.
- Prefer the current stable Next.js 16 line and Node.js 26 LTS; pin the chosen runtime major and exact package-manager version in the scaffold. Verify toolchain compatibility locally and in CI before considering the choice implemented. Node.js 24 LTS is a documented fallback only if an actual compatibility problem is found.
- Use pnpm with one committed lockfile for reproducible installs.
- Use Next.js layouts and routing for navigation. Keep interactive roster workflows in focused Client Components; do not turn Next.js into a second business/API backend.

### API contracts and data management

- Generate TypeScript HTTP contract types from the ASP.NET Core API's OpenAPI document using `openapi-typescript`. Use `openapi-fetch` as the typed HTTP client rather than hand-maintained copies of backend response models.
- The scaffold issue must define and verify a repeatable type-regeneration command. Generated types must be reviewable and buildable without requiring a live API during every frontend CI run; API contract changes require regenerating those types. Add drift detection when there is a reliable export mechanism, rather than inventing one in this ADR.
- Use TanStack Query for remote API reads, mutations and cache invalidation. Keep transient interface state (open drawer, selected tab, form draft) in local React state or the appropriate form helper. Do not duplicate remote server data in a second mutable global store.
- Keep frontend validation for usability; the ASP.NET Core Application/Domain remain authoritative for roster eligibility, membership rules, validation and persistence.

### Presentation and accessibility

- Use Tailwind CSS with semantic design tokens/CSS variables; use a small set of accessible shadcn/ui-style component primitives and Lucide icons where useful. Avoid a large design system, custom icon domain model or speculative asset pipeline.
- Organize implementation by user-facing feature with a small shared UI layer, not a maze of generic services and abstractions.
- Follow the current [UX notes](../product/ux-notes.md) for the local workspace/Team/roster journey, responsive details and form states. The UX pattern must remain usable by keyboard, touch and assistive technology.

### Local-only network and security boundary

- Run the web development server and ASP.NET API on loopback interfaces; use explicitly configured local origin(s) and a narrow ASP.NET Core Development-only CORS policy to permit the browser app to call the API across local ports.
- Browser components call the ASP.NET Core API directly via an explicitly configured local base URL. Do not add management proxy routes, Next.js Route Handlers, Server Actions or rewrites merely to avoid CORS.
- Verify loopback binding and that management endpoints are absent outside Development. CORS is a browser-origin policy, not authentication or a network firewall; localhost-only binding plus endpoint gating are distinct safeguards.
- No hosted or network-accessible anonymous management UI is authorized. Authentication, account identity, profile stewardship and authorization are deferred to a separate approved design. Development-only profile create/edit exposure does not establish permission policy.

### Quality and validation baseline

- Configure ESLint, Prettier, TypeScript checks, a production build and frontend tests as explicit, separately runnable package scripts. Do not assume `next build` runs the linter.
- Use Vitest and React Testing Library for behavior-focused components/hooks when appropriate. Start with a real local API integration smoke check once the connected journey exists; introduce browser automation only where it offers durable value.
- Add a lightweight web CI job for install, formatting, lint, type check, tests and build once the scaffold actually supplies these commands. PostgreSQL-backed checks remain separately opt-in.
- The exact commands and dependency versions belong to the scaffold and `apps/web/README.md`, then `apps/web/AGENTS.md` should refer to the verified commands.

## Trade-offs and alternatives

- **Plain React + Vite:** viable for a browser-only SPA, but Next.js offers established routing/layout conventions and an extensible web application structure without requiring us to use server-side management proxies. Next.js does not automatically make the source code a React Native mobile app; the HTTP API remains the reusable cross-platform boundary.
- **Handwritten API DTOs / Axios:** possible but risk type drift or extra wrapper code. Generated OpenAPI types with `openapi-fetch` are the narrower default; contract-generation workflow must be made repeatable.
- **Next.js management proxy / BFF:** may be useful in a later authenticated architecture, but adds an additional accessible boundary with no current need or approved security design.
- **Node 24 LTS:** a sound fallback, but 26 is now Active LTS for a new web application. Follow observed compatibility rather than upgrade for novelty.

## Consequences and deferred work

- Phase 4 includes a Development-only UI journey: select/create Team, view roster, add a player by selecting an existing Person or creating a new one, view Player details, and explicitly create/edit optional PlayerProfile attributes.
- Profile creation remains opt-in and person-level; it does not happen automatically when creating a Person, membership or roster entry.
- Any Team/Person lookup endpoint, local CORS configuration, OpenAPI export/type-generation support or CI changes require separately scoped implementation work and validation. This ADR alone changes no API or network behavior.
- Keep future account/organizational/other platform concepts out of the public MVP documentation.

## References

- [Planning gate #157](https://github.com/bbubb/squadsync/issues/157)
- [Documentation gate #158](https://github.com/bbubb/squadsync/issues/158)
- [MVP scope](../planning/mvp-scope.md)
- [System overview](../architecture/system-overview.md)
- [Domain model](../architecture/domain-model.md)
- [ADR 0006: represented people and authentication](0006-represented-person-identity-api-contract.md)
- [UX notes](../product/ux-notes.md)
- [Web instructions](../../apps/web/AGENTS.md)
