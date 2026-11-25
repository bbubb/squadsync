import Link from "next/link";
import { TeamDetail } from "@/features/teams/team-detail";
import { TeamRosterEntry } from "@/features/teams/team-roster-entry";
import { isRosterEntryId, isTeamId } from "@/features/teams/team-route";

export default async function RosterEntryPage({
  params,
}: {
  params: Promise<{ teamId: string; rosterEntryId: string }>;
}) {
  const { teamId, rosterEntryId } = await params;
  const validTeam = isTeamId(teamId);

  return (
    <div className="mx-auto w-full max-w-6xl">
      <Link className="team-back" href={validTeam ? `/teams/${teamId}/roster` : "/"}>
        {validTeam ? "← Back to Team roster" : "← All Teams"}
      </Link>
      {validTeam ? (
        <>
          <div className="page-heading">
            <h1>Player details</h1>
          </div>
          {isRosterEntryId(rosterEntryId) ? (
            <TeamDetail teamId={teamId}>
              <TeamRosterEntry teamId={teamId} rosterEntryId={rosterEntryId} />
            </TeamDetail>
          ) : (
            <div className="team-panel">
              <p role="alert">Invalid roster entry link.</p>
            </div>
          )}
        </>
      ) : (
        <div className="team-panel">
          <h1>Invalid Team link</h1>
          <p>Return to the workspace to choose an available Team.</p>
        </div>
      )}
    </div>
  );
}
