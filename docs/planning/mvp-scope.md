# MVP Scope

## Status

Draft for Sprint 0 review.

## Purpose

This document defines the public MVP boundary for SquadSync. It keeps the project narrow enough to ship while still demonstrating meaningful architecture, domain modeling, full-stack delivery, and future cloud/service integration.

## Product Statement

SquadSync is a soccer-focused team management and match-planning platform for coaches. The MVP helps a coach organize a team roster, track player profiles and availability, plan matches, build lineups, and prepare for future integration with a separate lineup optimization service.

## Primary User

The primary MVP user is a soccer coach or team manager responsible for:

- Maintaining a roster
- Understanding player roles and positions
- Planning match-day lineups
- Tracking availability
- Communicating lineup-related changes later through notification workflows

Players and parents may become future user types, but the first public MVP should center on the coach workflow.

## Included in MVP

### Team and Roster Management

- Create and manage teams
- Add users or player profiles to a team roster
- Track player number, preferred positions, dominant side, and status
- Track whether a player is active, inactive, injured, unavailable, or otherwise unavailable for selection

### Membership and Roles

- Represent team participation through explicit `TeamMembership` records
- Assign membership roles such as Coach, AssistantCoach, Manager, Player, or Guardian/Viewer later if needed
- Keep authorization simple for the MVP

### Match Planning

- Create matches for a team
- Track opponent, date/time, location, and match status
- Track player availability for a match

### Lineup Planning

- Select a formation
- Create a manual lineup
- Assign players to lineup slots
- Store the lineup for a match
- Prepare a future integration point for lineup suggestions

### Soccer-Subber Integration Contract

- Define a public-safe request/response contract for an external lineup suggestion service
- Provide a mock or adapter boundary inside SquadSync
- Keep actual optimization internals outside the public platform repository

### Event and Notification Readiness

- Identify key events such as `LineupCreated`, `LineupPublished`, and `PlayerAvailabilityChanged`
- Document future AWS eventing/notification architecture
- Avoid implementing complex cloud infrastructure before the core product slice is functional

## Excluded from MVP

The public MVP must not include:

- Generalized competition-platform modeling
- Universal sport/competition configuration
- Dynamic role-bearing abstractions such as the archived `IRoleBearer` pattern
- Proprietary substitution or optimization algorithm internals
- Private business strategy
- Multi-tenant billing/subscription behavior
- Full parent/player communication portal
- Tournament management
- Advanced statistics engine
- AI-generated coaching analysis beyond a future public-safe summary placeholder

## Future but Not Now

These are valid future directions but should not drive early implementation:

- Cloud-hosted production deployment
- AWS EventBridge/SNS/SQS/Lambda notification pipelines
- Separate `soccer-subber` microservice deployment
- AI-generated lineup explanation summaries
- Auth provider integration
- Advanced player development analytics
- Mobile-first player/parent experience
- Multi-team organization management

## Success Criteria for the Public MVP

The MVP is successful when a reviewer can see that the project has:

- A clean, professional repo structure
- A coherent domain model
- A working backend API
- A working frontend dashboard
- Team and roster management
- Match and lineup planning
- A credible external service boundary
- Documentation explaining key architecture decisions
- A clear AWS/cloud roadmap

## Scope Principle

When in doubt, choose the smallest soccer-specific implementation that demonstrates the architectural idea without revealing broader platform IP.
