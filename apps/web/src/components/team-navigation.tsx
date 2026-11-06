"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { isTeamId } from "@/features/teams/team-route";

export function TeamNavigation() {
  const pathname = usePathname();
  const [, segment, teamId] = pathname.split("/");
  if (segment !== "teams" || !teamId || !isTeamId(teamId)) return null;
  const overview = `/teams/${teamId}`;

  return (
    <nav className="team-navigation" aria-label="Selected Team">
      <p className="eyebrow">Selected Team</p>
      <ul className="team-navigation-items">
        <li>
          <Link
            className="workspace-nav"
            href={overview}
            aria-current={pathname === overview ? "page" : undefined}
          >
            Overview
          </Link>
        </li>
        <li>
          <span className="workspace-nav" aria-disabled="true">
            Roster <small>Coming soon</small>
          </span>
        </li>
        <li>
          <span className="workspace-nav" aria-disabled="true">
            Members <small>Coming soon</small>
          </span>
        </li>
      </ul>
    </nav>
  );
}
