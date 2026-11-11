import type { ReactNode } from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import RosterPage from "@/app/teams/[teamId]/roster/page";
import { WorkspaceShell } from "@/components/workspace-shell";
import type { components } from "@/lib/api/schema";
import { RosterList } from "./team-roster";

const route = vi.hoisted(() => ({ pathname: "/" }));
vi.mock("next/navigation", () => ({ usePathname: () => route.pathname }));
const teamA = "ad828d12-92c5-4578-985b-e83bb37cc861";
const teamB = "bd828d12-92c5-4578-985b-e83bb37cc861";
const entries = [
  {
    id: "63f1dd2c-5d81-4242-b6c4-94e95368d6ea",
    teamMembershipId: "5d2f687c-78fd-43e9-a0e2-ab7d73ed83ad",
    person: { id: "2cfeea39-8f80-4266-bdd4-8ac5b6ba0abd", firstName: "Alex", lastName: "Rivera" },
    jerseyNumber: "007",
    rosterStatus: "Active",
  },
  {
    id: "6512c06f-a6c4-42b8-ae87-07e29aa157fd",
    teamMembershipId: "6d2f687c-78fd-43e9-a0e2-ab7d73ed83ad",
    person: { id: "3cfeea39-8f80-4266-bdd4-8ac5b6ba0abd", firstName: "Alex", lastName: "Rivera" },
    jerseyNumber: null,
    rosterStatus: "Reserved",
  },
  {
    id: "7512c06f-a6c4-42b8-ae87-07e29aa157fd",
    teamMembershipId: "7d2f687c-78fd-43e9-a0e2-ab7d73ed83ad",
    person: { id: "4cfeea39-8f80-4266-bdd4-8ac5b6ba0abd", firstName: "Sam", lastName: "Lee" },
    jerseyNumber: "0",
    rosterStatus: "Suspended",
  },
] satisfies components["schemas"]["RosterEntryResponse"][];

afterEach(() => {
  vi.unstubAllGlobals();
  route.pathname = "/";
});

function setup(content: ReactNode) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return render(content, {
    wrapper: ({ children }) => (
      <QueryClientProvider client={client}>
        <WorkspaceShell>{children}</WorkspaceShell>
      </QueryClientProvider>
    ),
  });
}

async function page(teamId = teamA) {
  route.pathname = `/teams/${teamId}/roster`;
  return RosterPage({ params: Promise.resolve({ teamId }) });
}

test("deep-linked roster shows selected Team, distinct same-name players, strings and every status", async () => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    if (path === `/api/teams/${teamA}/roster`) return Promise.resolve(Response.json(entries));
    expect(path).toBe(`/api/teams/${teamA}`);
    return Promise.resolve(Response.json({ id: teamA, name: "Touchline FC" }));
  });
  vi.stubGlobal("fetch", fetcher);
  setup(await page());
  expect(await screen.findByRole("heading", { name: "Touchline FC" })).toBeVisible();
  const list = await screen.findByRole("list", { name: "Roster entries" });
  const rows = within(list).getAllByRole("listitem");
  expect(rows).toHaveLength(3);
  expect(within(list).getAllByRole("heading", { name: "Alex Rivera" })).toHaveLength(2);
  expect(within(rows[0]).getByText("007")).toBeVisible();
  expect(within(rows[0]).getByText("Active")).toBeVisible();
  expect(within(rows[1]).getByText("Not assigned")).toBeVisible();
  expect(within(rows[1]).getByText("Reserved")).toBeVisible();
  expect(within(rows[2]).getByText("0")).toBeVisible();
  expect(within(rows[2]).getByText("Suspended")).toBeVisible();
  const nav = screen.getByRole("navigation", { name: "Selected Team" });
  const rosterLink = within(nav).getByRole("link", { name: "Roster" });
  expect(rosterLink).toHaveAttribute("href", `/teams/${teamA}/roster`);
  expect(rosterLink).toHaveAttribute("aria-current", "page");
  const overview = within(nav).getByRole("link", { name: "Overview" });
  expect(overview).toHaveAttribute("href", `/teams/${teamA}`);
  expect(overview).not.toHaveAttribute("aria-current");
  expect(within(nav).getByText(/Members/)).toHaveAttribute("aria-disabled", "true");
  expect(within(nav).getAllByRole("link")).toHaveLength(2);
  const user = userEvent.setup();
  for (let i = 0; i < 5; i++) await user.tab();
  expect(rosterLink).toHaveFocus();
  await user.tab();
  expect(screen.getByRole("link", { name: /All Teams/ })).toHaveFocus();
  expect(screen.getByRole("link", { name: /All Teams/ })).toHaveAttribute("href", "/");
  expect(screen.queryByRole("button")).not.toBeInTheDocument();
  expect(fetcher).toHaveBeenCalledTimes(2);
});

