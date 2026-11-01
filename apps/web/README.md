# SquadSync Web

The local SquadSync workspace shell uses Next.js App Router, React, strict TypeScript, and Tailwind CSS. It has a responsive layout, semantic navigation, and a keyboard skip link. Team selection is clearly marked as coming next; the shell does not fetch or display Teams.

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

Open <http://127.0.0.1:3000>. Stop with `Ctrl+C`. The development script explicitly binds to IPv4 loopback; keep that hostname. If the port is busy, stop the existing process or use `pnpm dev --port 3001`, then use the port reported by Next.js.

The shell needs no API, PostgreSQL, environment variables, remote fonts, or credentials. No API client, proxy, Route Handlers, Server Actions, or management endpoint is introduced here.

## Validate and preview a build

```powershell
pnpm typecheck
pnpm build
pnpm start
```

`typecheck` generates Next.js route types before strict TypeScript compilation, including on a fresh checkout. `build` produces the production build; `start` previews it at <http://127.0.0.1:3000> using loopback binding. This is a local preview, not deployment authorization.

Check both a desktop and narrow phone viewport, keyboard Tab navigation, the visible focus indicator, and the first “Skip to content” link. Confirm that the page fits without horizontal scrolling and that the Team-selection placeholder is non-interactive.

While the server runs, verify the listener in another PowerShell terminal (adjust the port if changed):

```powershell
Get-NetTCPConnection -State Listen -LocalPort 3000 |
  Select-Object LocalAddress, LocalPort, OwningProcess
```

Expect `127.0.0.1`, not `0.0.0.0` or `::`. Keep the app local: do not change the hostname, add a tunnel/proxy, or expose it on a LAN. Later API integration must preserve the Development-only API boundary in [ADR 0007](../../docs/adr/0007-web-foundation-architecture.md#local-development-boundary); the API is not started or reconfigured by these commands.

Formatting, linting, component-test tooling, and web CI belong to [#165](https://github.com/bbubb/squadsync/issues/165). Those commands are not available yet. Generated OpenAPI types, the API client, and TanStack Query belong to [#166](https://github.com/bbubb/squadsync/issues/166); the owner-authored Team selector belongs to [#167](https://github.com/bbubb/squadsync/issues/167).

## Source layout

- `src/app/`: root route, layout, metadata, and Tailwind styling tokens.
- `src/components/workspace-shell.tsx`: shared header, workspace navigation, main landmark, and footer. The single current navigation item needs no mobile menu; it becomes a compact bar on narrow screens.
- `src/features/workspace/workspace-landing.tsx`: presentational workspace landing content, ready for the later Team-selection slice.

These are Server Components without client state or fetching. Use the existing layout and feature boundary as the starting point for future UI work; create additional abstractions only when actual behavior calls for them.

## Compatibility notes

Tailwind **3.4.19** is pinned for this scaffold. During validation, Tailwind 4.3.3's stylesheet resolver returned a null-byte-escaped path for the `#` in this Windows checkout's `C#` directory, failing both Turbopack and Webpack builds. Tailwind 3 uses the standard PostCSS directives and avoids that resolver path. Revisit a Tailwind 4 upgrade separately once it can build from this checkout; Node 26 and Next.js 16 remain the approved targets.

`next.config.ts` scopes Turbopack to this web app and disables automatic agent-rule generation so `next dev` preserves repository-owned `AGENTS.md` instructions.

## References

- [Web Foundation ADR](../../docs/adr/0007-web-foundation-architecture.md)
- [UX Notes](../../docs/product/ux-notes.md)
- [System Overview](../../docs/architecture/system-overview.md)
- [MVP Scope](../../docs/planning/mvp-scope.md)
- [Coding Standards](../../docs/agentic-workflow/workflow/coding-standards.md)
