"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

export function WorkspaceNavigation() {
  const pathname = usePathname();
  return (
    <Link
      className="workspace-nav flex items-center gap-3"
      href="/"
      aria-current={pathname === "/" ? "page" : undefined}
    >
      <svg
        aria-hidden="true"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="1.6"
      >
        <rect x="3" y="3" width="7" height="7" rx="1.5" />
        <rect x="14" y="3" width="7" height="7" rx="1.5" />
        <rect x="3" y="14" width="7" height="7" rx="1.5" />
        <rect x="14" y="14" width="7" height="7" rx="1.5" />
      </svg>
      Workspace
    </Link>
  );
}
