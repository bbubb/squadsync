import Link from "next/link";
import { TeamDetail } from "@/features/teams/team-detail";
import { TeamMembers } from "@/features/teams/team-members";
import { isTeamId } from "@/features/teams/team-route";

export default async function MembersPage({ params }: { params: Promise<{ teamId: string }> }) {
  const { teamId } = await params;
  return (
    <div className="mx-auto w-full max-w-6xl">
      <Link className="team-back" href="/">
        ← All Teams
      </Link>
      {isTeamId(teamId) ? (
        <>
          <div className="page-heading">
            <p className="eyebrow">Local workspace</p>
            <h1>Team members</h1>
          </div>
          <TeamDetail teamId={teamId}>
            <p className="mt-4 text-[var(--color-muted)]">
              People associated with this Team and their roles.
            </p>
            <TeamMembers teamId={teamId} />
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
