import { render } from "@testing-library/react";
import { useQueryClient, type QueryClient } from "@tanstack/react-query";
import { expect, test } from "vitest";
import { QueryProvider } from "./query-provider";

test("provides a stable cache across renders and isolates separate provider mounts", () => {
  const clients: QueryClient[] = [];
  function Consumer() {
    clients.push(useQueryClient());
    return null;
  }
  const view = render(
    <QueryProvider>
      <Consumer />
    </QueryProvider>,
  );
  clients[0].setQueryData(["provider-test"], "retained");
  view.rerender(
    <QueryProvider>
      <Consumer />
    </QueryProvider>,
  );
  expect(clients[1]).toBe(clients[0]);
  expect(clients[1].getQueryData(["provider-test"])).toBe("retained");
  view.unmount();
  render(
    <QueryProvider>
      <Consumer />
    </QueryProvider>,
  );
  expect(clients[2]).not.toBe(clients[0]);
  expect(clients[2].getQueryData(["provider-test"])).toBeUndefined();
});
