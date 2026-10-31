# UX Notes

## Document Header

| Field | Value |
|---|---|
| Version | v0.2.0 |
| Date | 2026-10-31 |
| Owner | Brandon Bunch |
| Status | Draft |
| Change Summary | Phase 4 first coach journey, responsive navigation, and local UI behavior. |

## Purpose

Capture the smallest usable coach-centered web experience and recurring UX principles. This is the public MVP UI direction, not an account/product-authority design or an extensive design system. The implementation boundary is established by [ADR 0007](../adr/0007-web-foundation-architecture.md) once accepted.

## UX Goal

SquadSync should feel like a practical coaching workspace, not a generic admin dashboard.

The MVP should help coaches quickly answer:

- Who is on my roster?
- Who is available?
- What match am I planning for?
- What lineup am I building?
- What needs attention before game time?

## UX Principles

- Prioritize clarity over visual complexity and make common coaching tasks quick.
- Use natural soccer language, a clear active Team context, and predictable back navigation.
- Prefer progressive disclosure over dense screens.
- Expose functionality through accessible buttons, labeled controls, keyboard navigation and visible focus states; do not rely on color or hover alone.
- Keep initial page and component patterns adaptable to desktop and narrow-screen use without committing to a native mobile application.

## Phase 4 First Journey (Development-only)

The local demonstration follows this path:

1. **Workspace landing:** display available Teams or a useful empty state with a Create Team action. There is no sign-in, default Team, personalized dashboard or notification feed in Phase 4.
2. **Select or create Team:** show the selected Team context in navigation and provide a direct way to reach its roster.
3. **Roster:** display rostered players with available name, jersey number and status; support loading, no-players and API failure states.
4. **Add Player:** explicitly choose **Select Existing Person** or **Create New Person**. Selection is limited to a narrow local Development-only lookup, not a public identity search. Creation and selection then use the existing Person → TeamMembership (Player) → RosterEntry API workflow. Explain conflicts and incomplete steps without inventing or bypassing server-side business rules.
5. **Player details:** show represented Person information, team-context roster data, and optional person-level PlayerProfile data. Allow deliberate PlayerProfile creation or editing using existing API fields; never create a profile automatically because a Person or roster entry was created.

All management actions are Development-only capability demonstrations. Their visibility does not assert authenticated permissions, identity verification, account ownership, or future entitlement rules.

## Desktop and Responsive Navigation

Use a lightweight separation of context:

- **Workspace/top region:** product identity, current Team, and an accessible Team switcher. Represent Teams with readable labels or initials until real assets exist. A constrained, scrollable selection pattern is acceptable if there are many Teams, but text must remain discoverable without relying exclusively on icon recognition.
- **Team navigation/sidebar:** contextual sections such as Overview and Roster, with a clear active state. Avoid empty navigation items for unimplemented features.
- **Main workspace:** selected Team page, roster and forms; reserve space for useful content, not decorative cards.
- **Player details:** use a stable navigable detail route as the source of truth. On roomy desktop screens, the same detail content can appear in a side panel so the roster remains visible; on narrow screens it becomes a full page. Closing the desktop panel should return focus to the selected roster row where practical.
- **Narrow screen:** replace persistent dense navigation with a compact Team selector and a menu/drawer; give forms and Player details the screen width they need. Preserve keyboard and touch accessibility.

This describes a user mental model, not an authorization or nested-organization structure. Do not build speculative multi-level organization navigation in Phase 4.

## States and Feedback

Each connected view should demonstrate:

- **Loading:** clear pending state or skeleton without flashing misleading empty content.
- **Empty:** a meaningful next action, such as Create Team or Add Player.
- **Error:** explain failed loading or mutation in plain language; keep the user's valid form entries available for correction. Do not expose raw database diagnostics.
- **Success:** confirm the action, update the relevant Team/roster view, and avoid duplicate submissions.
- **Validation:** labeled fields, required/optional indicators, inline field errors and appropriate server error summaries. Backend validation remains authoritative.
- **Navigation:** a selected Team and back/close behavior that preserve orientation; deep-linked detail routes should remain usable when opened directly.

## Early Screen Areas

Phase 4: workspace landing, Team overview/selection, roster, Add Player, Player details and optional PlayerProfile form.

Later MVP phases may add match planning, availability, manual lineup builder, and lineup review; do not create nonfunctional navigation for them now.

## Visual Direction and Tokens

Prefer a modern, calm, functional interface with restrained emphasis and enough contrast. Use reusable spacing, typography, surface and accent tokens so refinement stays inexpensive. Reuse accessible primitives and consistent icons; no custom identity artwork or finalized brand palette is required to make the first journey usable.

The canonical [brand notes](brand-notes.md) remain intentionally lightweight until we review a working shell.

## Deferred UX Decisions

Defer authenticated account controls, public/private player discovery, advanced organization navigation, inbox/chat, notifications, multi-team personalized home content, native mobile app design, and match/lineup interaction details until their dedicated phases. Do not treat these deferred areas as requirements for the first scaffold.

## References

- [Product Brief](product-brief.md)
- [MVP Scope](../planning/mvp-scope.md)
- [System Overview](../architecture/system-overview.md)
- [Domain Model](../architecture/domain-model.md)
- [Web Foundation ADR](../adr/0007-web-foundation-architecture.md)
- [Phase 4 planning issue](https://github.com/bbubb/squadsync/issues/157)
