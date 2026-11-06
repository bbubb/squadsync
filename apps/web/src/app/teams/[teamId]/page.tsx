import Link from "next/link";
import { TeamDetail } from "@/features/teams/team-detail";
import { TeamOverview } from "@/features/teams/team-overview";
import { isTeamId } from "@/features/teams/team-route";

export default async function TeamPage({ params }: { params: Promise<{ teamId: string }> }) {
  const { teamId } = await params;
  const validId = isTeamId(teamId);
  return (
    <div className="mx-auto w-full max-w-6xl">
      <Link className="team-back" href="/">
        ← All Teams
      </Link>
      {validId ? (
        <>
          <div className="page-heading">
            <p className="eyebrow">Local workspace</p>
            <h1>Team overview</h1>
          </div>
          <TeamDetail teamId={teamId}>
            <TeamOverview teamId={teamId} />
          </TeamDetail>
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
