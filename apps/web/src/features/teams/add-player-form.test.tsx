import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import { TeamMembers } from "./team-members";
import { TeamOverview } from "./team-overview";
import { TeamRoster } from "./team-roster";
import { teamMembershipsQueryKey } from "./use-team-memberships";
import { teamRosterQueryKey } from "./use-team-roster";

const team = "ad828d12-92c5-4578-985b-e83bb37cc861";
const otherTeam = "bd828d12-92c5-4578-985b-e83bb37cc861";
const player = {
  id: "roster-1",
  person: { id: "person-1", firstName: "Alex", lastName: "Rivera" },
  teamMembershipId: "member-1",
  jerseyNumber: "007",
  rosterStatus: "Reserved",
};
const candidate = { personId: "person-1", firstName: "Alex", lastName: "Rivera" };
const postPath = `/api/teams/${team}/roster/players`;

afterEach(() => vi.unstubAllGlobals());

async function open(fetcher: typeof fetch, showSummaries = false) {
  vi.stubGlobal("fetch", fetcher);
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false }, mutations: { retry: 2 } },
  });
  client.setQueryData(teamRosterQueryKey(otherTeam), [player]);
  client.setQueryData(teamMembershipsQueryKey(otherTeam), []);
  render(
    <QueryClientProvider client={client}>
      <TeamRoster teamId={team} />
      {showSummaries && (
        <>
          <TeamMembers teamId={team} />
          <TeamOverview teamId={team} />
        </>
      )}
    </QueryClientProvider>,
  );
  const user = userEvent.setup();
  const opener = screen.getByRole("button", { name: "Add Player" });
  opener.focus();
  await user.keyboard("{Enter}");
  expect(screen.getByRole("heading", { name: "Add Player" })).toHaveFocus();
  return { user, client };
}

test("creates a new Person with one pending POST and refreshes roster, Members and totals", async () => {
  let committed = false;
  let complete!: () => void;
  const posts: unknown[] = [];
  const fetcher = vi.fn<typeof fetch>(async (input) => {
    const request = input as Request;
    const path = new URL(request.url).pathname;
    if (request.method === "POST") {
      expect(path).toBe(postPath);
      posts.push(await request.json());
      await new Promise<void>((resolve) => {
        complete = resolve;
      });
      committed = true;
      return Response.json(player, { status: 201 });
    }
    if (path.endsWith("/memberships")) {
      return Response.json(
        committed ? [{ id: "member-1", teamRole: "Player", person: player.person }] : [],
      );
    }
    return Response.json(committed ? [player] : []);
  });
  const { user, client } = await open(fetcher, true);
  await screen.findByLabelText("Roster entries: 0");
  await screen.findByLabelText("Memberships: 0");
  await user.tab();
  expect(screen.getByRole("radio", { name: "Create New Person" })).toHaveFocus();
  await user.tab();
  expect(screen.getByLabelText("First name")).toHaveFocus();
  await user.type(screen.getByLabelText("First name"), " Alex ");
  await user.type(screen.getByLabelText("Last name"), "Rivera");
  await user.type(screen.getByLabelText("Jersey number (optional)"), "007");
  await user.selectOptions(screen.getByLabelText("Roster status"), "Reserved");

  const form = screen.getByRole("form", { name: "Add Player" });
  fireEvent.submit(form);
  fireEvent.submit(form);
  await waitFor(() =>
    expect(posts).toEqual([
      {
        newPerson: { firstName: "Alex", lastName: "Rivera" },
        jerseyNumber: "007",
        rosterStatus: "Reserved",
      },
    ]),
  );
  expect(screen.getByRole("button", { name: "Add to roster" })).toBeDisabled();
  expect(screen.getByLabelText("First name")).toBeDisabled();
  complete();

  expect(await screen.findByText("Alex Rivera was added to the roster.")).toHaveAttribute(
    "role",
    "status",
  );
  expect(screen.getByRole("button", { name: "Add Player" })).toHaveFocus();
  expect(
    within(screen.getByRole("list", { name: "Roster entries" })).getByText("007"),
  ).toBeVisible();
  expect(
    await within(screen.getByRole("list", { name: "Team members" })).findByText("Player"),
  ).toBeVisible();
  expect(await screen.findByLabelText("Roster entries: 1")).toBeVisible();
  expect(await screen.findByLabelText("Memberships: 1")).toBeVisible();
  expect(client.getQueryState(teamRosterQueryKey(otherTeam))?.isInvalidated).toBe(false);
  expect(client.getQueryState(teamMembershipsQueryKey(otherTeam))?.isInvalidated).toBe(false);
});

