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
- relevant docs/ADRs are linked or named

## Labels

Recommended labels:

- `agent-ready`
- `codex`
- `phase-*`
- `sprint-*`
- `documentation`, `backend`, `frontend`, `infra`, or `workflow` as applicable
