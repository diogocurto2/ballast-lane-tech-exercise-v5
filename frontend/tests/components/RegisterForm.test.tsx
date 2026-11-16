import { describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { RegisterForm } from "../../src/components/RegisterForm";
import type { ApiResult, User, UsersApi } from "../../src/api/types";

function createUsersApi(result: ApiResult<User>): UsersApi {
  return {
    register: vi.fn().mockResolvedValue(result),
    login: vi.fn(),
    currentUser: vi.fn(),
  } as unknown as UsersApi;
}

describe("RegisterForm", () => {
  it("shows an error for a malformed email address", async () => {
    const api = createUsersApi({ ok: true, data: { id: "1", email: "a@b.test", createdAt: "2026-11-16T09:00:00Z" } });
    render(<RegisterForm api={api} onRegistered={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/email/i), "not-an-email");
    await userEvent.type(screen.getByLabelText(/password/i), "ValidPassw0rd!");
    await userEvent.click(screen.getByRole("button", { name: /create account/i }));

    expect(await screen.findByText(/valid email address/i)).toBeInTheDocument();
    expect(api.register).not.toHaveBeenCalled();
  });

  it("shows a password policy error for a short password", async () => {
    const api = createUsersApi({ ok: true, data: { id: "1", email: "a@b.test", createdAt: "2026-11-16T09:00:00Z" } });
    render(<RegisterForm api={api} onRegistered={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/email/i), "new.user@example.test");
    await userEvent.type(screen.getByLabelText(/password/i), "short12");
    await userEvent.click(screen.getByRole("button", { name: /create account/i }));

    expect(await screen.findByText(/at least 8 characters/i)).toBeInTheDocument();
    expect(api.register).not.toHaveBeenCalled();
  });

  it("surfaces a server field error against the field it belongs to", async () => {
    const api = createUsersApi({
      ok: false,
      failure: {
        status: 409,
        title: "Conflict",
        detail: "An account already exists for this email address.",
        errors: {},
      },
    });
    render(<RegisterForm api={api} onRegistered={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/email/i), "taken@example.test");
    await userEvent.type(screen.getByLabelText(/password/i), "ValidPassw0rd!");
    await userEvent.click(screen.getByRole("button", { name: /create account/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(/already exists/i);
  });

  it("calls onRegistered after a successful registration", async () => {
    const user: User = { id: "1", email: "new.user@example.test", createdAt: "2026-11-16T09:00:00Z" };
    const api = createUsersApi({ ok: true, data: user });
    const onRegistered = vi.fn();
    render(<RegisterForm api={api} onRegistered={onRegistered} />);

    await userEvent.type(screen.getByLabelText(/email/i), user.email);
    await userEvent.type(screen.getByLabelText(/password/i), "ValidPassw0rd!");
    await userEvent.click(screen.getByRole("button", { name: /create account/i }));

    await waitFor(() => expect(onRegistered).toHaveBeenCalledWith(user));
  });
});
