"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiClient, ApiError } from "@/lib/api/client";
import type { paths } from "@/lib/api/schema";
import { teamMembershipsQueryKey } from "./use-team-memberships";
import { teamRosterQueryKey } from "./use-team-roster";

type Operation = paths["/api/teams/{teamId}/roster/players"]["post"];
type Request = NonNullable<Operation["requestBody"]>["content"]["application/json"];
export type AddedPlayer = Operation["responses"][201]["content"]["application/json"];

// Swagger describes exclusivity in prose. Narrow only the feature input; keep generated DTOs intact.
export type AddPlayerInput = Omit<Request, "personId" | "newPerson"> &
  (
    | { personId: NonNullable<Request["personId"]>; newPerson?: never }
    | { newPerson: NonNullable<Request["newPerson"]>; personId?: never }
  );

export type AddPlayerFailure =
  | { kind: "validation"; status: 400 }
  | { kind: "not-found"; status: 404 }
  | { kind: "conflict"; status: 409 }
  | { kind: "unknown-outcome"; reason: "transport" | "response"; status?: number };

export class AddPlayerError extends Error {
  constructor(
    public readonly failure: AddPlayerFailure,
    public readonly problem: ApiError["problem"],
    cause: unknown,
  ) {
    super(
      failure.kind === "unknown-outcome"
        ? "The Add Player outcome is unknown. Check the Team roster before trying again."
        : `Add Player failed: ${failure.kind}.`,
      { cause },
    );
    this.name = "AddPlayerError";
  }
}

export function useAddPlayer(teamId: string) {
  const queryClient = useQueryClient();
  return useMutation<AddedPlayer, AddPlayerError, AddPlayerInput, string>({
    retry: false,
    onMutate: () => teamId,
    mutationFn: async (body) => {
      try {
        const { data } = await apiClient.POST("/api/teams/{teamId}/roster/players", {
          params: { path: { teamId } },
          body,
        });
        if (data === undefined) {
          throw new ApiError("Add Player returned no roster entry.");
        }
        return data;
      } catch (cause) {
        const status = cause instanceof ApiError ? cause.status : undefined;
        const failure: AddPlayerFailure =
          status === 400
            ? { kind: "validation", status }
            : status === 404
              ? { kind: "not-found", status }
              : status === 409
                ? { kind: "conflict", status }
                : {
                    kind: "unknown-outcome",
                    reason:
                      cause instanceof ApiError && cause.cause !== undefined
                        ? "transport"
                        : "response",
                    status,
                  };
        throw new AddPlayerError(
          failure,
          cause instanceof ApiError ? cause.problem : undefined,
          cause,
        );
      }
    },
    // Overview totals derive from these same collections. Exact keys leave other Teams untouched.
    onSuccess: async (_data, _variables, mutationTeamId) => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: teamRosterQueryKey(mutationTeamId),
          exact: true,
        }),
        queryClient.invalidateQueries({
          queryKey: teamMembershipsQueryKey(mutationTeamId),
          exact: true,
        }),
      ]);
    },
  });
}
