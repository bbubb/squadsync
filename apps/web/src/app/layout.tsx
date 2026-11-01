import type { Metadata } from "next";
import type { ReactNode } from "react";
import { WorkspaceShell } from "@/components/workspace-shell";
import "./globals.css";

export const metadata: Metadata = {
  title: "Workspace | SquadSync",
  description: "Your SquadSync coaching workspace.",
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <WorkspaceShell>{children}</WorkspaceShell>
      </body>
    </html>
  );
}
