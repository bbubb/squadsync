"use client";

import Link from "next/link";
import { useTeams } from "./use-teams";

export function TeamCollection({ selectedTeamId }: { selectedTeamId?: string }) {
  const query = useTeams();

  if (query.isPending) return <p role="status">Loading Teams…</p>;
  if (query.isError) {
    return (
      <div>
        <p role="alert">
          Teams could not be loaded. Check that the local Development API is running.
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

  if (selectedTeamId) {
    const team = query.data.find((team) => team.id.toLowerCase() === selectedTeamId.toLowerCase());
    if (!team)
      return (
        <p role="status">
          This Team is not in the local workspace. Return to the workspace to choose an available
          Team.
        </p>
      );
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

  if (query.data.length === 0) {
    return (
      <p role="status">
        No Teams yet. Create a Team through the local Development API or run the documented demo
        seed, then reload this workspace.
      </p>
    );
  }

  return (
    <ul className="team-grid">
      {query.data.map((team) => (
        <li key={team.id}>
          <Link className="team-card" href={`/teams/${team.id}`}>
            <span className="team-name">{team.name}</span>
            <span className="team-identity">{team.id}</span>
            <span className="team-enter">
              Open Team <span aria-hidden="true">↗</span>
            </span>
          </Link>
        </li>
      ))}
    </ul>
  );
}
