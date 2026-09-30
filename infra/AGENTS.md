# Infrastructure Agent Instructions

## Status

Placeholder for a planned implementation area.

This file reserves local guidance for future agents working under `infra/`. It is intentionally concise until infrastructure work begins.

## Purpose

`infra/` contains local and future cloud infrastructure direction for SquadSync.

Current infrastructure direction:

- use Docker only for local development support when needed;
- prefer AWS serverless or scale-to-zero options for later cloud features;
- defer Terraform, CDK, SAM, Kubernetes, and deployment pipelines until a later issue or ADR selects them.

## Planned Rules

When infrastructure work begins:

- keep local Docker configuration separate from cloud deployment assets;
- do not introduce cloud services without an ADR or roadmap-approved issue;
- document cost and scale-to-zero implications for cloud choices;
- keep secrets out of the repository;
- prefer minimal infrastructure that supports the MVP and portfolio goal.

## Stop Conditions

Agents should stop before adding infrastructure code if:

- the task requires a cloud provider or IaC tool decision not covered by an ADR;
- credentials, secrets, or account-specific values would be needed;
- the work would expand the MVP into production infrastructure prematurely.

## Context Routing

For future infrastructure work, start with the active issue, root `AGENTS.md`, `CONTRIBUTING.md`, and this file. Load only the setup or design sources selected by the task: consult `infra/docker/README.md` for local Docker changes, `infra/aws/README.md` for an AWS-specific issue, `infra/README.md` for general infrastructure orientation, the system overview for a service-boundary question, and roadmap or validation guidance when the issue requires it. Do not preload AWS, Docker, roadmap, or architecture documents solely because a task is under `infra/`.

Follow references or investigate further when a dependency, ambiguity, conflict, or architecture concern emerges. Read `PLANS.md` when the task is complex, cross-cutting, risky, or multi-step. Preserve the stop conditions above.
