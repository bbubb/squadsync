"use client";

import { ApiError } from "@/lib/api/client";
import { RosterEntryDetails } from "./roster-entry-details";
import { useTeamRosterEntry } from "./use-team-roster";

export function TeamRosterEntry({
  teamId,
  rosterEntryId,
}: {
  teamId: string;
  rosterEntryId: string;
}) {
  const query = useTeamRosterEntry(teamId, rosterEntryId);

  if (query.isPending) return <p role="status">Loading player details…</p>;
  if (query.isError) {
    if (query.error instanceof ApiError) {
      if (query.error.status === 404) {
        return <p role="status">This roster entry was not found for this Team.</p>;
      }
      if (query.error.status === 400) {
        return <p role="alert">Invalid roster entry link.</p>;
      }
    }
    return (
      <div>
        <p role="alert">Player details could not be loaded. Check the local Development API.</p>
        <button
          className="team-action"
          type="button"
          onClick={() => void query.refetch()}
          disabled={query.isFetching}
        >
          {query.isFetching ? "Retrying…" : "Retry player details"}
        </button>
      </div>
    );
  }

  const entry = query.data;
  return (
    <div className="mt-6">
      <h3 className="mb-3 text-lg font-semibold wrap-anywhere">
        {entry.person.firstName} {entry.person.lastName}
      </h3>
      <RosterEntryDetails entry={entry} />
    </div>
  );
}
