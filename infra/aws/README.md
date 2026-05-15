# AWS Infrastructure

This directory is reserved for future AWS deployment and integration artifacts.

## Direction

SquadSync should prefer scale-to-zero or low-idle-cost AWS services where practical, especially for event-driven capabilities such as notifications and future soccer-subber integration.

Potential future candidates:

- AWS Lambda for event-driven/serverless workloads
- Amazon EventBridge for event routing
- Amazon SNS/SES for notifications, if needed
- Amazon RDS or Aurora Serverless for relational persistence, if justified
- AWS SAM or CDK for serverless infrastructure, if the project outgrows manual setup

## Current Status

Placeholder only. No AWS deployment artifacts have been selected or added yet.

## Deferred Decisions

- Terraform vs CDK vs SAM
- Container hosting vs serverless hosting
- Managed database choice
- Production network/security architecture
- CI/CD handoff from GitHub Actions to AWS-native tooling

## Non-Goals

- Do not introduce Kubernetes for the MVP without a specific ADR.
- Do not add Terraform-style `modules/` or `bootstrap/` structure until Terraform is selected.
- Do not add AWS deployment code before the core application is useful locally.
