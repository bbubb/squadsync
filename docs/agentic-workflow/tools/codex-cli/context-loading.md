# Codex CLI Context Loading

## Purpose

This document defines the context Codex CLI should load before beginning implementation work.

The goal is to make each Codex session repo-driven and repeatable.

## Required Context Order

Before modifying files, Codex CLI should read:

1. `AGENTS.md`
2. `README.md`
3. `docs/planning/project-roadmap.md`
4. `docs/planning/mvp-scope.md`
5. `docs/architecture/system-overview.md`
6. `docs/architecture/domain-model.md`
7. relevant ADRs in `docs/adr/`
8. `docs/agentic-workflow/README.md`
9. relevant policy/workflow/spec docs under `docs/agentic-workflow/`
10. the relevant tool profile under `docs/agentic-workflow/tools/codex-cli/`
11. the current GitHub issue

## Task-Specific Context

For each issue, Codex should identify:

- affected project area
- relevant docs and ADRs
- expected files or folders
- acceptance criteria
- validation gates
- stop conditions

## Context Conflicts

If docs conflict, Codex should stop and report the conflict instead of choosing silently.

If the issue conflicts with documented architecture, the documented architecture wins until the human owner approves a change through an ADR or workflow update.

## Context Minimization

Codex should load enough context to act correctly but should not expand the task by reading unrelated future phases unless the issue requires it.
