"use client";

import type { components } from "@/lib/api/schema";
import { useTeamMemberships } from "./use-team-memberships";

type Membership = components["schemas"]["MembershipResponse"];

// Presentation only: preserve unknown API values rather than inventing a role or permission.
function roleLabel(role: string) {
  return role === "AssistantCoach" ? "Assistant Coach" : role;
}

export function MembersList({ memberships }: { memberships: readonly Membership[] }) {
  return (
    <ul className="mt-6 list-none p-0" aria-label="Team members">
      {memberships.map((membership) => (
        <li
          key={membership.id}
          className="grid grid-cols-1 items-center gap-3 border-t border-[#dfe4da] py-5 sm:grid-cols-2 sm:gap-4"
        >
          <h3 className="min-w-0 font-semibold wrap-anywhere">
            {membership.person.firstName} {membership.person.lastName}
          </h3>
          <dl className="min-w-0 wrap-anywhere">
            <dt className="text-sm text-[var(--color-muted)]">Team role</dt>
            <dd className="mt-1">{roleLabel(membership.teamRole)}</dd>
          </dl>
        </li>
      ))}
    </ul>
  );
}

export function TeamMembers({ teamId }: { teamId: string }) {
  const query = useTeamMemberships(teamId);
  if (query.isPending) return <p role="status">Loading members…</p>;
  if (query.isError) {
    return (
      <div>
        <p role="alert">
          Members could not be loaded. Check that the local Development API is running, then retry.
        </p>
        <button
          className="team-action"
          onClick={() => void query.refetch()}
          disabled={query.isFetching}
        >
          {query.isFetching ? "Retrying…" : "Retry members"}
        </button>
      </div>
    );
  }
  if (query.data.length === 0) return <p role="status">No members yet for this Team.</p>;
  return <MembersList memberships={query.data} />;
}
