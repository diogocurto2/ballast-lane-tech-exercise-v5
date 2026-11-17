import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { App } from "../../src/App";
import { getAccessToken, setAccessToken } from "../../src/api/authToken";
import { tasksApi } from "../../src/api/tasks";
import type { TaskItem, User } from "../../src/api/types";

const signedInUser: User = {
  id: "11111111-1111-4111-8111-111111111111",
  email: "demo.user@example.test",
  createdAt: "2026-11-16T09:00:00Z",
};

const task: TaskItem = {
  id: "33333333-3333-4333-8333-333333333333",
  title: "Write the specification",
  description: "Cover traceability",
  status: "InProgress",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:30:00Z",
};

type Responder = () => Response | Promise<Response>;
type RecordedCall = { method: string; path: string; authorization: string | null };

function json(status: number, body?: unknown): Response {
  return new Response(body === undefined ? null : JSON.stringify(body), {
    status,
    headers: { "Content-Type": "application/json" },
  });
}

const unauthorized = (): Response =>
  json(401, {
    title: "Authentication required",
    detail: "The access token is missing, invalid, or expired.",
    status: 401,
  });

const taskPath = `/api/tasks/${task.id}`;

/**
 * Replaces fetch with a route table. Each route answers from its queue, and the last responder
 * repeats, so a test can describe a sequence without restating the steady state.
 */
function installFetch(routes: Record<string, Responder[]>): RecordedCall[] {
  const calls: RecordedCall[] = [];

  vi.stubGlobal("fetch", async (input: RequestInfo | URL, init?: RequestInit) => {
    const url = typeof input === "string" ? input : input.toString();
    const method = (init?.method ?? "GET").toUpperCase();
    const path = new URL(url).pathname;
    const headers = (init?.headers ?? {}) as Record<string, string>;

    calls.push({ method, path, authorization: headers.Authorization ?? null });

    const responders = routes[`${method} ${path}`];
    if (!responders || responders.length === 0) {
      throw new Error(`Unexpected request: ${method} ${path}`);
    }

    return await (responders.length > 1 ? responders.shift()!() : responders[0]());
  });

  return calls;
}

/** An answer the test settles explicitly, so a slow request can overlap a later sign-in. */
function deferredResponse() {
  let release!: (response: Response) => void;
  const promise = new Promise<Response>((resolve) => {
    release = resolve;
  });

  return { promise, release };
}

const signInRoutes: Record<string, Responder[]> = {
  "POST /api/users/login": [
    () =>
      json(200, {
        accessToken: "issued-token",
        tokenType: "Bearer",
        expiresAtUtc: "2099-01-01T00:00:00Z",
      }),
  ],
  "GET /api/users/me": [() => json(200, signedInUser)],
};

async function signIn() {
  await userEvent.type(await screen.findByLabelText(/email/i), signedInUser.email);
  await userEvent.type(screen.getByLabelText(/password/i), "DemoPassw0rd!");
  await userEvent.click(screen.getByRole("button", { name: /^log in$/i }));
}

async function expectSignedOutWithMessage() {
  expect(await screen.findByRole("heading", { name: /^log in$/i })).toBeInTheDocument();
  expect(await screen.findByText(/session has expired/i)).toBeInTheDocument();
  expect(getAccessToken()).toBeNull();
}

