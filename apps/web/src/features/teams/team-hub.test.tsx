import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import TeamPage from "@/app/teams/[teamId]/page";
import { WorkspaceShell } from "@/components/workspace-shell";
import { TeamDetail } from "./team-detail";
import { TeamOverview } from "./team-overview";
import type { paths } from "@/lib/api/schema";

const route = vi.hoisted(() => ({ pathname: "/" }));
vi.mock("next/navigation", () => ({ usePathname: () => route.pathname }));
const teamA = "ad828d12-92c5-4578-985b-e83bb37cc861";
const teamB = "bd828d12-92c5-4578-985b-e83bb37cc861";
const memberships = [
  {
    id: "5d2f687c-78fd-43e9-a0e2-ab7d73ed83ad",
    teamId: teamA,
    person: { id: "2cfeea39-8f80-4266-bdd4-8ac5b6ba0abd", firstName: "Alex", lastName: "Rivera" },
    teamRole: "Coach",
  },
] satisfies paths["/api/teams/{teamId}/memberships"]["get"]["responses"][200]["content"]["application/json"];
const roster = [
  {
    id: "63f1dd2c-5d81-4242-b6c4-94e95368d6ea",
    teamMembershipId: memberships[0].id,
    person: memberships[0].person,
    jerseyNumber: "7",
    rosterStatus: "Active",
  },
  {
    id: "6512c06f-a6c4-42b8-ae87-07e29aa157fd",
    teamMembershipId: memberships[0].id,
    person: memberships[0].person,
    jerseyNumber: null,
    rosterStatus: "Inactive",
  },
] satisfies paths["/api/teams/{teamId}/roster"]["get"]["responses"][200]["content"]["application/json"];

afterEach(() => {
  vi.unstubAllGlobals();
  route.pathname = "/";
});

function setup(content: React.ReactNode) {
  const client = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={client}>
      <WorkspaceShell>{children}</WorkspaceShell>
    </QueryClientProvider>
  );
  return render(content, { wrapper });
}

test("deep-linked Overview has keyboard navigation in the existing sidebar and honest totals", async () => {
  route.pathname = `/teams/${teamA}`;
  vi.stubGlobal(
    "fetch",
    vi.fn<typeof fetch>().mockImplementation((input) => {
      const path = new URL((input as Request).url).pathname;
      if (path.endsWith("/memberships")) return Promise.resolve(Response.json(memberships));
      if (path.endsWith("/roster")) return Promise.resolve(Response.json(roster));
      expect(path).toBe(`/api/teams/${teamA}`);
      return Promise.resolve(Response.json({ id: teamA, name: "Touchline FC" }));
    }),
  );
  setup(await TeamPage({ params: Promise.resolve({ teamId: teamA }) }));
  const nav = screen.getByRole("navigation", { name: "Selected Team" });
  const overview = within(nav).getByRole("link", { name: "Overview" });
  expect(overview).toHaveAttribute("aria-current", "page");
  expect(overview).toHaveAttribute("href", `/teams/${teamA}`);
  expect(nav.closest("aside")).toBe(screen.getByRole("complementary"));
  expect(within(nav).getAllByRole("link")).toHaveLength(1);
  expect(within(nav).getByText(/Roster/)).toHaveAttribute("aria-disabled", "true");
  expect(within(nav).getByText(/Members/)).toHaveAttribute("aria-disabled", "true");
  const user = userEvent.setup();
  for (let i = 0; i < 4; i++) await user.tab();
  expect(overview).toHaveFocus();
  await user.tab();
  expect(screen.getByRole("link", { name: /All Teams/ })).toHaveFocus();
  expect(await screen.findByLabelText("Memberships: 1")).toBeVisible();
  expect(await screen.findByLabelText("Roster entries: 2")).toBeVisible();
  expect(screen.getByText("Total roster entries across all statuses.")).toBeVisible();
  expect(screen.queryByText("Alex")).not.toBeInTheDocument();
});

test("summary loading and failure stay distinct from zero and keyboard retry preserves other counts", async () => {
  let resolveRoster!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    if (path.endsWith("/memberships")) return Promise.resolve(Response.json([]));
    return new Promise<Response>((resolve) => {
      resolveRoster = resolve;
    });
  });
  vi.stubGlobal("fetch", fetcher);
  setup(<TeamOverview teamId={teamA} />);
  const roster = screen.getByRole("region", { name: "Roster entries" });
  expect(within(roster).getByRole("status")).toHaveTextContent("Loading roster entries");
  expect(within(roster).queryByLabelText("Roster entries: 0")).not.toBeInTheDocument();
  resolveRoster(new Response(null, { status: 500 }));
  expect(await within(roster).findByRole("alert")).toHaveTextContent("could not be loaded");
  expect(await screen.findByLabelText("Memberships: 0")).toBeVisible();
  expect(screen.getByText("No memberships yet.")).toBeVisible();
  fetcher.mockImplementation(() => Promise.resolve(Response.json([])));
  screen.getByRole("button", { name: "Retry roster entries" }).focus();
  await userEvent.setup().keyboard("{Enter}");
  expect(await within(roster).findByLabelText("Roster entries: 0")).toBeVisible();
  expect(fetcher).toHaveBeenCalledTimes(3);
});

test("switching URL IDs never shows the previous Team's summaries and reuses ID-keyed caches", async () => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    const id = path.split("/")[3];
    if (path.endsWith("/memberships"))
      return Promise.resolve(Response.json(id === teamA ? memberships : []));
    if (path.endsWith("/roster")) return Promise.resolve(Response.json([]));
    return Promise.resolve(Response.json({ id, name: id === teamA ? "Team A" : "Team B" }));
  });
  vi.stubGlobal("fetch", fetcher);
  const content = (id: string) => (
    <TeamDetail teamId={id}>
      <TeamOverview teamId={id} />
    </TeamDetail>
  );
  route.pathname = `/teams/${teamA}`;
  const view = setup(content(teamA));
  expect(await screen.findByLabelText("Memberships: 1")).toBeVisible();
  route.pathname = `/teams/${teamB}`;
  view.rerender(content(teamB));
  expect(screen.queryByLabelText("Memberships: 1")).not.toBeInTheDocument();
  expect(await screen.findByRole("heading", { name: "Team B" })).toBeVisible();
  expect(await screen.findByLabelText("Memberships: 0")).toBeVisible();
  expect(screen.getByRole("link", { name: "Overview" })).toHaveAttribute("href", `/teams/${teamB}`);
  route.pathname = `/teams/${teamA}`;
  view.rerender(content(teamA));
  expect(await screen.findByLabelText("Memberships: 1")).toBeVisible();
  expect(fetcher).toHaveBeenCalledTimes(6);
  route.pathname = "/";
  view.rerender(<h1>Workspace</h1>);
  expect(screen.queryByRole("navigation", { name: "Selected Team" })).not.toBeInTheDocument();
  expect(screen.getByRole("link", { name: "Workspace" })).toHaveAttribute("aria-current", "page");
});

test.each(["/teams/new", "/teams/bad-link"])("%s has no contextual links", (pathname) => {
  route.pathname = pathname;
  setup(<h1>Workspace</h1>);
  expect(screen.queryByRole("navigation", { name: "Selected Team" })).not.toBeInTheDocument();
});
