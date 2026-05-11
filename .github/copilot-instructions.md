# Copilot Instructions

SquadSync is a soccer-focused MVP used to demonstrate production-minded full-stack architecture. Keep generated suggestions aligned with the documented scope and ADRs.

## Global Guidance

- Prefer clear, explicit, maintainable code over clever abstractions.
- Keep the implementation soccer-focused.
- Do not introduce broad new product areas unless a future ADR approves them.
- Do not implement lineup optimization logic in the core platform unless explicitly requested by an approved issue.
- Keep changes small and reviewable.
- When architecture changes, update or propose an ADR.
- When behavior changes, update tests and documentation.

## Backend Defaults

- Use C# and ASP.NET Core.
- Preserve modular boundaries: API, Application, Domain, Infrastructure.
- Use Entity Framework Core for persistence.
- Use FluentValidation for request/use-case validation where appropriate.
- Use Serilog for structured logging.
- Prefer explicit team membership modeling.

## Frontend Defaults

- Use Next.js, TypeScript, Tailwind CSS, and feature-oriented structure.
- Keep API access typed and centralized.
- Prefer small components with clear props.
- Keep state local unless shared state is justified.

## Documentation Defaults

- Use Markdown.
- Keep docs accurate and decision-oriented.
- Avoid hype.
- Explain trade-offs plainly.
- Keep docs focused on current product scope and approved architecture.