test("keeps same-name choices distinct across opaque pages and clears selection when the name changes", async () => {
  const cursor = "opaque+/= &";
  const searches: [string | null, string | null][] = [];
  const posts: unknown[] = [];
  const fetcher = vi.fn<typeof fetch>(async (input) => {
    const request = input as Request;
    const url = new URL(request.url);
    if (url.pathname === postPath) {
      posts.push(await request.json());
      return Response.json(player, { status: 201 });
    }
    if (!url.pathname.endsWith("/candidates")) return Response.json([]);
    const q = url.searchParams.get("q");
    const passedCursor = url.searchParams.get("cursor");
    searches.push([q, passedCursor]);
    expect(url.searchParams.get("limit")).toBe("20");
    if (q !== "Al") return Response.json({ items: [], nextCursor: null });
    return passedCursor
      ? Response.json({
          items: [candidate, { ...candidate, personId: "person-2" }],
          nextCursor: null,
        })
      : Response.json({ items: [candidate], nextCursor: cursor });
  });
  const { user } = await open(fetcher);
  await user.click(screen.getByRole("radio", { name: "Select Existing Person" }));
  const search = screen.getByRole("searchbox", { name: "Search by name" });
  await user.type(search, "A");
  expect(searches).toEqual([]);
  await user.type(search, "l");
  const first = await screen.findByRole("radio", { name: "Alex Rivera" });
  expect(first).not.toBeChecked();
  await user.click(screen.getByRole("button", { name: "Add to roster" }));
  expect(first).toHaveFocus();
  expect(screen.getByRole("alert")).toHaveTextContent("Select a person");
  expect(posts).toEqual([]);

  await user.click(first);
  await user.click(screen.getByRole("button", { name: "Load more" }));
  await waitFor(() =>
    expect(screen.getAllByRole("radio", { name: "Alex Rivera" })).toHaveLength(2),
  );
  expect(first).toBeChecked();
  expect(screen.getByText(/People can share the same name/)).toBeVisible();
  expect(searches).toEqual([
    ["Al", null],
    ["Al", cursor],
  ]);
  expect(screen.queryByRole("button", { name: "Load more" })).not.toBeInTheDocument();
  await user.type(search, "z");
  expect(await screen.findByText(/No people match/)).toBeVisible();
  expect(screen.queryByRole("radio", { name: "Alex Rivera" })).not.toBeInTheDocument();
  await user.type(search, "{Backspace}");
  expect(await screen.findAllByRole("radio", { name: "Alex Rivera" })).toHaveLength(1);
  expect(screen.getByRole("radio", { name: "Alex Rivera" })).not.toBeChecked();
  await user.click(screen.getByRole("button", { name: "Load more" }));
  const [, second] = await screen.findAllByRole("radio", { name: "Alex Rivera" });
  await user.click(second);
  await user.selectOptions(screen.getByLabelText("Roster status"), "Suspended");
  await user.click(screen.getByRole("button", { name: "Add to roster" }));
  await waitFor(() =>
    expect(posts).toEqual([
      { personId: "person-2", jerseyNumber: null, rosterStatus: "Suspended" },
    ]),
  );
  expect(await screen.findByText(/was added to the roster/)).toBeVisible();
});

test("announces candidate loading and permits keyboard recovery from a read error", async () => {
  let respond!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>((input) =>
    (input as Request).url.includes("/candidates")
      ? new Promise<Response>((resolve) => {
          respond = resolve;
        })
      : Promise.resolve(Response.json([])),
  );
  const { user } = await open(fetcher);
  await user.click(screen.getByRole("radio", { name: "Select Existing Person" }));
  await user.type(screen.getByRole("searchbox"), "Al");
  expect(await screen.findByText("Searching for people…")).toHaveAttribute("role", "status");
  respond(new Response(null, { status: 503 }));
  expect(await screen.findByRole("alert")).toHaveTextContent("People could not be loaded");
  screen.getByRole("button", { name: "Retry search" }).focus();
  await user.keyboard("{Enter}");
  expect(await screen.findByText("Searching for people…")).toBeVisible();
  respond(Response.json({ items: [], nextCursor: null }));
  expect(await screen.findByText(/No people match/)).toBeVisible();
  screen.getByRole("button", { name: "Cancel" }).focus();
  await user.keyboard("{Enter}");
  expect(screen.getByRole("button", { name: "Add Player" })).toHaveFocus();
});

test.each([
  [400, /API rejected the player details/],
  [404, /Team or selected person could not be found/],
  [409, /may already be on the roster or have a different Team role/],
])("HTTP %s explains recovery and retains the entered player", async (status, message) => {
  const { user } = await open(async (input) =>
    (input as Request).method === "POST" ? new Response(null, { status }) : Response.json([]),
  );
  await user.type(screen.getByLabelText("First name"), "Alex");
  await user.type(screen.getByLabelText("Last name"), "Rivera");
  await user.type(screen.getByLabelText("Jersey number (optional)"), "007");
  await user.click(screen.getByRole("button", { name: "Add to roster" }));
  expect(await screen.findByRole("alert")).toHaveTextContent(message);
  expect(screen.getByLabelText("First name")).toHaveValue("Alex");
  expect(screen.getByLabelText("Last name")).toHaveValue("Rivera");
  expect(screen.getByLabelText("Jersey number (optional)")).toHaveValue("007");
  expect(screen.getByRole("button", { name: "Add to roster" })).toBeEnabled();
});

test("lost write response requires a successful roster refresh without repeating the POST", async () => {
  let posts = 0;
  const { user } = await open(async (input) => {
    if ((input as Request).method === "POST") {
      posts++;
      throw new TypeError("Network error");
    }
    return Response.json(posts ? [player] : []);
  });
  await user.type(screen.getByLabelText("First name"), "Alex");
  await user.type(screen.getByLabelText("Last name"), "Rivera");
  await user.click(screen.getByRole("button", { name: "Add to roster" }));
  expect(await screen.findByRole("alert")).toHaveTextContent(/did not confirm/);
  expect(screen.getByRole("button", { name: "Add to roster" })).toBeDisabled();
  expect(screen.getByLabelText("First name")).toHaveValue("Alex");
  await user.click(screen.getByRole("button", { name: "Refresh roster" }));
  expect(await screen.findByText(/Roster refreshed below/)).toBeVisible();
  expect(await screen.findByRole("heading", { name: "Alex Rivera" })).toBeVisible();
  expect(screen.getByRole("button", { name: "Add to roster" })).toBeEnabled();
  expect(posts).toBe(1);
});
