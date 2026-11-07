import Link from "next/link";
import { WorkspaceNavigation } from "./workspace-navigation";
import { TeamNavigation } from "./team-navigation";
import type { ReactNode } from "react";

export function WorkspaceShell({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-dvh">
      <a className="skip-link" href="#main-content">
        Skip to content
      </a>
      <header className="workspace-header flex flex-wrap items-center justify-between gap-4">
        <Link className="brand inline-flex items-center gap-3" href="/" aria-label="SquadSync home">
          <span className="brand-mark" aria-hidden="true">
            S<span>↗</span>
          </span>
          <span>
            SquadSync<span className="brand-dot">.</span>
          </span>
        </Link>
        <span className="preview-label">Local workspace preview</span>
      </header>

      <div className="workspace-layout grid">
        <aside className="workspace-sidebar">
          <div>
            <nav aria-label="Workspace">
              <p className="eyebrow sidebar-label">Your space</p>
              <WorkspaceNavigation />
            </nav>
            <TeamNavigation />
          </div>
          <div className="sidebar-note">
            <span className="note-marker" aria-hidden="true" />
            <p>
              A place for your team.
              <br />
              Room for what comes next.
            </p>
          </div>
        </aside>

        <main id="main-content" tabIndex={-1} className="workspace-main min-w-0">
          {children}
        </main>
      </div>
      <footer className="workspace-footer flex flex-wrap justify-between gap-2">
        <p>SquadSync · The team comes first.</p>
        <p>Made for the touchline.</p>
      </footer>
    </div>
  );
}
