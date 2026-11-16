import { describe, expect, it, vi } from "vitest";
import { act, render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { AuthProvider } from "../../src/auth/AuthProvider";
import { useAuth } from "../../src/auth/useAuth";
import type { ApiResult, LoginResponse, User, UsersApi } from "../../src/api/types";

const user: User = { id: "1", email: "demo.user@example.test", createdAt: "2026-11-16T09:00:00Z" };

function createUsersApi(overrides: Partial<UsersApi> = {}): UsersApi {
  return {
    register: vi.fn(),
    login: vi.fn().mockResolvedValue({
      ok: true,
      data: { accessToken: "token-value", tokenType: "Bearer", expiresAtUtc: "2099-01-01T00:00:00Z" },
    } satisfies ApiResult<LoginResponse>),
    currentUser: vi.fn().mockResolvedValue({ ok: true, data: user } satisfies ApiResult<User>),
    ...overrides,
  } as unknown as UsersApi;
}

function Probe() {
  const auth = useAuth();

  return (
    <div>
      <p data-testid="status">{auth.isAuthenticated ? "authenticated" : "anonymous"}</p>
      <p data-testid="email">{auth.user?.email ?? "none"}</p>
      <button type="button" onClick={() => void auth.login({ email: user.email, password: "DemoPassw0rd!" })}>
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
});
