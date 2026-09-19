# Agent Task Specification

## Purpose

This specification defines the standard shape of an agent-ready GitHub issue.

Agent tasks should be clear enough for Codex CLI or another future implementation tool to execute without relying on hidden chat context.

## Required Sections

### Objective

A concise statement of the task outcome.

### Context

Background needed to understand why the task exists.

### Scope

Files, folders, behavior, or documentation areas that may be changed.

### Non-Goals

Explicit boundaries for what should not be changed.

### Relevant Docs / ADRs

Canonical references the agent must read before starting.

### Acceptance Criteria

A checklist of observable outcomes required for completion.

### Validation

Commands, checks, or documentation validation expected before PR review.

### Stop Conditions

Known reasons the agent should pause and ask for clarification.

### Notes for Agent

Implementation guidance, constraints, or warnings that do not fit elsewhere.

## Agent-Ready Standard

An issue should receive `agent-ready` only when:

- scope is narrow enough for one focused PR
- non-goals are clear
- acceptance criteria are testable or reviewable
- validation expectations are documented
- required architecture decisions already exist or are not needed
- relevant docs/ADRs are linked or named by exact repository path

## Label Lifecycle

Creating an issue from the agent-task template does not make it ready for implementation. After the issue is complete, the main planning thread or human owner should compare its paths, scope, acceptance criteria, validation, and required decisions with the current repository and then add `agent-ready`.

Treat an existing `agent-ready` label as stale if the issue conflicts with current canonical guidance or no longer satisfies this specification. Pause implementation until the issue is refreshed; remove or restore the label only through an authorized GitHub action.

## Labels

Recommended labels:

- `agent-ready`
- `codex`
- `phase-*`
- `sprint-*`
- `documentation`, `backend`, `frontend`, `infra`, or `workflow` as applicable
