import type { ReactNode } from "react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import MembersPage from "@/app/teams/[teamId]/members/page";
import { WorkspaceShell } from "@/components/workspace-shell";
import type { components } from "@/lib/api/schema";
import { MembersList } from "./team-members";

const route = vi.hoisted(() => ({ pathname: "/" }));
vi.mock("next/navigation", () => ({ usePathname: () => route.pathname }));
const teamA = "ad828d12-92c5-4578-985b-e83bb37cc861";
const teamB = "bd828d12-92c5-4578-985b-e83bb37cc861";
const roles = ["Owner", "Coach", "AssistantCoach", "Manager", "Player", "Viewer"];
const labels = ["Owner", "Coach", "Assistant Coach", "Manager", "Player", "Viewer"];
const memberships: components["schemas"]["MembershipResponse"][] = roles.map((teamRole, index) => ({
  id: `membership-${index}`,
  teamId: teamA,
  person: { id: `person-${index}`, firstName: "Alex", lastName: "Rivera" },
  teamRole,
}));

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
  route.pathname = `/teams/${teamId}/members`;
  return MembersPage({ params: Promise.resolve({ teamId }) });
}

test("deep-linked members shows all roles and same-name people with working keyboard navigation", async () => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    if (path === `/api/teams/${teamA}/memberships`)
      return Promise.resolve(Response.json(memberships));
    expect(path).toBe(`/api/teams/${teamA}`);
    return Promise.resolve(Response.json({ id: teamA, name: "Touchline FC" }));
  });
  vi.stubGlobal("fetch", fetcher);
  setup(await page());
  expect(await screen.findByRole("heading", { name: "Touchline FC" })).toBeVisible();
  const list = await screen.findByRole("list", { name: "Team members" });
  const rows = within(list).getAllByRole("listitem");
  expect(rows).toHaveLength(6);
  expect(within(list).getAllByRole("heading", { name: "Alex Rivera" })).toHaveLength(6);
  rows.forEach((row, index) => {
    expect(within(row).getByText("Team role")).toBeVisible();
    expect(within(row).getByText(labels[index])).toBeVisible();
  });
  expect(screen.getByText(/Membership is separate from player roster participation/)).toBeVisible();
  expect(screen.getByText(/do not grant the current browser user permissions/)).toBeVisible();
  const nav = screen.getByRole("navigation", { name: "Selected Team" });
  const members = within(nav).getByRole("link", { name: "Members" });
  expect(members).toHaveAttribute("href", `/teams/${teamA}/members`);
  expect(members).toHaveAttribute("aria-current", "page");
  for (const [name, suffix] of [
    ["Overview", ""],
    ["Roster", "/roster"],
  ]) {
    const link = within(nav).getByRole("link", { name });
    expect(link).toHaveAttribute("href", `/teams/${teamA}${suffix}`);
    expect(link).not.toHaveAttribute("aria-current");
  }
  const user = userEvent.setup();
  for (let i = 0; i < 6; i++) await user.tab();
  expect(members).toHaveFocus();
  await user.tab();
  expect(screen.getByRole("link", { name: /All Teams/ })).toHaveFocus();
  expect(screen.getByRole("link", { name: /All Teams/ })).toHaveAttribute("href", "/");
  expect(screen.queryByRole("button")).not.toBeInTheDocument();
  expect(fetcher).toHaveBeenCalledTimes(2);
});

test("reordering same-name memberships preserves each membership ID's row", () => {
  const view = render(<MembersList memberships={memberships} />);
  const rows = screen.getAllByRole("listitem");
  view.rerender(<MembersList memberships={[memberships[1], memberships[0]]} />);
  const reordered = screen.getAllByRole("listitem");
  expect(reordered[0]).toBe(rows[1]);
  expect(reordered[1]).toBe(rows[0]);
  expect(within(reordered[1]).getByText("Owner")).toBeVisible();
});

test("unrecognized role strings stay visible without being interpreted as permissions", () => {
  render(<MembersList memberships={[{ ...memberships[0], teamRole: "FutureRole" }]} />);
  expect(screen.getByText("FutureRole")).toBeVisible();
});

test("loading, API failure and keyboard retry to empty stay distinct", async () => {
  let respond!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    if (new URL((input as Request).url).pathname.endsWith("/memberships")) {
      return new Promise<Response>((resolve) => {
        respond = resolve;
      });
    }
    return Promise.resolve(Response.json({ id: teamA, name: "Touchline FC" }));
  });
  vi.stubGlobal("fetch", fetcher);
  setup(await page());
  expect(await screen.findByText("Loading members…")).toHaveAttribute("role", "status");
  expect(screen.queryByText(/No members yet/)).not.toBeInTheDocument();
  respond(new Response(null, { status: 500 }));
  expect(await screen.findByRole("alert")).toHaveTextContent("Members could not be loaded");
  expect(screen.queryByText(/No members yet/)).not.toBeInTheDocument();
  const retry = screen.getByRole("button", { name: "Retry members" });
  retry.focus();
  await userEvent.setup().keyboard("{Enter}");
  expect(await screen.findByText("Loading members…")).toHaveAttribute("role", "status");
  respond(Response.json([]));
  expect(await screen.findByText("No members yet for this Team.")).toHaveAttribute(
    "role",
    "status",
  );
  expect(screen.queryByRole("alert")).not.toBeInTheDocument();
  expect(screen.queryByRole("list", { name: "Team members" })).not.toBeInTheDocument();
  expect(fetcher).toHaveBeenCalledTimes(3);
});

test("switching Team URLs clears previous members and navigation, returning reuses the cache", async () => {
  let respondB!: (response: Response) => void;
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    const id = path.split("/")[3];
    if (path === `/api/teams/${teamB}/memberships`) {
      return new Promise<Response>((resolve) => {
        respondB = resolve;
      });
    }
    return Promise.resolve(
      Response.json(
        path.endsWith("/memberships")
          ? memberships
          : {
              id,
              name: id === teamA ? "Team A" : "Team B",
            },
      ),
    );
  });
  vi.stubGlobal("fetch", fetcher);
  const view = setup(await page());
  expect(await screen.findByRole("list", { name: "Team members" })).toBeVisible();
  view.rerender(await page(teamB));
  expect(screen.queryByText("Alex Rivera")).not.toBeInTheDocument();
  expect(await screen.findByRole("heading", { name: "Team B" })).toBeVisible();
  expect(await screen.findByText("Loading members…")).toBeVisible();
  expect(screen.getByRole("link", { name: "Members" })).toHaveAttribute(
    "href",
    `/teams/${teamB}/members`,
  );
  respondB(
    Response.json([
      {
        ...memberships[0],
        teamId: teamB,
        person: {
          id: "person-b",
          firstName: "Sam",
          lastName: "Lee",
        },
      },
    ]),
  );
  expect(await screen.findByText("Sam Lee")).toBeVisible();
  expect(screen.queryByText("Alex Rivera")).not.toBeInTheDocument();
  view.rerender(await page());
  expect(await screen.findByRole("heading", { name: "Team A" })).toBeVisible();
  expect(screen.getAllByText("Alex Rivera")).toHaveLength(6);
  expect(screen.queryByText("Sam Lee")).not.toBeInTheDocument();
  expect(fetcher).toHaveBeenCalledTimes(4);
});

test.each(["bad-link", teamA])(
  "invalid or nonexistent Team %s never requests members",
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
