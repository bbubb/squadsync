# MVP Scope

## Status

Draft for Sprint 0 review.

## Purpose

This document defines the first shippable product slice for SquadSync. It keeps the project narrow enough to build while still demonstrating meaningful architecture, domain modeling, full-stack delivery, and future cloud/service integration.

## Product Statement

SquadSync is a soccer-focused team management and match-planning platform for coaches. The MVP helps a coach organize a team roster, track player profiles and availability, plan matches, build lineups, and prepare for future integration with a separate lineup assistance service.

## Primary User

The primary MVP user is a soccer coach or team manager responsible for:

- Maintaining a roster
- Understanding player roles and positions
- Planning match-day lineups
- Tracking availability
- Communicating lineup-related changes later through notification workflows

Players and parents may become future user types, but the first MVP centers on the coach workflow.

## Included in MVP

### Team and Roster Management

- Create and manage teams
- Add users or player profiles to a team roster
- Track player number, preferred positions, dominant side, and status
- Track whether a player is active, inactive, injured, unavailable, or otherwise unavailable for selection

### Membership and Roles

- Represent team participation through `TeamMembership` records
- Assign membership roles such as Coach, AssistantCoach, Manager, Player, or Viewer
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

- Define a request/response contract for an external lineup suggestion service
- Provide a mock or adapter boundary inside SquadSync
- Keep the core platform responsible for team, roster, match, and lineup data

### Event and Notification Readiness

- Identify key events such as `LineupCreated`, `LineupPublished`, and `PlayerAvailabilityChanged`
- Document future AWS eventing/notification architecture
- Avoid implementing complex cloud infrastructure before the core product slice is functional

## Deferred Until Later

The following areas are outside the first MVP and should be introduced only through future issues and ADRs:

- Cloud-hosted production deployment
- AWS EventBridge/SNS/SQS/Lambda notification pipelines
- Separate `soccer-subber` microservice deployment
- AI-generated lineup explanation summaries
- Auth provider integration
- Mobile-first player/parent experience
- Multi-team organization management

## Success Criteria for the MVP

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

When in doubt, choose the smallest soccer-specific implementation that proves the architectural idea and supports the coach workflow.
