import Link from "next/link";
import { TeamCollection } from "@/features/teams/team-collection";

export function WorkspaceLanding() {
  return (
    <div className="mx-auto w-full max-w-6xl">
      <div className="page-heading">
        <p className="eyebrow">The bigger picture</p>
        <h1>Your workspace</h1>
        <p>A little clarity before the next whistle.</p>
      </div>

      <section className="welcome-panel grid overflow-hidden" aria-labelledby="welcome-heading">
        <div className="welcome-copy">
          <p className="eyebrow">For the love of the game</p>
          <h2 id="welcome-heading">
            Good preparation
            <br />
            starts here<span>.</span>
          </h2>
          <p>A calm space to bring your squad together. Your coaching workspace is taking shape.</p>
          <span className="welcome-caption">One team. A shared direction.</span>
        </div>
        <div className="pitch-art" aria-hidden="true">
          <div className="pitch-outline">
            <div className="pitch-halfway" />
            <div className="pitch-circle" />
            <div className="pitch-box pitch-box-top" />
            <div className="pitch-box pitch-box-bottom" />
            <span className="pitch-player player-one" />
            <span className="pitch-player player-two" />
            <span className="pitch-player player-three" />
          </div>
        </div>
      </section>

      <section className="team-panel" aria-labelledby="teams-heading">
        <p className="eyebrow">Your Teams</p>
        <h2 id="teams-heading">Choose your Team</h2>
        <p>Open a Team to enter its workspace.</p>
        <TeamCollection />
        <Link className="team-action inline-block" href="/teams/new">
          Create Team
        </Link>
      </section>
    </div>
  );
}
