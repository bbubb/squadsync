# Documentation Instructions

Apply these instructions to documentation changes under `docs/`, root Markdown files, and ADRs.

## Documentation Style

- Be clear, direct, and decision-oriented.
- Explain trade-offs plainly.
- Avoid marketing hype.
- Keep public docs strong but portfolio-safe.
- Prefer concrete examples over vague claims.
- Keep files useful to both a future maintainer and a hiring reviewer.

## ADR Rules

Architectural Decision Records should include:

- Status
- Context
- Decision
- Consequences
- Alternatives considered

Create or update an ADR when changing:

- major technology choices
- domain modeling strategy
- deployment architecture
- service boundaries
- public/private scope boundaries
- security posture

## Scope Safety

Do not document proprietary algorithm internals, generalized competition-platform strategy, or private business plans. It is acceptable to document public-safe future extension points without explaining hidden implementation details.

## Diagrams

Use Mermaid diagrams in Markdown when possible. Keep diagrams simple enough to maintain in source control.
