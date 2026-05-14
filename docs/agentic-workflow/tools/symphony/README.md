# Symphony Tool Profile

## Purpose

This profile reserves a place for future Symphony-oriented workflow documentation.

Symphony is not implemented in SquadSync during Sprint 0.1.

## Current Status

Status: future/reference profile.

SquadSync's immediate implementation path is:

```text
ChatGPT + human owner -> GitHub issues/docs -> Codex CLI -> PR review
```

Symphony may be evaluated later as an orchestrator for issue-driven, multi-agent, or higher-concurrency workflows.

## Why This Exists

The agentic workflow architecture is designed so future tools can be added under `docs/agentic-workflow/tools/` without changing the generic policy, workflow, and spec layers.

A future Symphony profile could document:

- orchestration model
- issue tracker integration
- workspace model
- concurrency settings
- validation behavior
- relationship to Codex CLI
- human review and merge boundaries

## Non-Goals

Sprint 0.1 does not:

- install Symphony
- configure Symphony
- replace Codex CLI
- automate multi-agent orchestration
- change GitHub Project workflow

## Evaluation Trigger

Consider evaluating Symphony after:

- Codex CLI has completed at least one implementation sprint
- recurring orchestration friction is documented
- the project has enough tests and validation gates to support higher automation
