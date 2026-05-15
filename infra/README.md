# Infrastructure

This directory contains local infrastructure direction and future cloud deployment artifacts for SquadSync.

## Current Status

No runtime infrastructure code has been added yet.

Phase 1 should prioritize local development support. Cloud deployment artifacts should be added later when the core vertical slice is useful.

## Structure

```text
infra/
  docker/  Local Docker and Docker Compose direction
  aws/     Future AWS/serverless direction
```

## Direction

- Use Docker/Docker Compose for local dependencies such as PostgreSQL.
- Prefer AWS serverless or scale-to-zero services where practical for future MVP deployment.
- Defer Terraform, CDK, SAM, and Kubernetes decisions until the deployment target is clearer.

## Non-Goals

- No Terraform modules yet.
- No Kubernetes manifests yet.
- No AWS deployment templates yet.
- No production environment configuration yet.
