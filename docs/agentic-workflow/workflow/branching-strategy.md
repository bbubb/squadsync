# Branching Strategy

## Purpose

This document defines SquadSync's branch and pull request strategy.

The goal is to keep `main` stable, avoid long-running divergence, and make human/agent work reviewable.

## Strategy

SquadSync uses a trunk-based GitHub Flow style:

```text
main = trunk
short-lived issue branches = reviewable work
pull requests = review gate
merge to main = accepted state
```

## Rules

- Keep `main` buildable and reviewable.
- Create short-lived branches from `main` for each focused issue or task.
- Prefer one issue-backed branch per PR.
- Avoid long-running `develop`, phase, or mega-feature branches.
- Merge only through reviewed PRs.
- Keep PRs small enough for a human to understand quickly.
- Do not mix unrelated docs, backend, frontend, and infrastructure work unless the issue explicitly scopes an integration change.

## Branch Naming

Recommended patterns:

```text
docs/<short-topic>
feature/<short-topic>
fix/<short-topic>
chore/<short-topic>
```

Examples:

```text
docs/finalize-phase-0-standards
feature/backend-scaffold
fix/lineup-slot-validation
chore/update-ci-dotnet
```

## Pull Request Expectations

Each PR should include:

- linked issue
- scope
- non-goals
- acceptance criteria status
- validation performed
- known limitations
- follow-up issues if needed

## Agent Guidance

Agents should not create broad branches that attempt to complete multiple unrelated issues.

If a task expands, the agent should stop, report scope drift, and suggest follow-up issues instead of silently broadening the PR.

## Phase 1 Implication

Once backend code exists, GitHub Actions should protect `main` with restore/build/test validation before application PRs are merged.
