import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { render, screen, waitFor, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import TeamPage from "@/app/teams/[teamId]/page";
import { WorkspaceShell } from "@/components/workspace-shell";
import { WorkspaceLanding } from "@/features/workspace/workspace-landing";
import type { paths } from "@/lib/api/schema";

const route = vi.hoisted(() => ({ pathname: "/" }));
vi.mock("next/navigation", () => ({ usePathname: () => route.pathname }));

const teams = [
  { id: "ad828d12-92c5-4578-985b-e83bb37cc861", name: "Touchline FC" },
  { id: "bd828d12-92c5-4578-985b-e83bb37cc861", name: "Touchline FC" },
] satisfies paths["/api/teams"]["get"]["responses"][200]["content"]["application/json"];

afterEach(() => {
  vi.unstubAllGlobals();
  route.pathname = "/";
});

function renderJourney(content: React.ReactNode) {
  const client = new QueryClient({
    defaultOptions: { queries: { retry: false, retryDelay: 0, gcTime: 0 } },
  });
  return render(
    <QueryClientProvider client={client}>
      <WorkspaceShell>{content}</WorkspaceShell>
    </QueryClientProvider>,
  );
}

test("workspace announces loading then offers distinct keyboard-accessible Team routes", async () => {
  let respond!: (response: Response) => void;
  vi.stubGlobal(
    "fetch",
    vi.fn(
      () =>
        new Promise<Response>((resolve) => {
          respond = resolve;
        }),
    ),
  );
  renderJourney(<WorkspaceLanding />);
  expect(screen.getByText("Your Teams")).toBeVisible();
  expect(screen.getByRole("status")).toHaveTextContent("Loading Teams");
  respond(Response.json(teams));
  const links = await screen.findAllByRole("link", { name: /Touchline FC/ });
  expect(links).toHaveLength(2);
  expect(links[0]).toHaveAttribute("href", `/teams/${teams[0].id}`);
  expect(links[1]).toHaveAttribute("href", `/teams/${teams[1].id}`);
  const user = userEvent.setup();
  for (let i = 0; i < 4; i++) await user.tab();
  expect(links[0]).toHaveFocus();
});

test("empty collection gives an honest next step without a fake create action", async () => {
  vi.stubGlobal("fetch", vi.fn().mockResolvedValue(Response.json([])));
  renderJourney(<WorkspaceLanding />);
  await waitFor(() => expect(screen.getByRole("status")).toHaveTextContent("No Teams yet"));
  expect(screen.getByRole("status")).toHaveTextContent("local Development API");
  expect(screen.queryByRole("link", { name: /create/i })).not.toBeInTheDocument();
});

test("API failure stays distinct from empty and can be retried with the keyboard", async () => {
  const fetcher = vi
    .fn()
    .mockResolvedValueOnce(new Response(null, { status: 500 }))
    .mockImplementation(() => Promise.resolve(Response.json(teams)));
  vi.stubGlobal("fetch", fetcher);
  renderJourney(<WorkspaceLanding />);
  expect(await screen.findByRole("alert")).toHaveTextContent("Teams could not be loaded");
  expect(screen.queryByText(/No Teams yet/)).not.toBeInTheDocument();
  const retry = screen.getByRole("button", { name: "Try again" });
  retry.focus();
  await userEvent.setup().keyboard("{Enter}");
  expect(await screen.findAllByRole("link", { name: /Touchline FC/ })).toHaveLength(2);
  expect(fetcher).toHaveBeenCalledTimes(2);
});

test("following a Team route and reloading derives real identity from a fresh remote cache", async () => {
  const fetcher = vi.fn<typeof fetch>().mockImplementation((input) => {
    const path = new URL((input as Request).url).pathname;
    if (path === "/api/teams") return Promise.resolve(Response.json(teams));
    expect(path).toBe(`/api/teams/${teams[1].id}`);
    return Promise.resolve(Response.json(teams[1]));
  });
  vi.stubGlobal("fetch", fetcher);
  const workspace = renderJourney(<WorkspaceLanding />);
  const link = (await screen.findAllByRole("link", { name: /Touchline FC/ }))[1];
  const href = link.getAttribute("href")!;
  workspace.unmount();
  route.pathname = href;
  // Resolve the actual App Router page with URL params, then mount with a fresh provider (reload).
  for (let reload = 0; reload < 2; reload++) {
    const view = renderJourney(
      await TeamPage({ params: Promise.resolve({ teamId: href.split("/")[2] }) }),
    );
    expect(await screen.findByRole("heading", { name: "Touchline FC" })).toBeVisible();
    expect(screen.getByText(teams[1].id)).toBeVisible();
    expect(screen.getByRole("link", { name: "← All Teams" })).toHaveAttribute("href", "/");
    const nav = screen.getByRole("navigation", { name: "Workspace" });
    expect(within(nav).getByRole("link", { name: "Workspace" })).not.toHaveAttribute(
      "aria-current",
    );
    view.unmount();
  }
  expect(fetcher).toHaveBeenCalledTimes(3);
  expect(
    fetcher.mock.calls.slice(1).map(([request]) => new URL((request as Request).url).pathname),
  ).toEqual([`/api/teams/${teams[1].id}`, `/api/teams/${teams[1].id}`]);
});

test("unknown Team context offers a return path instead of a fabricated Team", async () => {
  vi.stubGlobal("fetch", vi.fn().mockResolvedValue(new Response(null, { status: 404 })));
  renderJourney(await TeamPage({ params: Promise.resolve({ teamId: teams[0].id }) }));
  await waitFor(() =>
    expect(screen.getByRole("status")).toHaveTextContent("This Team was not found"),
  );
  expect(screen.queryByRole("alert")).not.toBeInTheDocument();
  const back = screen.getByRole("link", { name: "← All Teams" });
  const user = userEvent.setup();
  for (let i = 0; i < 6 && document.activeElement !== back; i++) await user.tab();
  expect(back).toHaveFocus();
  expect(screen.getByRole("link", { name: "← All Teams" })).toHaveAttribute("href", "/");
});

test.each([
  { label: "server failure", response: () => Promise.resolve(new Response(null, { status: 500 })) },
  { label: "transport failure", response: () => Promise.reject(new TypeError("Failed to fetch")) },
])(
  "Team detail distinguishes $label from not found and supports keyboard retry",
  async ({ response }) => {
    const fetcher = vi.fn<typeof fetch>().mockImplementation(response);
    vi.stubGlobal("fetch", fetcher);
    renderJourney(await TeamPage({ params: Promise.resolve({ teamId: teams[0].id }) }));
    expect(screen.getByRole("status")).toHaveTextContent("Loading Team");
    expect(await screen.findByRole("alert")).toHaveTextContent("Team could not be loaded");
    expect(screen.queryByText(/This Team was not found/)).not.toBeInTheDocument();
    expect(fetcher).toHaveBeenCalledTimes(4);
    fetcher.mockImplementation(() => Promise.resolve(Response.json(teams[0])));
    screen.getByRole("button", { name: "Try again" }).focus();
    await userEvent.setup().keyboard("{Enter}");
    expect(await screen.findByRole("heading", { name: teams[0].name })).toBeVisible();
    expect(fetcher).toHaveBeenCalledTimes(5);
    for (const [request] of fetcher.mock.calls) {
      expect(new URL((request as Request).url).pathname).toBe(`/api/teams/${teams[0].id}`);
    }
  },
);

test("malformed route fails gracefully without an API call", async () => {
  const fetcher = vi.fn();
  vi.stubGlobal("fetch", fetcher);
  renderJourney(await TeamPage({ params: Promise.resolve({ teamId: "bad-link" }) }));
  expect(screen.getByRole("heading", { name: "Invalid Team link" })).toBeVisible();
  expect(screen.getByRole("link", { name: "← All Teams" })).toHaveAttribute("href", "/");
  expect(fetcher).not.toHaveBeenCalled();
});
