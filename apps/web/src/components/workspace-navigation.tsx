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
      Workspace
    </Link>
  );
}
