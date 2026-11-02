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
          <h2 id="welcome-heading">Good preparation<br />starts here<span>.</span></h2>
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

      <section className="team-placeholder flex flex-col items-start gap-6 sm:flex-row sm:items-center" aria-labelledby="teams-heading">
        <div className="team-symbol" aria-hidden="true">
          <svg viewBox="0 0 32 32" fill="none" stroke="currentColor" strokeWidth="1.5">
            <path d="M16 3 27 7v8c0 6-5 10-11 14C10 25 5 21 5 15V7L16 3Z" />
            <path d="m11 16 3.5 3.5L22 12" />
          </svg>
        </div>
        <div className="min-w-0 flex-1">
          <p className="eyebrow">Up next</p>
          <h2 id="teams-heading">A home for your team</h2>
          <p>Team selection is coming next. This is where you’ll find your way into your team’s workspace.</p>
        </div>
        <span className="coming-next">Coming next</span>
      </section>
    </div>
  );
}
