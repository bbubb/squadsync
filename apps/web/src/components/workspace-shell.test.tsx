import { render, screen, within } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { expect, test, vi } from "vitest";

vi.mock("next/navigation", () => ({ usePathname: () => "/" }));
import { WorkspaceShell } from "./workspace-shell";

test("keyboard users can reach a skip link targeting the main workspace", async () => {
  const user = userEvent.setup();
  render(
    <WorkspaceShell>
      <h1>Your workspace</h1>
    </WorkspaceShell>,
  );

  await user.tab();
  const skipLink = screen.getByRole("link", { name: "Skip to content" });
  expect(skipLink).toHaveFocus();

  const main = screen.getByRole("main");
  expect(skipLink).toHaveAttribute("href", `#${main.id}`);
  expect(main).toHaveAttribute("tabindex", "-1");
  expect(within(main).getByRole("heading", { name: "Your workspace" })).toBeVisible();

  await user.tab();
  expect(screen.getByRole("link", { name: "SquadSync home" })).toHaveFocus();
  const navigation = screen.getByRole("navigation", { name: "Workspace" });
  expect(within(navigation).getByRole("link", { name: "Workspace" })).toHaveAttribute(
    "aria-current",
    "page",
  );
});
