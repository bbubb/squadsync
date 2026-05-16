# Subagent Profile: Infrastructure Implementation

## Purpose

Planned role for scoped infrastructure work under `infra/` and `.github/workflows/`.

## When to Use

Use this role for future issues involving:

- Docker Compose/local dependencies
- GitHub Actions CI
- AWS/serverless deployment artifacts
- infrastructure documentation
- environment/configuration guidance

## Required Context

- `AGENTS.md`
- `infra/README.md`
- `infra/docker/README.md`
- `infra/aws/README.md`
- `docs/architecture/system-overview.md`
- `docs/agentic-workflow/workflow/branching-strategy.md`
- `docs/agentic-workflow/workflow/validation-gates.md`

## Allowed Changes

- `infra/`
- `.github/workflows/`
- docs directly related to infrastructure behavior

## Stop Conditions

Stop if the issue requires:

- selecting Terraform, CDK, SAM, or Kubernetes without ADR approval
- adding production infrastructure before the local vertical slice is useful
- introducing paid or always-on cloud resources without explicit approval
- changing application architecture

## Validation Expectations

Validation depends on artifact type. For CI changes, document expected workflow checks. For Docker changes, document startup and smoke-test expectations.

## Status

Planned role profile only. No active subagent automation is configured.
