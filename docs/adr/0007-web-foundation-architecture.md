# ADR 0007: Web Foundation and API Integration

## Status

Proposed for Phase 4 under [#158](https://github.com/bbubb/squadsync/issues/158), pending owner approval. This ADR establishes technical direction; implementation is separately issue-backed.

## Context

SquadSync has a Development-only ASP.NET Core API and no web application scaffold. Phase 4 introduces a coach-facing web client while preserving the backend as the authoritative business and persistence boundary.

## Decision

### Application stack

- Use Next.js App Router, React, and strict TypeScript in `apps/web/`.
- Target Next.js 16 and Node.js 26 LTS, with pnpm and a committed lockfile. Pin validated runtime and package-manager versions during scaffolding. Node.js 24 LTS remains a compatibility fallback if a concrete issue is found.
- Organize code by feature, with shared components where reuse is meaningful. Use Next.js routing and layouts; choose Server or Client Components according to actual rendering and interaction needs.

### API and state

- Derive HTTP types from the ASP.NET Core OpenAPI contract with `openapi-typescript`; use `openapi-fetch` for typed requests. The scaffold must establish a reproducible regeneration workflow without requiring a live backend for every frontend CI build.
- Use TanStack Query for remote data, mutations, and cache invalidation. Keep transient interaction and form state local where appropriate.
- Treat frontend validation as user feedback, not a replacement for backend domain rules or authorization.

### Interface foundation

- Use Tailwind CSS, reusable styling tokens, accessible component primitives (including shadcn/ui), and icons such as Lucide as useful starting tools.
- Component source may be adapted, composed, or replaced with custom SquadSync components when interaction quality, identity, or maintainability warrants it. Do not confuse avoiding unnecessary framework complexity with limiting visual design.
- Keep screen structure, navigation models, and design-stage placeholders in [UX notes](../product/ux-notes.md), not in this technology decision.

### Local development boundary

- Keep the web dev server and API bound to trusted loopback interfaces. Configure an explicit local API base URL and a narrowly scoped, Development-only ASP.NET Core CORS policy for browser calls across local ports.
- Start with direct browser-to-API HTTP calls; do not introduce Next.js management proxies, Route Handlers, Server Actions, or rewrites without a concrete need and reviewed security boundary.
- Verify that anonymous management endpoints remain unavailable outside Development. CORS does not authenticate callers or prevent direct network requests. No public hosting of this management UI is authorized.

### Quality baseline

- Establish explicit lint, format, type-check, test, and production-build commands. Use ESLint, Prettier, Vitest, and React Testing Library where their respective checks apply.
- Add a lightweight frontend CI gate once those scripts exist. Validate a real local API journey separately when integration is available; add browser automation when it provides durable coverage.
- Record actual commands and dependency versions in the web area's setup documentation after they are verified.

## Alternatives and trade-offs

- **React with Vite:** a sound web-only approach; Next.js was chosen for its routing/layout model and extensible web structure, not because we need a second backend. Neither approach automatically creates a native mobile app; the API is the cross-platform reuse boundary.
- **Handwritten DTOs or Axios:** workable, but generated OpenAPI types reduce duplication and contract drift. Their regeneration workflow needs explicit maintenance.
- **Next.js proxy/BFF:** potentially useful after an authenticated design exists; it would add an unnecessary access boundary now.
- **Node.js 24 LTS:** viable and mature, but 26 is now Active LTS. Actual compatibility takes precedence over release novelty.

## Consequences

- The frontend is responsible for presentation, navigation, and interaction; ASP.NET Core remains responsible for authoritative business behavior.
- Additional API reads, local CORS configuration, type-generation plumbing, and CI require their own scoped implementation work.
- UX exploration may consider a coherent application shell beyond the immediate functional slice, while the running MVP delivers only approved, honestly represented capabilities.
- This ADR does not change API behavior, implement authentication, or authorize deployment.

## References

- [Phase 4 planning](https://github.com/bbubb/squadsync/issues/157)
- [Documentation gate](https://github.com/bbubb/squadsync/issues/158)
- [MVP scope](../planning/mvp-scope.md)
- [System overview](../architecture/system-overview.md)
- [ADR 0006](0006-represented-person-identity-api-contract.md)
- [UX notes](../product/ux-notes.md)
