"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";

type RosterCollection =
  paths["/api/teams/{teamId}/roster"]["get"]["responses"][200]["content"]["application/json"];

export const teamRosterQueryKey = (teamId: string) => ["teams", teamId, "roster"] as const;

export function useTeamRoster(teamId: string) {
  return useQuery({
    queryKey: teamRosterQueryKey(teamId),
    staleTime: 30_000,
    queryFn: async ({ signal }): Promise<RosterCollection> => {
      const { data } = await apiClient.GET("/api/teams/{teamId}/roster", {
        params: { path: { teamId } },
        signal,
      });
      if (data === undefined) {
        throw new ApiError(`GET /api/teams/${teamId}/roster returned no roster collection.`);
      }
      return data;
    },
  });
}
