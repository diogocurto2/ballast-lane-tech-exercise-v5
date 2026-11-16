import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { LoginForm } from "../../src/components/LoginForm";
import type { ApiResult, LoginResponse, UsersApi } from "../../src/api/types";

function createUsersApi(result: ApiResult<LoginResponse>): UsersApi {
  return {
    register: vi.fn(),
    login: vi.fn().mockResolvedValue(result),
    currentUser: vi.fn(),
  } as unknown as UsersApi;
}

describe("LoginForm", () => {
  it("shows one generic message for bad credentials", async () => {
    const api = createUsersApi({
      ok: false,
      failure: {
        status: 401,
        title: "Authentication required",
        detail: "Invalid email or password.",
        errors: {},
      },
    });
    render(<LoginForm api={api} onAuthenticated={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/email/i), "demo.user@example.test");
    await userEvent.type(screen.getByLabelText(/password/i), "WrongPassw0rd!");
    await userEvent.click(screen.getByRole("button", { name: /log in/i }));

    const alerts = await screen.findAllByRole("alert");
    expect(alerts).toHaveLength(1);
    expect(alerts[0]).toHaveTextContent(/invalid email or password/i);
    expect(screen.queryByText(/not registered/i)).not.toBeInTheDocument();
  });

  it("reports the issued token to the caller on success", async () => {
    const response: LoginResponse = {
      accessToken: "token-value",
      tokenType: "Bearer",
      expiresAtUtc: "2026-11-16T10:00:00Z",
    };
    const api = createUsersApi({ ok: true, data: response });
    const onAuthenticated = vi.fn();
    render(<LoginForm api={api} onAuthenticated={onAuthenticated} />);

    await userEvent.type(screen.getByLabelText(/email/i), "demo.user@example.test");
    await userEvent.type(screen.getByLabelText(/password/i), "DemoPassw0rd!");
    await userEvent.click(screen.getByRole("button", { name: /log in/i }));

    expect(onAuthenticated).toHaveBeenCalledWith(response);
  });

  it("does not submit an empty form", async () => {
    const api = createUsersApi({
      ok: false,
      failure: { status: 400, title: "Validation failed", detail: null, errors: {} },
    });
    render(<LoginForm api={api} onAuthenticated={vi.fn()} />);

    await userEvent.click(screen.getByRole("button", { name: /log in/i }));

    expect(await screen.findByText(/enter your email address/i)).toBeInTheDocument();
    expect(api.login).not.toHaveBeenCalled();
  });
});
