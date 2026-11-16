import { describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { TaskForm } from "../../src/components/TaskForm";
import type { ApiResult, TaskItem, TasksApi } from "../../src/api/types";

const created: TaskItem = {
  id: "44444444-4444-4444-8444-444444444444",
  title: "Write the specification",
  description: null,
  status: "Pending",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:00:00Z",
};

function createTasksApi(result: ApiResult<TaskItem>): TasksApi {
  return {
    list: vi.fn(),
    getById: vi.fn(),
    create: vi.fn().mockResolvedValue(result),
    update: vi.fn(),
    remove: vi.fn(),
  } as unknown as TasksApi;
}

describe("TaskForm in create mode", () => {
  it("shows a required-title error and sends no request", async () => {
    const api = createTasksApi({ ok: true, data: created });
    render(<TaskForm api={api} mode={{ kind: "create" }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/due date/i), "2026-11-20");
    await userEvent.click(screen.getByRole("button", { name: /create task/i }));

    expect(await screen.findByText(/title is required/i)).toBeInTheDocument();
    expect(api.create).not.toHaveBeenCalled();
  });

  it("shows a missing-date error and sends no request", async () => {
    const api = createTasksApi({ ok: true, data: created });
    render(<TaskForm api={api} mode={{ kind: "create" }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/title/i), "Write the specification");
    await userEvent.click(screen.getByRole("button", { name: /create task/i }));

    // A native date input cannot hold a non-calendar value, so the reachable invalid case is an
    // empty one; the format rule is still enforced in code for any non-native client.
    expect(await screen.findByText(/due date is required/i)).toBeInTheDocument();
    expect(api.create).not.toHaveBeenCalled();
  });

  it("disables submission while the request is in flight", async () => {
    let resolveCreate: ((value: ApiResult<TaskItem>) => void) | undefined;
    const api = createTasksApi({ ok: true, data: created });
    vi.mocked(api.create).mockImplementation(
      () => new Promise<ApiResult<TaskItem>>((resolve) => {
        resolveCreate = resolve;
      }),
    );

    render(<TaskForm api={api} mode={{ kind: "create" }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/title/i), "Write the specification");
    await userEvent.type(screen.getByLabelText(/due date/i), "2026-11-20");
    await userEvent.click(screen.getByRole("button", { name: /create task/i }));

    expect(screen.getByRole("button", { name: /saving/i })).toBeDisabled();

    resolveCreate?.({ ok: true, data: created });
    await waitFor(() => expect(screen.getByRole("button", { name: /create task/i })).toBeEnabled());
  });

  it("reports the created task so the list can show it without a reload", async () => {
    const api = createTasksApi({ ok: true, data: created });
    const onSaved = vi.fn();
    render(<TaskForm api={api} mode={{ kind: "create" }} onSaved={onSaved} onCancel={vi.fn()} />);

    await userEvent.type(screen.getByLabelText(/title/i), "Write the specification");
    await userEvent.type(screen.getByLabelText(/due date/i), "2026-11-20");
    await userEvent.click(screen.getByRole("button", { name: /create task/i }));

    await waitFor(() => expect(onSaved).toHaveBeenCalledWith(created));
  });
});
