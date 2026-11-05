"use client";

import { ApiError } from "@/lib/api/client";
import { useTeam } from "./use-team";

export function TeamDetail({ teamId }: { teamId: string }) {
  const query = useTeam(teamId);

  if (query.isPending) return <p role="status">Loading Team…</p>;
  if (query.isError) {
    if (query.error instanceof ApiError && query.error.status === 404) {
      return (
        <p role="status">
          This Team was not found. Return to the workspace to choose an available Team.
        </p>
      );
    }
    return (
      <div>
        <p role="alert">
          Team could not be loaded. Check that the local Development API is running.
        </p>
        <button
          className="team-action"
          onClick={() => void query.refetch()}
          disabled={query.isFetching}
        >
          {query.isFetching ? "Retrying…" : "Try again"}
        </button>
      </div>
    );
  }

  const team = query.data;
  return (
    <section className="team-panel" aria-labelledby="selected-team-heading">
      <p className="eyebrow">Selected Team</p>
      <h2 id="selected-team-heading" className="team-name">
        {team.name}
      </h2>
      <dl className="team-identity">
        <dt>Team ID</dt>
        <dd>{team.id}</dd>
      </dl>
      <p>
        You’re in this Team’s workspace. Roster and Player workflows are coming in a later slice.
      </p>
    </section>
  );
}
