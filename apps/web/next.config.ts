import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Keep repository-owned AGENTS.md guidance stable when the dev server starts.
  agentRules: false,
  experimental: { agentUpgrade: "security" },
  poweredByHeader: false,
  turbopack: { root: process.cwd() },
};

export default nextConfig;
