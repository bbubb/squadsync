# Frontend Instructions

Apply these instructions to frontend changes under `frontend/`.

## Target Architecture

Use a Next.js + TypeScript frontend organized by feature:

```text
frontend/
  src/
    app/
    components/
    features/
      teams/
      players/
      matches/
      lineups/
    lib/
    services/
    types/
```

## Implementation Defaults

- Use TypeScript for all production frontend code.
- Use Tailwind CSS for styling.
- Use TanStack Query for server-state fetching and mutation workflows.
- Centralize API calls in typed service modules.
- Prefer simple, composable components.
- Keep feature-specific components inside their feature folder.
- Keep shared UI components generic and reusable.

## UX Scope

The MVP frontend should support:

- Team dashboard
- Roster management
- Player profile management
- Match planning
- Manual lineup builder
- Future soccer-subber integration status/results

Do not build generic competition-platform UI or multi-sport configuration screens in the public MVP.

## State Management Rules

- Prefer local component state for local UI concerns.
- Use TanStack Query for server state.
- Avoid global context unless the state is genuinely cross-cutting.
- Avoid premature state frameworks.

## Safety

Do not expose private algorithm assumptions in UI copy. Use public-safe language such as “lineup suggestion,” “optimization service,” and “planning summary.”
