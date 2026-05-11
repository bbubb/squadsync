# Copilot Instructions

SquadSync is a public, soccer-focused MVP used to demonstrate production-minded full-stack architecture. Keep generated suggestions aligned with the documented scope and ADRs.

## Global Guidance

- Prefer clear, explicit, maintainable code over clever abstractions.
- Keep the implementation soccer-focused and portfolio-safe.
- Do not introduce generalized competition-platform concepts unless a future ADR approves them.
- Do not expose proprietary lineup/substitution algorithm details.
- Keep changes small and reviewable.
- When architecture changes, update or propose an ADR.
- When behavior changes, update tests and documentation.

## Backend Defaults

- Use C# and ASP.NET Core.
- Preserve modular boundaries: API, Application, Domain, Infrastructure.
- Use Entity Framework Core for persistence.
- Use FluentValidation for request/use-case validation where appropriate.
- Use Serilog for structured logging.
- Prefer explicit membership modeling over polymorphic role-bearing abstractions.

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
- Keep public docs strong but not strategically revealing.