test("reordering same-name entries preserves each API ID's row identity", () => {
  const view = render(<RosterList entries={entries} />);
  const rows = screen.getAllByRole("listitem");
  view.rerender(<RosterList entries={[entries[1], entries[0], entries[2]]} />);
  const reordered = screen.getAllByRole("listitem");
  expect(reordered[0]).toBe(rows[1]);
  expect(reordered[1]).toBe(rows[0]);
  expect(within(reordered[1]).getByText("007")).toBeVisible();
});

test("an omitted jersey in a mocked response also shows Not assigned without inventing zero", async () => {
  const { jerseyNumber: _jerseyNumber, ...withoutJersey } = entries[0];
  expect(_jerseyNumber).toBe("007");
  vi.stubGlobal(
    "fetch",
    vi
      .fn<typeof fetch>()
      .mockImplementation((input) =>
        Promise.resolve(
          Response.json(
            new URL((input as Request).url).pathname.endsWith("/roster")
              ? [withoutJersey]
              : { id: teamA, name: "Touchline FC" },
          ),
        ),
      ),
  );
  setup(await page());
  expect(await screen.findByText("Not assigned")).toBeVisible();
  expect(screen.queryByText("0")).not.toBeInTheDocument();
});

test("roster loading, failure and keyboard retry to empty are distinct", async () => {
  let respond!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    if (new URL((input as Request).url).pathname.endsWith("/roster")) {
      return new Promise<Response>((resolve) => {
        respond = resolve;
      });
    }
    return Promise.resolve(Response.json({ id: teamA, name: "Touchline FC" }));
  });
  vi.stubGlobal("fetch", fetcher);
  setup(await page());
  expect(await screen.findByText("Loading roster…")).toHaveAttribute("role", "status");
  expect(screen.queryByText(/No roster entries/)).not.toBeInTheDocument();
  respond(new Response(null, { status: 500 }));
  expect(await screen.findByRole("alert")).toHaveTextContent("Roster could not be loaded");
  expect(screen.queryByText(/No roster entries/)).not.toBeInTheDocument();
  const retry = screen.getByRole("button", { name: "Retry roster" });
  retry.focus();
  await userEvent.setup().keyboard("{Enter}");
  expect(await screen.findByText("Loading roster…")).toHaveAttribute("role", "status");
  respond(Response.json([]));
  expect(await screen.findByText("No roster entries yet for this Team.")).toHaveAttribute(
    "role",
    "status",
  );
  expect(screen.queryByRole("alert")).not.toBeInTheDocument();
  expect(screen.queryByRole("list", { name: "Roster entries" })).not.toBeInTheDocument();
  expect(fetcher).toHaveBeenCalledTimes(3);
});

test("switching Team URLs clears previous players and updates navigation, returning reuses the cache", async () => {
  let respondB!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    const id = path.split("/")[3];
    if (path === `/api/teams/${teamB}/roster`) {
      return new Promise<Response>((resolve) => {
        respondB = resolve;
      });
    }
    return Promise.resolve(
      Response.json(
        path.endsWith("/roster")
          ? entries
          : {
              id,
              name: id === teamA ? "Team A" : "Team B",
            },
      ),
    );
  });
  vi.stubGlobal("fetch", fetcher);
  const view = setup(await page());
  expect(await screen.findByText("007")).toBeVisible();
  view.rerender(await page(teamB));
  expect(screen.queryByText("007")).not.toBeInTheDocument();
  expect(await screen.findByRole("heading", { name: "Team B" })).toBeVisible();
  expect(await screen.findByText("Loading roster…")).toBeVisible();
  expect(screen.getByRole("link", { name: "Roster" })).toHaveAttribute(
    "href",
    `/teams/${teamB}/roster`,
  );
  respondB(Response.json([]));
  expect(await screen.findByText(/No roster entries/)).toBeVisible();
  view.rerender(await page());
  expect(await screen.findByText("007")).toBeVisible();
  expect(screen.getByRole("heading", { name: "Team A" })).toBeVisible();
  expect(fetcher).toHaveBeenCalledTimes(4);
});

test.each(["bad-link", teamA])(
  "invalid or nonexistent Team %s never requests a roster",
  async (id) => {
    const fetcher = vi.fn<typeof fetch>().mockResolvedValue(new Response(null, { status: 404 }));
    vi.stubGlobal("fetch", fetcher);
    setup(await page(id));
    if (id === "bad-link") {
      expect(screen.getByRole("heading", { name: "Invalid Team link" })).toBeVisible();
      expect(fetcher).not.toHaveBeenCalled();
    } else {
      expect(await screen.findByText(/This Team was not found/)).toBeVisible();
      expect(fetcher).toHaveBeenCalledTimes(1);
    }
    expect(screen.queryByRole("navigation", { name: "Selected Team" })).not.toBeInTheDocument();
    expect(screen.getByRole("link", { name: /All Teams/ })).toHaveAttribute("href", "/");
  },
);
