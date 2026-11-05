import Link from "next/link";
import { TeamDetail } from "@/features/teams/team-detail";

export default async function TeamPage({ params }: { params: Promise<{ teamId: string }> }) {
  const { teamId } = await params;
  const validId = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(teamId);
  return (
    <div className="mx-auto w-full max-w-6xl">
      <Link className="team-back" href="/">
        ← All Teams
      </Link>
      {validId ? (
        <>
          <div className="page-heading">
            <p className="eyebrow">Local workspace</p>
            <h1>Team workspace</h1>
          </div>
          <TeamDetail teamId={teamId} />
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
