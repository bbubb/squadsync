# UX Notes

## Document Header

| Field | Value |
|---|---|
| Version | v0.3.0 |
| Date | 2026-11-12 |
| Owner | Repository owner |
| Status | Draft |
| Change Summary | Clarify provisional Add Player, bounded local lookup, and deferred verified identity UX. |

## Purpose

Establish the initial coach/team-manager experience within SquadSync's soccer team-management platform, guiding design exploration and successive MVP slices. **A UX concept or mockup may show more than the current implementation delivers.** Functional delivery remains governed by approved phase/issue scope; technical implementation choices belong to [ADR 0007](../adr/0007-web-foundation-architecture.md).

## Experience Goal

The initial web experience should feel like a modern, organized coaching workspace, not a generic administrative dashboard. It should help coaches find their Team, understand the roster, and act quickly. Later MVP work extends this toward availability, match preparation, and lineup planning.

Principles:

- Clear context and navigation hierarchy, especially when moving between Teams.
- Calm, distinctive visual hierarchy with purposeful emphasis; avoid visual noise without sacrificing personality.
- Fast common actions, progressive disclosure of detail, and predictable return paths.
- Responsive, keyboard-accessible, touch-usable interactions with visible focus and useful feedback.
- Component and layout flexibility so later capabilities can fit naturally without dictating their implementation today.

## Working Application Shell

The initial design direction separates workspace scope from selected-Team navigation:

| Region | Intended UX responsibility |
|---|---|
| **Global/top region** | Product identity, Team/workspace selection, and a predictable place for app-level utilities such as settings |
| **Team navigation** | Contextual sections for the selected Team, in a left sidebar on roomy screens |
| **Main workspace** | Current screen and primary coaching tasks |
| **Contextual detail** | Player information in a desktop side panel when useful, or a full-page route on smaller screens |

The Team selector may use names, initials, compact symbols, or a horizontally scrollable presentation; compare actual usability and density before settling on one. Distinctive components and visual assets are welcome when they improve this experience. Avoid tying navigation to an icon field that the backend does not provide.

For narrow screens, adapt the same information hierarchy into a compact Team selector and accessible navigation/menu pattern. Player details must have a stable navigable route; the desktop side panel is a convenient presentation of that detail, not a second independent content model.

This is an initial UX direction, not a requirement to implement every shell region or hypothetical navigation level in Phase 4.

## First Functional Journey — Phase 4

1. **Workspace:** find an existing Team or create one; provide a useful empty state.
2. **Team:** enter the selected Team's overview or roster, maintaining clear context.
3. **Roster:** see player names, jersey numbers and roster statuses.
4. **Add Player:** choose **Select Existing Person** or **Create New Person**, then complete the supported Team membership and roster workflow.
5. **Player details:** see relevant Person, roster, and optional person-level PlayerProfile information; explicitly create or edit the existing PlayerProfile fields when needed.

The current API provides Development-only management capabilities without authenticated user permissions. The functional UI demonstrates these operations; it does not define who will be authorized to perform them later. Refer to the domain model and API contracts for underlying behavior.

## Player selection, provisioning and later invitations

**Phase 4 approved direction (not a complete API specification):** The normal Add Player journey starts from the destination Team's roster. Offer **Create New Person** for a coach-provisioned player who has no Account, and **Select Existing Person** through bounded **server-filtered** name lookup in trusted-local Development. Neither path requires the coach to find a source Team or obtain a database GUID from a player. The lookup contract and its safe duplicate-name/ambiguous-candidate behavior are the remaining design gate in [#199](https://github.com/bbubb/squadsync/issues/199); the connected workflow is [Sprint 12 #207](https://github.com/bbubb/squadsync/issues/207).

- Show only minimal Person identity in candidate results. Do not download all People to filter them in React, treat identical names as a verified match, automatically merge People, or imply that the coach owns the represented Person.
- Make ambiguity and no-match states explicit. The user may choose a well-understood existing result or deliberately create a new provisional record; an ID remains internal to API calls, not a human handoff mechanism.
- Membership and RosterEntry actions belong to the selected Team; PlayerProfile information remains associated with the Person across Teams. Avoid presenting a roster departure as Person/Profile deletion.
- [Sprint 13 #208](https://github.com/bbubb/squadsync/issues/208) addresses roster correction/removal, player details and explicit optional PlayerProfile editing. The Person-level editing screen in the trusted-local MVP is not a claim of authenticated coach or guardian authority.

**Later authenticated UX — deferred:** A participant/guardian may be invited by a link or other verified mechanism to accept participation or claim a provisional Person, but a link alone is not proof of control over that Person. Participant-facing aliases, optional search visibility, private profile fields, organizational lookup and consent must be separately reviewed before public discovery. Do not add fake Invite buttons or public search affordances to the working MVP. These are public-safe future constraints, not approved Phase 4 features.

## Design Exploration Versus Shipped Behavior

- **Mockups and prototypes:** may include anticipated structural elements—such as a settings affordance or future navigation area—to test layout and discoverability. Mark features without implemented behavior as conceptual.
- **Running MVP:** controls should represent real actions. If a deliberately retained placeholder helps orientation, make its unavailable state clear and accessible; otherwise omit it from the functional UI until its behavior exists. Do not route users into empty, apparently functional pages.
- **Evolution:** preserve sensible layout slots, tokens, reusable components and navigation boundaries rather than constructing inactive feature systems in advance.

This approach permits a coherent product vision without increasing the current functional scope.

## UI States and Accessibility

Connected views and forms need meaningful loading, empty, error, validation, and success feedback. Preserve submitted information when correcting errors, prevent accidental repeated actions, and update the visible roster after a successful mutation.

Design forms with labels, appropriate keyboard behavior, clear focus order, screen-reader announcements where needed, and touch targets appropriate to narrow layouts. A desktop detail panel should have discoverable close/back behavior and return focus sensibly.

## Visual Language

Use consistent typography, spacing, color roles, surfaces and iconography with room for custom SquadSync components and refinements. Accessible primitives from a toolkit can accelerate quality; they are not a cap on layout, branding or interaction sophistication.

Use the lightweight [brand notes](brand-notes.md) to guide provisional visual choices, refining them against working screens without making a finalized identity a Phase 4 prerequisite.

## Later UX Work

Keep later MVP workflows and broader product ideas in design exploration when relevant to the shell, but do not imply their implementation in Phase 4. Review the navigation composition, responsive behavior, and actual styling after the first connected screens are usable.

## References

- [Product Brief](product-brief.md)
- [MVP Scope](../planning/mvp-scope.md)
- [System Overview](../architecture/system-overview.md)
- [Domain Model](../architecture/domain-model.md)
- [Web Foundation ADR](../adr/0007-web-foundation-architecture.md)
- [Phase 4 planning issue](https://github.com/bbubb/squadsync/issues/157)
