import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, expect, test, vi } from "vitest";
import CreateTeamPage from "@/app/teams/new/page";
import { TeamCollection } from "./team-collection";
import { teamsQueryKey } from "./use-teams";

const push = vi.hoisted(() => vi.fn());
vi.mock("next/navigation", () => ({ useRouter: () => ({ push }) }));

const created = { id: "ad828d12-92c5-4578-985b-e83bb37cc861", name: "Touchline FC" };

afterEach(() => {
  vi.unstubAllGlobals();
  vi.clearAllMocks();
});

function renderCreate() {
  const client = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: Infinity },
      mutations: { gcTime: 0 },
    },
  });
  client.setQueryData(teamsQueryKey, []);
  const view = render(
    <QueryClientProvider client={client}>
      <CreateTeamPage />
    </QueryClientProvider>,
  );
  return { client, ...view };
}

test("keyboard submit trims the typed POST, invalidates the list and opens the returned Team", async () => {
  const fetcher = vi.fn<typeof fetch>().mockResolvedValue(Response.json(created, { status: 201 }));
  vi.stubGlobal("fetch", fetcher);
  const { client, rerender } = renderCreate();
  const user = userEvent.setup();
  await user.tab();
  expect(screen.getByRole("textbox", { name: "Team name" })).toHaveFocus();
  await user.keyboard("  Touchline FC  {Enter}");
  await waitFor(() => expect(push).toHaveBeenCalledWith(`/teams/${created.id}`));
  expect(fetcher).toHaveBeenCalledTimes(1);
  const request = fetcher.mock.calls[0][0] as Request;
  expect(request.method).toBe("POST");
  expect(request.url).toBe("http://127.0.0.1:5050/api/teams");
  expect(request.headers.get("Content-Type")).toBe("application/json");
  expect(await request.json()).toEqual({ name: "Touchline FC" });
  expect(client.getQueryState(teamsQueryKey)?.isInvalidated).toBe(true);
  fetcher.mockResolvedValue(Response.json([created]));
  rerender(
    <QueryClientProvider client={client}>
      <TeamCollection />
    </QueryClientProvider>,
  );
  expect(await screen.findByRole("link", { name: /Touchline FC/ })).toHaveAttribute(
    "href",
    `/teams/${created.id}`,
  );
  expect(fetcher).toHaveBeenCalledTimes(2);
});

test.each(["", "   "])(
  "blank name %j gives associated feedback without a request",
  async (name) => {
    const fetcher = vi.fn();
    vi.stubGlobal("fetch", fetcher);
    renderCreate();
    const user = userEvent.setup();
    const input = screen.getByRole("textbox", { name: "Team name" });
    if (name) await user.type(input, name);
    await user.click(screen.getByRole("button", { name: "Create Team" }));
    expect(screen.getByRole("alert")).toHaveTextContent("Enter a Team name");
    expect(input).toHaveAttribute("aria-invalid", "true");
    expect(input).toHaveAccessibleDescription(/Spaces alone cannot be used/);
    expect(input).toHaveFocus();
    expect(fetcher).not.toHaveBeenCalled();
    expect(push).not.toHaveBeenCalled();
  },
);

test("pending state blocks rapid clicks and submit events until navigation", async () => {
  let respond!: (response: Response) => void;
  const fetcher = vi.fn(() => new Promise<Response>((resolve) => (respond = resolve)));
  vi.stubGlobal("fetch", fetcher);
  renderCreate();
  const user = userEvent.setup();
  const input = screen.getByRole("textbox", { name: "Team name" });
  await user.type(input, "Touchline FC");
  const submit = screen.getByRole("button", { name: "Create Team" });
  const form = submit.closest("form")!;
  fireEvent.submit(form);
  fireEvent.submit(form);
  await user.dblClick(submit);
  expect(fetcher).toHaveBeenCalledTimes(1);
  expect(submit).toBeDisabled();
  expect(input).toBeDisabled();
  expect(screen.getByRole("button", { name: "Cancel" })).toBeDisabled();
  expect(screen.getByRole("status")).toHaveTextContent("Creating Team");
  respond(Response.json(created, { status: 201 }));
  await waitFor(() => expect(push).toHaveBeenCalledWith(`/teams/${created.id}`));
  fireEvent.submit(form);
  expect(fetcher).toHaveBeenCalledTimes(1);
});

test.each([
  {
    label: "API 400",
    response: () => Promise.resolve(new Response(null, { status: 400 })),
    message: /did not accept this Team name/,
  },
  {
    label: "API 500",
    response: () => Promise.resolve(new Response(null, { status: 500 })),
    message: /local Development API/,
  },
  {
    label: "transport failure",
    response: () => Promise.reject(new TypeError("Failed to fetch")),
    message: /local Development API/,
  },
  {
    label: "missing body",
    response: () => Promise.resolve(new Response(null, { status: 201 })),
    message: /local Development API/,
  },
])(
  "$label retains input and permits an explicit retry without navigation or invalidation",
  async ({ response, message }) => {
    const fetcher = vi.fn<typeof fetch>().mockImplementation(response);
    vi.stubGlobal("fetch", fetcher);
    const { client } = renderCreate();
    const user = userEvent.setup();
    const input = screen.getByRole("textbox", { name: "Team name" });
    await user.type(input, "Touchline FC");
    await user.click(screen.getByRole("button", { name: "Create Team" }));
    expect(await screen.findByRole("alert")).toHaveTextContent(message);
    expect(input).toHaveValue("Touchline FC");
    expect(fetcher).toHaveBeenCalledTimes(1);
    expect(push).not.toHaveBeenCalled();
    expect(client.getQueryState(teamsQueryKey)?.isInvalidated).toBe(false);
    fetcher.mockImplementation(() => Promise.resolve(Response.json(created, { status: 201 })));
    await user.click(screen.getByRole("button", { name: "Create Team" }));
    await waitFor(() => expect(push).toHaveBeenCalledWith(`/teams/${created.id}`));
    expect(fetcher).toHaveBeenCalledTimes(2);
  },
);

test("Tab reaches submit and Cancel; keyboard cancel returns to the workspace without POST", async () => {
  const fetcher = vi.fn();
  vi.stubGlobal("fetch", fetcher);
  renderCreate();
  const user = userEvent.setup();
  await user.tab();
  expect(screen.getByRole("textbox", { name: "Team name" })).toHaveFocus();
  await user.tab();
  expect(screen.getByRole("button", { name: "Create Team" })).toHaveFocus();
  await user.tab();
  expect(screen.getByRole("button", { name: "Cancel" })).toHaveFocus();
  await user.keyboard("{Enter}");
  expect(push).toHaveBeenCalledWith("/");
  expect(fetcher).not.toHaveBeenCalled();
});
