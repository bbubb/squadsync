"use client";

import { useQuery } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";

type MembershipCollection =
  paths["/api/teams/{teamId}/memberships"]["get"]["responses"][200]["content"]["application/json"];

export const teamMembershipsQueryKey = (teamId: string) =>
  ["teams", teamId, "memberships"] as const;

export function useTeamMemberships(teamId: string) {
  return useQuery({
    queryKey: teamMembershipsQueryKey(teamId),
    staleTime: 30_000,
    queryFn: async ({ signal }): Promise<MembershipCollection> => {
      const { data } = await apiClient.GET("/api/teams/{teamId}/memberships", {
        params: { path: { teamId } },
        signal,
      });
      if (data === undefined) {
        throw new ApiError(
          `GET /api/teams/${teamId}/memberships returned no membership collection.`,
        );
      }
      return data;
    },
  });
}
