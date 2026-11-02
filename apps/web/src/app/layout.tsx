import type { Metadata } from "next";
import type { ReactNode } from "react";
import { WorkspaceShell } from "@/components/workspace-shell";
import "./globals.css";
import { QueryProvider } from "./query-provider";

export const metadata: Metadata = {
  title: "Workspace | SquadSync",
  description: "Your SquadSync coaching workspace.",
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <QueryProvider>
          <WorkspaceShell>{children}</WorkspaceShell>
        </QueryProvider>
      </body>
    </html>
  );
}