describe("session handling", () => {
  beforeEach(() => {
    setAccessToken(null);
    window.history.replaceState({}, "", "/");
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("clears the token on logout and sends no credential afterwards", async () => {
    const calls = installFetch({ ...signInRoutes, "GET /api/tasks": [() => json(200, [task])] });

    render(<App />);
    await signIn();

    await screen.findByRole("heading", { name: /your tasks/i });
    expect(getAccessToken()).toBe("issued-token");

    await userEvent.click(screen.getByRole("button", { name: /log out/i }));

    await screen.findByRole("heading", { name: /^log in$/i });
    expect(getAccessToken()).toBeNull();

    await tasksApi.list();

    const listCalls = calls.filter((call) => call.method === "GET" && call.path === "/api/tasks");
    expect(listCalls).toHaveLength(2);
    expect(listCalls[0].authorization).toBe("Bearer issued-token");
    expect(listCalls[1].authorization).toBeNull();
  });

  it("keeps the newer session when a delayed 401 answers the previous token", async () => {
    const stale = deferredResponse();
    installFetch({
      ...signInRoutes,
      // The manual call below consumes the deferred answer; the signed-in list request reads the task.
      "GET /api/tasks": [() => stale.promise, () => json(200, [task])],
    });

    setAccessToken("previous-token");
    const staleRequest = tasksApi.list();

    render(<App />);
    await signIn();

    await screen.findByRole("heading", { name: /your tasks/i });
    expect(getAccessToken()).toBe("issued-token");

    stale.release(unauthorized());
    await staleRequest;

    expect(await screen.findByRole("heading", { name: /your tasks/i })).toBeInTheDocument();
    expect(screen.queryByText(/session has expired/i)).not.toBeInTheDocument();
    expect(getAccessToken()).toBe("issued-token");
  });

  it("ends the session when the task list reports 401", async () => {
    const calls = installFetch({ ...signInRoutes, "GET /api/tasks": [unauthorized] });

    render(<App />);
    await signIn();

    await expectSignedOutWithMessage();

    await tasksApi.list();
    const listCalls = calls.filter((call) => call.method === "GET" && call.path === "/api/tasks");
    expect(listCalls[listCalls.length - 1]?.authorization).toBeNull();
  });

  it("ends the session when a task detail request reports 401", async () => {
    installFetch({
      ...signInRoutes,
      "GET /api/tasks": [() => json(200, [task])],
      [`GET ${taskPath}`]: [unauthorized],
    });

    render(<App />);
    await signIn();

    await userEvent.click(await screen.findByRole("button", { name: new RegExp(task.title, "i") }));

    await expectSignedOutWithMessage();
    expect(screen.queryByText(/could not be found/i)).not.toBeInTheDocument();
  });

  it("ends the session when the task edit form cannot load its task", async () => {
    installFetch({
      ...signInRoutes,
      "GET /api/tasks": [() => json(200, [task])],
      // The detail view loads first; the edit page's own load is the one that expires.
      [`GET ${taskPath}`]: [() => json(200, task), unauthorized],
    });

    render(<App />);
    await signIn();

    await userEvent.click(await screen.findByRole("button", { name: new RegExp(task.title, "i") }));
    await userEvent.click(await screen.findByRole("link", { name: /edit task/i }));

    await expectSignedOutWithMessage();
    expect(screen.queryByText(/could not be loaded for editing/i)).not.toBeInTheDocument();
  });

  it("ends the session when creating a task reports 401", async () => {
    installFetch({
      ...signInRoutes,
      "GET /api/tasks": [() => json(200, [])],
      "POST /api/tasks": [unauthorized],
    });

    render(<App />);
    await signIn();

    await userEvent.click(await screen.findByRole("button", { name: /new task/i }));
    await userEvent.type(screen.getByLabelText(/title/i), "A task that will not be saved");
    await userEvent.type(screen.getByLabelText(/due date/i), "2026-11-20");
    await userEvent.click(screen.getByRole("button", { name: /create task/i }));

    await expectSignedOutWithMessage();
  });

  it("ends the session when deleting a task reports 401", async () => {
    installFetch({
      ...signInRoutes,
      "GET /api/tasks": [() => json(200, [task])],
      [`GET ${taskPath}`]: [() => json(200, task)],
      [`DELETE ${taskPath}`]: [unauthorized],
    });

    render(<App />);
    await signIn();

    await userEvent.click(await screen.findByRole("button", { name: new RegExp(task.title, "i") }));
    await userEvent.click(await screen.findByRole("button", { name: /delete task/i }));
    await userEvent.click(await screen.findByRole("button", { name: /yes, delete/i }));

    await expectSignedOutWithMessage();
    expect(screen.queryByText(/could not be deleted/i)).not.toBeInTheDocument();
  });
});
