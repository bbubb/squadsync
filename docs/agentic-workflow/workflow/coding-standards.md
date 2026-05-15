# Coding Standards

## Purpose

This document defines baseline engineering standards for SquadSync implementation work.

The goal is to keep code simple, modular, testable, and understandable to human reviewers and future agents.

## Core Principles

- Prefer clarity over cleverness.
- Keep changes small and reviewable.
- Follow documented architecture boundaries.
- Use domain language consistently.
- Make behavior testable.
- Avoid premature abstraction.
- Add abstractions only when they reduce real duplication or protect a clear boundary.

## Clean Architecture Direction

Backend code should preserve a modular ASP.NET Core structure:

```text
API -> Application -> Domain
Infrastructure -> Application
Infrastructure -> Domain
```

Expected responsibilities:

- Domain: entities, value objects, enums, domain rules, domain events when needed.
- Application: use cases, DTO orchestration, ports/interfaces, validation coordination, transaction boundaries.
- Infrastructure: EF Core, external clients, logging adapters, persistence implementations.
- API: endpoints/controllers, request/response contracts, middleware, dependency injection, OpenAPI, health checks.

The Domain layer should not depend on ASP.NET Core, EF Core, database packages, external APIs, or infrastructure concerns.

## SOLID Guidance

Use SOLID as a practical review lens, not as ceremony.

- Single Responsibility: classes and functions should have one clear reason to change.
- Open/Closed: avoid modifying stable domain logic for every new integration path.
- Liskov Substitution: do not create abstractions whose implementations violate expected behavior.
- Interface Segregation: keep interfaces small and role-specific.
- Dependency Inversion: higher-level application/domain behavior should not depend on infrastructure details.

## Function and Class Design

Prefer:

- explicit names
- small functions
- cohesive classes
- narrow interfaces
- constructor injection where dependencies are required
- immutable or guarded domain state where practical
- validation close to the boundary where input enters the system

Avoid:

- large service classes with many responsibilities
- anemic abstractions created only for appearance
- hidden side effects
- broad static helpers for domain behavior
- infrastructure leakage into domain/application code

## Error Handling

Early implementation should use simple, explicit error handling.

Do not add broad exception frameworks, result wrappers, or middleware complexity until the first API behaviors reveal the needed shape.

When adding error handling, document the convention and keep API responses consistent.

## Logging

Use structured logging when application code exists.

Do not log sensitive user data.

Keep logs useful for debugging local development and future production-readiness.

## Agent Guidance

Agents should:

- follow existing patterns once established
- avoid broad refactors unless explicitly scoped
- keep generated code idiomatic for the project stack
- update docs when conventions change
- add tests with behavior changes
- stop when architecture boundaries are unclear

## Review Standard

A reviewer should be able to understand:

- why the change exists
- which layer owns the behavior
- how the change was validated
- what was intentionally not included
- whether follow-up work is needed
