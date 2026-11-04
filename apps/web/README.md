# SquadSync Web

The local SquadSync workspace shell uses Next.js App Router, React, strict TypeScript, and Tailwind CSS. It has a responsive layout, semantic navigation, and a keyboard skip link. The workspace loads stored Teams through the typed API query. Each Team links to `/teams/[teamId]`, displaying its real name and ID with an All Teams return path. Team creation and roster workflows are not implemented in the web UI.

## Prerequisites

- Node.js **26.13.0** (also pinned in `.node-version` and `package.json`).
- pnpm **12.16.1** (pinned in `package.json`). With the required Node version active, install it using `npm install --global pnpm@12.16.1` if needed.

Verify `node --version` and `pnpm --version` before installing. `.npmrc` enforces the declared engines. Use a version manager or the [official Node.js download](https://nodejs.org/en/download); a system-wide runtime change is not required.

## Install and run

From the repository root:

```powershell
cd apps/web
pnpm install --frozen-lockfile
pnpm dev
```

Open <http://127.0.0.1:3000>. Stop with `Ctrl+C`. The development script explicitly binds to IPv4 loopback; keep that hostname. For API browser calls, keep port 3000 available: the API permits only the exact origin `http://127.0.0.1:3000`. A shell-only run on port 3001 has no API CORS permission; do not widen the policy to accommodate it.

Rendering the shell needs no API, PostgreSQL, remote fonts, or credentials. Loading real Teams requires the local Development API and its PostgreSQL database. Browser requests go directly to the API without a web proxy. The default loopback API URL needs no environment override.

For a manual cross-origin API check, start the API's loopback `local` launch profile and follow the [API CORS and listener verification](../api/README.md#local-browser-cors-and-listener-verification). It includes listener inspection and a browser-console Team-list GET from this origin. CORS is not authentication or a network access control; anonymous management routes remain Development-only. Opening the workspace now automatically reads `GET /api/teams`.

## Quality checks and local preview

```powershell
pnpm format:check
pnpm lint
pnpm api:check
pnpm typecheck
pnpm test
pnpm build
pnpm start
```

Run these checks from `apps/web` in any editor or terminal. Use `pnpm format` to apply formatting before committing; `format:check` reports differences without changing files. Prettier owns formatting (two spaces, double quotes, semicolons, trailing commas, 100-column wrapping and LF endings), including CSS with Tailwind directives. It preserves utility-class order. ESLint uses the Next.js Core Web Vitals and TypeScript presets, with formatting rules disabled through `eslint-config-prettier`; warnings fail the lint gate. TypeScript errors and test failures are reported by their own commands.

`pnpm test` runs Vitest once with React Testing Library and jsdom. Shell and Team journey tests check keyboard access, loading, empty results, failure/retry, distinct Team links, route-selected identity with fresh caches, and malformed/unknown IDs. Fetch is mocked; tests resolve the actual Team page with route params but do not run a browser router. It requires no API or database. jsdom does not verify browser fragment-focus movement, CSS visibility or responsive layout; retain the browser checks below. Run `pnpm exec vitest` for watch mode while authoring tests.

The [Web CI workflow](../../.github/workflows/web-ci.yml) runs on pull requests and pushes to `main` that change `apps/web/**` or the workflow itself. It reads the exact Node and pnpm versions from `.node-version` and `package.json`, installs with `pnpm install --frozen-lockfile`, then runs format check, lint, generated API type check, typecheck, tests and build in that order. Each check has its own step. It needs no services or secrets and performs no deployment. The API workflow remains independent.

`typecheck` generates Next.js route types before strict TypeScript compilation, including on a fresh checkout. `build` produces the production build; `start` previews it at <http://127.0.0.1:3000> using loopback binding. This is a local preview, not deployment authorization.

Check both a desktop and narrow phone viewport, keyboard Tab navigation, the visible focus indicator, and the first “Skip to content” link. Confirm that the page fits without horizontal scrolling and that Team cards and retry/return controls remain usable. Check long Team names and distinct IDs for Teams sharing a name.

While the server runs, verify the listener in another PowerShell terminal (adjust the port if changed):

```powershell
Get-NetTCPConnection -State Listen -LocalPort 3000 |
  Select-Object LocalAddress, LocalPort, OwningProcess
```

Expect `127.0.0.1`, not `0.0.0.0` or `::`. Keep the app local: do not change the hostname, add a tunnel/proxy, or expose it on a LAN. Later API integration must preserve the Development-only API boundary in [ADR 0007](../../docs/adr/0007-web-foundation-architecture.md#local-development-boundary); the API is not started or reconfigured by these commands.

Generated OpenAPI types, the API client, and TanStack Query were introduced by [#166](https://github.com/bbubb/squadsync/issues/166); the Team-selection journey is scoped by [#167](https://github.com/bbubb/squadsync/issues/167).

## Manual Team journey smoke check

Before final approval, Brandon must perform this environment-dependent check:

1. Follow the [API PostgreSQL setup and demo-seed instructions](../api/README.md#local-postgresql-smoke-check). Use existing local Teams or the documented explicit demo seed; do not delete existing data. Start the API with its `local` launch profile in Development.
2. Run `pnpm dev` here, then inspect both listeners:

   ```powershell
   Get-NetTCPConnection -State Listen -LocalPort 3000,5050 |
     Select-Object LocalAddress, LocalPort, OwningProcess
   ```

   Both must report `127.0.0.1`. Keep the exact web origin `http://127.0.0.1:3000`; do not widen CORS or bind to a LAN address.

3. Open that origin. Observe stored Team names and IDs matching `GET http://127.0.0.1:5050/api/teams`. Select a Team with the keyboard, verify its URL, name and ID, then reload and confirm the same identity. Use All Teams, browser Back and Forward to verify return/navigation behavior and the Workspace current-page indication.
4. Open `/teams/not-a-team` and `/teams/00000000-0000-0000-0000-000000000000` (assuming that ID is absent). Expect an invalid-link or unavailable-Team message and a working return path.
5. Stop the API and reload the workspace. Observe loading followed by failure and Try again, rather than an empty collection. Restart the API, retry, and confirm recovery. Check the true empty state only against an already empty local database; do not erase records to manufacture it.
6. Check desktop and narrow phone viewports, long names, Tab/Enter navigation, visible focus, the skip link, and absence of horizontal scrolling. Confirm that there are no pretend create/roster actions.

Agent environment observation (2026-11-03): no listener observations were returned for ports 3000/5050/5432. Docker inspection was denied in the sandbox, and the browser access host/native pipe was unavailable. The real browser to API to PostgreSQL journey and visual/mobile checks were **not demonstrated**; they remain Brandon's checkpoint before final approval. Mocked tests do not substitute for this smoke check.

## API contract and local configuration

`openapi/squadsync.json` is a reviewed snapshot of Development Swagger. `src/lib/api/schema.d.ts` is generated with pinned `openapi-typescript` **7.13.0**; do not edit it or handwrite matching DTOs. The Team collection is `GET /api/teams`, returning an array of `TeamResponse` with required non-null `id` (UUID string) and `name` (string).

For an intentional contract update, start the current API from the repository root:

```powershell
dotnet run --project apps/api/src/SquadSync.Api --launch-profile local
```

In another terminal, from `apps/web`:

```powershell
pnpm api:refresh
# Review openapi/squadsync.json against the approved API change.
pnpm api:generate
pnpm api:check
```

Refresh fetches `/swagger/v1/swagger.json` explicitly; Swagger needs no database. Stop for owner review if the live contract differs from the approved endpoint. Commit the reviewed snapshot and generated types together. Generation and `api:check` read only the snapshot and reject external references. CI, tests, typecheck and build never refresh Swagger or need a running API/database. A successful live refresh after API PR #178 was verified on 2026-11-03; only TeamResponse required/nullability metadata changed.

For #185 on 2026-11-04, the current API at `b938492` was built with `dotnet build apps/api/src/SquadSync.Api --no-restore --disable-build-servers -m:1`, then run directly from `apps/api/src/SquadSync.Api` using `dotnet bin/Debug/net10.0/SquadSync.Api.dll`, with `ASPNETCORE_ENVIRONMENT=Development` and `ASPNETCORE_URLS=http://127.0.0.1:5050`. From `apps/web`, `pnpm api:refresh` fetched actual Development Swagger, then `pnpm api:generate` regenerated the types after review. The diff contained only PR #190's membership/roster response corrections: required fields, non-null names/role/status, and a required but nullable `jerseyNumber`. The API was stopped after refresh; no database was needed.

The single setting `NEXT_PUBLIC_API_BASE_URL` defaults to `http://127.0.0.1:5050`. It is a public, build-time browser setting, never a place for secrets. To override it, set it in your terminal before refresh, dev or build, for example `$env:NEXT_PUBLIC_API_BASE_URL = "http://127.0.0.1:5050"`. Only HTTP loopback origins are accepted, without credentials, paths, query strings or fragments. Changing the API origin does not widen its CORS permission for the web origin `http://127.0.0.1:3000`.

`openapi-fetch` **0.17.0** uses generated `paths`, omits credentials and refuses redirects. Non-2xx responses throw `ApiError` with HTTP status; transport failures throw a useful local-API error with the original cause. Failed requests must not become empty collections. Mocked client tests cover successful and empty Team collections, HTTP/transport errors and local URL restrictions.

TanStack Query **5.105.3** owns remote data. The Server Component root layout imports the small Client Component `QueryProvider`, which maintains one cache per mounted provider. Generated types describe the contract; the client executes the typed HTTP operation; a feature query puts its result in that cache; a component reads query loading, error and data state. Keep transient UI state local and represent selected Team context in the route; avoid copying Team data into component state. Provider tests verify cache retention across renders and isolation between mounts.

Client Components under that provider can call `useTeams()` from `src/features/teams/use-teams.ts`. It returns the Query result, including `data`, `isPending`, `isError` and `error`, for the generated Team collection. Reads share `teamsQueryKey` (`["teams"]`) and remain fresh for 30 seconds; later mounts can reuse the cached collection. The query passes cancellation to the typed client, propagates request failures, and rejects a missing response body instead of substituting `[]`. A real empty collection remains a successful result. The focused hook tests mock HTTP at `fetch`, exercise success/empty/error results through the real typed client and verify cache sharing. The `TeamCollection` Client Component reads this hook for both the workspace list and route-selected identity. The URL owns the selected ID, which is matched against the cached collection without copying remote data into local state. No transient interaction state is needed. Unknown IDs show a return instruction; malformed route IDs are rejected before fetching. Workspace navigation only marks `/` as the current page.

## Source layout

The Team-scoped read hooks `useTeamMemberships(teamId)` and `useTeamRoster(teamId)` live beside `useTeams` under `src/features/teams/`. They use the same typed client, 30-second stale time, and Query result states. Their keys are `["teams", teamId, "memberships"]` and `["teams", teamId, "roster"]`: including `teamId` isolates each Team's cached data, while the final segment separates the collections. Switching IDs starts with pending data for an uncached Team instead of displaying the previous Team's collection; returning within the stale time reuses that Team's cache. The hooks pass the fetch abort signal so Query can cancel requests that no longer have observers.

TanStack Query owns these remote collections and their loading/error state; future components should keep only transient UI interactions locally. As with `useTeams`, successful `[]` means the API returned an empty collection. HTTP/network failures and missing bodies propagate as errors instead of being converted to `[]`. Fetch-mocked hook tests cover represented-person/role/status/jersey fields (including null jerseys), empty results, failures, Team switching, cache reuse, and cancellation. These hooks add no roster UI or write operations.

- `src/app/`: root route, layout, metadata, and Tailwind styling tokens.
- `src/app/query-provider.tsx`: Client Component providing TanStack Query to interactive descendants.
- `src/lib/api/`: generated schema, local base URL configuration and typed HTTP client.
- `openapi/` and `scripts/openapi.mjs`: committed contract and explicit refresh/generate/check workflow.
- `src/components/workspace-shell.tsx`: shared header, workspace navigation, main landmark, and footer. The small `workspace-navigation.tsx` Client Component reads the pathname for accurate `aria-current`. The single navigation item needs no mobile menu; it becomes a compact bar on narrow screens.
- `src/features/workspace/workspace-landing.tsx`: Server Component landing content with the interactive Team collection.
- `src/features/teams/team-collection.tsx`: Team list, selected identity, loading, empty and retry presentation.
- `src/app/teams/[teamId]/page.tsx`: route-backed Team context and return navigation.
- `src/features/teams/use-teams.ts`: typed Team collection query using the shared remote cache.

The shell and workspace landing remain Server Components. Their Client Component descendants handle pathname and remote-query interaction. Future roster UI can compose into the selected Team route under the same shell.

## Compatibility notes

Quality tools are pinned in `package.json` and the lockfile. ESLint **9.39.5** matches the peer ranges of the React, import and accessibility plugins shipped by `eslint-config-next` **16.4.3**. Although ESLint 9 is marked deprecated by the registry, ESLint 10 currently falls outside those plugin ranges; upgrade them together when compatible. Prettier **3.9.10**, Vitest **5.0.5**, jsdom **30.1.3**, and React Testing Library **16.3.3** were validated with the pinned runtime.

Tailwind CSS and `@tailwindcss/postcss` are pinned to **4.3.3**. Global CSS imports Tailwind with `@import "tailwindcss"` and defines styling tokens with `@theme`; source detection is automatic. The dedicated PostCSS plugin handles vendor prefixing, so a separate Autoprefixer dependency and Tailwind JavaScript configuration are unnecessary.

Use a checkout path without `#`, such as `D:\Utilities\Projects\Csharp\squadsync`. The previous Windows `C#` path caused a reproduced null-byte pathname error in Tailwind 4's stylesheet resolver. Identical worktree tests built successfully outside that path and failed again when `#` was present. The scaffold now uses Tailwind 4 from the `Csharp` checkout.

`next.config.ts` scopes Turbopack to this web app and disables automatic agent-rule generation so `next dev` preserves repository-owned `AGENTS.md` instructions.

## References

- [Web Foundation ADR](../../docs/adr/0007-web-foundation-architecture.md)
- [UX Notes](../../docs/product/ux-notes.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [MVP Scope](../../docs/planning/mvp-scope.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
