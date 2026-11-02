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

## Quality checks and local preview

```powershell
pnpm format:check
pnpm lint
pnpm typecheck
pnpm test
pnpm build
pnpm start
```

Run these checks from `apps/web` in any editor or terminal. Use `pnpm format` to apply formatting before committing; `format:check` reports differences without changing files. Prettier owns formatting (two spaces, double quotes, semicolons, trailing commas, 100-column wrapping and LF endings), including CSS with Tailwind directives. It preserves utility-class order. ESLint uses the Next.js Core Web Vitals and TypeScript presets, with formatting rules disabled through `eslint-config-prettier`; warnings fail the lint gate. TypeScript errors and test failures are reported by their own commands.

`pnpm test` runs Vitest once with React Testing Library and jsdom. The representative shell test checks keyboard access to the skip link, its focusable main-content target, and the current navigation link. It requires no API or database. jsdom does not verify browser fragment-focus movement, CSS visibility or responsive layout; retain the browser checks below. Run `pnpm exec vitest` for watch mode while authoring tests.

The [Web CI workflow](../../.github/workflows/web-ci.yml) runs on pull requests and pushes to `main` that change `apps/web/**` or the workflow itself. It reads the exact Node and pnpm versions from `.node-version` and `package.json`, installs with `pnpm install --frozen-lockfile`, then runs format check, lint, typecheck, tests and build in that order. Each check has its own step. It needs no services or secrets and performs no deployment. The API workflow remains independent.

`typecheck` generates Next.js route types before strict TypeScript compilation, including on a fresh checkout. `build` produces the production build; `start` previews it at <http://127.0.0.1:3000> using loopback binding. This is a local preview, not deployment authorization.

Check both a desktop and narrow phone viewport, keyboard Tab navigation, the visible focus indicator, and the first “Skip to content” link. Confirm that the page fits without horizontal scrolling and that the Team-selection placeholder is non-interactive.

While the server runs, verify the listener in another PowerShell terminal (adjust the port if changed):

```powershell
Get-NetTCPConnection -State Listen -LocalPort 3000 |
  Select-Object LocalAddress, LocalPort, OwningProcess
```

Expect `127.0.0.1`, not `0.0.0.0` or `::`. Keep the app local: do not change the hostname, add a tunnel/proxy, or expose it on a LAN. Later API integration must preserve the Development-only API boundary in [ADR 0007](../../docs/adr/0007-web-foundation-architecture.md#local-development-boundary); the API is not started or reconfigured by these commands.

Generated OpenAPI types, the API client, and TanStack Query belong to [#166](https://github.com/bbubb/squadsync/issues/166); the owner-authored Team selector belongs to [#167](https://github.com/bbubb/squadsync/issues/167).

## Source layout

- `src/app/`: root route, layout, metadata, and Tailwind styling tokens.
- `src/components/workspace-shell.tsx`: shared header, workspace navigation, main landmark, and footer. The single current navigation item needs no mobile menu; it becomes a compact bar on narrow screens.
- `src/features/workspace/workspace-landing.tsx`: presentational workspace landing content, ready for the later Team-selection slice.

These are Server Components without client state or fetching. Use the existing layout and feature boundary as the starting point for future UI work; create additional abstractions only when actual behavior calls for them.

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
