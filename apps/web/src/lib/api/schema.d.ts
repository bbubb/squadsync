// Generated from openapi/squadsync.json; run pnpm api:generate. Do not edit.
export interface paths {
  "/api/teams/{teamId}/memberships": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["MembershipResponse"][];
            "application/json": components["schemas"]["MembershipResponse"][];
            "text/json": components["schemas"]["MembershipResponse"][];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
        };
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["CreateMembershipRequest"];
          "text/json": components["schemas"]["CreateMembershipRequest"];
          "application/*+json": components["schemas"]["CreateMembershipRequest"];
        };
      };
      responses: {
        /** @description Created */
        201: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["MembershipResponse"];
            "application/json": components["schemas"]["MembershipResponse"];
            "text/json": components["schemas"]["MembershipResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
        /** @description Conflict */
        409: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/teams/{teamId}/memberships/{membershipId}": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
          membershipId: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["MembershipResponse"];
            "application/json": components["schemas"]["MembershipResponse"];
            "text/json": components["schemas"]["MembershipResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post?: never;
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/people": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get?: never;
    put?: never;
    post: {
      parameters: {
        query?: never;
        header?: never;
        path?: never;
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["CreatePersonRequest"];
          "text/json": components["schemas"]["CreatePersonRequest"];
          "application/*+json": components["schemas"]["CreatePersonRequest"];
        };
      };
      responses: {
        /** @description Created */
        201: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["PersonResponse"];
            "application/json": components["schemas"]["PersonResponse"];
            "text/json": components["schemas"]["PersonResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
      };
    };
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/people/{id}": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          id: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["PersonResponse"];
            "application/json": components["schemas"]["PersonResponse"];
            "text/json": components["schemas"]["PersonResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post?: never;
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/people/{personId}/player-profile": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          personId: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["PlayerProfileResponse"];
            "application/json": components["schemas"]["PlayerProfileResponse"];
            "text/json": components["schemas"]["PlayerProfileResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          personId: string;
        };
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["UpdatePlayerProfileRequest"];
          "text/json": components["schemas"]["UpdatePlayerProfileRequest"];
          "application/*+json": components["schemas"]["UpdatePlayerProfileRequest"];
        };
      };
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["PlayerProfileResponse"];
            "application/json": components["schemas"]["PlayerProfileResponse"];
            "text/json": components["schemas"]["PlayerProfileResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    post: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          personId: string;
        };
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["CreatePlayerProfileRequest"];
          "text/json": components["schemas"]["CreatePlayerProfileRequest"];
          "application/*+json": components["schemas"]["CreatePlayerProfileRequest"];
        };
      };
      responses: {
        /** @description Created */
        201: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["PlayerProfileResponse"];
            "application/json": components["schemas"]["PlayerProfileResponse"];
            "text/json": components["schemas"]["PlayerProfileResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
        /** @description Conflict */
        409: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/teams/{teamId}/roster": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["RosterEntryResponse"][];
            "application/json": components["schemas"]["RosterEntryResponse"][];
            "text/json": components["schemas"]["RosterEntryResponse"][];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
        };
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["CreateRosterEntryRequest"];
          "text/json": components["schemas"]["CreateRosterEntryRequest"];
          "application/*+json": components["schemas"]["CreateRosterEntryRequest"];
        };
      };
      responses: {
        /** @description Created */
        201: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["RosterEntryResponse"];
            "application/json": components["schemas"]["RosterEntryResponse"];
            "text/json": components["schemas"]["RosterEntryResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
        /** @description Conflict */
        409: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/teams/{teamId}/roster/{rosterEntryId}": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          teamId: string;
          rosterEntryId: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["RosterEntryResponse"];
            "application/json": components["schemas"]["RosterEntryResponse"];
            "text/json": components["schemas"]["RosterEntryResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post?: never;
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/teams": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path?: never;
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["TeamResponse"][];
            "application/json": components["schemas"]["TeamResponse"][];
            "text/json": components["schemas"]["TeamResponse"][];
          };
        };
      };
    };
    put?: never;
    post: {
      parameters: {
        query?: never;
        header?: never;
        path?: never;
        cookie?: never;
      };
      requestBody?: {
        content: {
          "application/json": components["schemas"]["CreateTeamRequest"];
          "text/json": components["schemas"]["CreateTeamRequest"];
          "application/*+json": components["schemas"]["CreateTeamRequest"];
        };
      };
      responses: {
        /** @description Created */
        201: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["TeamResponse"];
            "application/json": components["schemas"]["TeamResponse"];
            "text/json": components["schemas"]["TeamResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
      };
    };
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
  "/api/teams/{id}": {
    parameters: {
      query?: never;
      header?: never;
      path?: never;
      cookie?: never;
    };
    get: {
      parameters: {
        query?: never;
        header?: never;
        path: {
          id: string;
        };
        cookie?: never;
      };
      requestBody?: never;
      responses: {
        /** @description OK */
        200: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["TeamResponse"];
            "application/json": components["schemas"]["TeamResponse"];
            "text/json": components["schemas"]["TeamResponse"];
          };
        };
        /** @description Bad Request */
        400: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ValidationProblemDetails"];
            "application/json": components["schemas"]["ValidationProblemDetails"];
            "text/json": components["schemas"]["ValidationProblemDetails"];
          };
        };
        /** @description Not Found */
        404: {
          headers: {
            [name: string]: unknown;
          };
          content: {
            "text/plain": components["schemas"]["ProblemDetails"];
            "application/json": components["schemas"]["ProblemDetails"];
            "text/json": components["schemas"]["ProblemDetails"];
          };
        };
      };
    };
    put?: never;
    post?: never;
    delete?: never;
    options?: never;
    head?: never;
    patch?: never;
    trace?: never;
  };
}
export type webhooks = Record<string, never>;
export interface components {
  schemas: {
    CreateMembershipRequest: {
      /** Format: uuid */
      personId: string;
      teamRole: string;
    };
    CreatePersonRequest: {
      firstName: string;
      lastName: string;
    };
    CreatePlayerProfileRequest: {
      dominantFoot?: string | null;
      /** Format: int32 */
      heightInches?: number | null;
      /** Format: double */
      weightPounds?: number | null;
    };
    CreateRosterEntryRequest: {
      /** Format: uuid */
      teamMembershipId: string;
      jerseyNumber?: string | null;
      rosterStatus: string;
    };
    CreateTeamRequest: {
      name: string;
    };
    MembershipPersonResponse: {
      /** Format: uuid */
      id: string;
      firstName: string;
      lastName: string;
    };
    MembershipResponse: {
      /** Format: uuid */
      id: string;
      /** Format: uuid */
      teamId: string;
      person: components["schemas"]["MembershipPersonResponse"];
      teamRole: string;
    };
    PersonResponse: {
      /** Format: uuid */
      id?: string;
      firstName?: string | null;
      lastName?: string | null;
    };
    PlayerProfileResponse: {
      /** Format: uuid */
      id?: string;
      /** Format: uuid */
      personId?: string;
      dominantFoot?: string | null;
      /** Format: int32 */
      heightInches?: number | null;
      /** Format: double */
      weightPounds?: number | null;
    };
    ProblemDetails: {
      type?: string | null;
      title?: string | null;
      /** Format: int32 */
      status?: number | null;
      detail?: string | null;
      instance?: string | null;
    } & {
      [key: string]: unknown;
    };
    RosterEntryResponse: {
      /** Format: uuid */
      id: string;
      /** Format: uuid */
      teamMembershipId: string;
      person: components["schemas"]["RosterPersonResponse"];
      jerseyNumber: string | null;
      rosterStatus: string;
    };
    RosterPersonResponse: {
      /** Format: uuid */
      id: string;
      firstName: string;
      lastName: string;
    };
    TeamResponse: {
      /** Format: uuid */
      id: string;
      name: string;
    };
    UpdatePlayerProfileRequest: {
      dominantFoot: string | null;
      /** Format: int32 */
      heightInches: number | null;
      /** Format: double */
      weightPounds: number | null;
    };
    ValidationProblemDetails: {
      type?: string | null;
      title?: string | null;
      /** Format: int32 */
      status?: number | null;
      detail?: string | null;
      instance?: string | null;
      errors?: {
        [key: string]: string[];
      } | null;
    } & {
      [key: string]: unknown;
    };
  };
  responses: never;
  parameters: never;
  requestBodies: never;
  headers: never;
  pathItems: never;
}
export type $defs = Record<string, never>;
export type operations = Record<string, never>;
