# Testing Strategy

## Purpose

This document defines how SquadSync should approach testing before and during implementation.

The goal is to make behavior verifiable, keep agent-generated code safe to review, and avoid treating tests as an afterthought.

## Strategy

SquadSync uses a TDD-oriented testing workflow.

This means tests should normally be defined before or alongside behavior implementation, but the project is not dogmatic about red-green-refactor for every docs, scaffold, or configuration task.

## Testing Principles

- Behavior changes should include tests.
- Domain and application logic should be designed for unit testing.
- Infrastructure and API behavior should use integration tests where unit tests would provide false confidence.
- Tests should verify business behavior, not implementation trivia.
- Agents should document when tests are not applicable or cannot be run.
- A PR should not claim validation passed unless it was actually run.

## Backend Testing Expectations

When backend code exists, expected validation includes:

```bash
dotnet restore
dotnet build
dotnet test
```

Recommended backend test layers:

- Domain unit tests
- Application use-case tests
- API/integration tests
- Architecture/dependency tests later if useful

## TDD-Oriented Flow

For behavior work:

```text
1. Clarify expected behavior and acceptance criteria.
2. Define or update tests for that behavior.
3. Implement the smallest code needed to pass.
4. Refactor while keeping tests green.
5. Run validation gates.
6. Report results in the PR.
```

## When Strict TDD Is Not Required

Strict red-green-refactor is not required for:

- docs-only changes
- repository scaffolding with no behavior yet
- initial wiring tasks where validation is build/run based
- exploratory spikes explicitly scoped as spikes

Even then, the PR must document validation.

## Agent Guidance

Implementation agents should:

- identify expected tests before coding behavior
- add or update tests with behavior changes
- avoid broad test rewrites outside scope
- document missing test infrastructure as a limitation
- suggest follow-up issues when stronger test coverage is needed

## Phase 1 Implication

The backend scaffold should include test projects early so future issues can follow this testing strategy without retrofitting basic test infrastructure later.
