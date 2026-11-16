import { useState } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { act, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { AuthProvider } from "../../src/auth/AuthProvider";
import { useAuth } from "../../src/auth/useAuth";
import { getAccessToken, setAccessToken } from "../../src/api/authToken";
import type { ApiFailure, ApiResult, LoginResponse, User, UsersApi } from "../../src/api/types";

const user: User = { id: "1", email: "demo.user@example.test", createdAt: "2026-11-16T09:00:00Z" };

function createUsersApi(overrides: Partial<UsersApi> = {}): UsersApi {
  return {
    register: vi.fn(),
    // The real client stores the issued token, so the double does the same to keep the contract honest.
    login: vi.fn(async () => {
      setAccessToken("token-value");
      return {
        ok: true,
        data: { accessToken: "token-value", tokenType: "Bearer", expiresAtUtc: "2099-01-01T00:00:00Z" },
      } satisfies ApiResult<LoginResponse>;
    }),
    currentUser: vi.fn().mockResolvedValue({ ok: true, data: user } satisfies ApiResult<User>),
    ...overrides,
  } as unknown as UsersApi;
}

function Probe() {
  const auth = useAuth();
  const [outcome, setOutcome] = useState("idle");

  async function signIn() {
    const result = await auth.login({ email: user.email, password: "DemoPassw0rd!" });
    setOutcome(result.ok ? "success" : `failure:${result.failure.status}`);
  }

  return (
    <div>
      <p data-testid="status">{auth.isAuthenticated ? "authenticated" : "anonymous"}</p>
      <p data-testid="email">{auth.user?.email ?? "none"}</p>
      <p data-testid="outcome">{outcome}</p>
      <button type="button" onClick={() => void signIn()}>
        sign in
      </button>
      <button type="button" onClick={auth.logout}>
        sign out
      </button>
      <button type="button" onClick={() => void auth.refreshCurrentUser()}>
        refresh
      </button>
    </div>
  );
}

describe("auth context", () => {
  beforeEach(() => {
    setAccessToken(null);
  });

  it("starts anonymous, becomes authenticated after login, and clears on logout", async () => {
    const api = createUsersApi();
    render(
      <AuthProvider api={api}>
        <Probe />
      </AuthProvider>,
    );

    expect(screen.getByTestId("status")).toHaveTextContent("anonymous");

    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));
    expect(screen.getByTestId("email")).toHaveTextContent(user.email);

    await userEvent.click(screen.getByRole("button", { name: /sign out/i }));

    expect(screen.getByTestId("status")).toHaveTextContent("anonymous");
    expect(screen.getByTestId("email")).toHaveTextContent("none");
  });

  it("clears the session when a protected call reports 401", async () => {
    const api = createUsersApi({
      // The first lookup belongs to login and succeeds; the refresh then reports an expired session.
      currentUser: vi
        .fn()
        .mockResolvedValueOnce({ ok: true, data: user } satisfies ApiResult<User>)
        .mockResolvedValueOnce({
          ok: false,
          failure: { status: 401, title: "Authentication required", detail: "Expired", errors: {} },
        } satisfies ApiResult<User>),
    });
    render(
      <AuthProvider api={api}>
        <Probe />
      </AuthProvider>,
    );

    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));

    await act(async () => {
      await userEvent.click(screen.getByRole("button", { name: /refresh/i }));
    });

    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("anonymous"));
  });

  it("clears the in-memory access token on logout", async () => {
    const api = createUsersApi();
    render(
      <AuthProvider api={api}>
        <Probe />
      </AuthProvider>,
    );

    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));
    await waitFor(() => expect(screen.getByTestId("status")).toHaveTextContent("authenticated"));
    expect(getAccessToken()).toBe("token-value");

    await userEvent.click(screen.getByRole("button", { name: /sign out/i }));

    expect(getAccessToken()).toBeNull();
    expect(screen.getByTestId("status")).toHaveTextContent("anonymous");
    expect(screen.getByTestId("email")).toHaveTextContent("none");
  });

  const unresolvedIdentity: Array<[string, ApiFailure]> = [
    [
      "a network error",
      {
        status: 0,
        title: "The server could not be reached.",
        detail: "Check that the API is running and try again.",
        errors: {},
      },
    ],
    [
      "HTTP 401",
      {
        status: 401,
        title: "Authentication required",
        detail: "The access token is missing, invalid, or expired.",
        errors: {},
      },
    ],
    ["HTTP 500", { status: 500, title: "An unexpected error occurred.", detail: null, errors: {} }],
  ];

  it.each(unresolvedIdentity)(
    "does not report a successful login when the identity lookup fails with %s",
    async (_case, failure) => {
      const api = createUsersApi({
        currentUser: vi.fn().mockResolvedValue({ ok: false, failure } satisfies ApiResult<User>),
      });
      render(
        <AuthProvider api={api}>
          <Probe />
        </AuthProvider>,
      );

      await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

      await waitFor(() =>
        expect(screen.getByTestId("outcome")).toHaveTextContent(`failure:${failure.status}`),
      );
      expect(screen.getByTestId("status")).toHaveTextContent("anonymous");
      expect(screen.getByTestId("email")).toHaveTextContent("none");
      expect(getAccessToken()).toBeNull();
    },
  );
});
