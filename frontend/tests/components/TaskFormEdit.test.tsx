import { describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { TaskForm } from "../../src/components/TaskForm";
import type { ApiResult, TaskItem, TasksApi } from "../../src/api/types";

const existing: TaskItem = {
  id: "55555555-5555-4555-8555-555555555555",
  title: "Write the specification",
  description: "Cover traceability",
  status: "InProgress",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:30:00Z",
};

function createTasksApi(result: ApiResult<TaskItem>): TasksApi {
  return {
    list: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn().mockResolvedValue(result),
    remove: vi.fn(),
  } as unknown as TasksApi;
}

describe("TaskForm in edit mode", () => {
  it("pre-populates the current values", () => {
    const api = createTasksApi({ ok: true, data: existing });
    render(<TaskForm api={api} mode={{ kind: "edit", task: existing }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    expect(screen.getByLabelText(/title/i)).toHaveValue(existing.title);
    expect(screen.getByLabelText(/description/i)).toHaveValue(existing.description);
    expect(screen.getByLabelText(/due date/i)).toHaveValue(existing.dueDate);
    expect(screen.getByLabelText(/status/i)).toHaveValue(existing.status);
  });

  it("shows a server validation error against the field it belongs to", async () => {
    const api = createTasksApi({
      ok: false,
      failure: {
        status: 400,
        title: "Validation failed",
        detail: "One or more validation errors occurred.",
        errors: { title: ["Title is required."] },
      },
    });
    render(<TaskForm api={api} mode={{ kind: "edit", task: existing }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    await userEvent.clear(screen.getByLabelText(/title/i));
    await userEvent.type(screen.getByLabelText(/title/i), "x");
    await userEvent.click(screen.getByRole("button", { name: /save changes/i }));

    expect(await screen.findByText(/title is required/i)).toBeInTheDocument();
  });

  it("keeps the previous values visible when the update fails", async () => {
    const api = createTasksApi({
      ok: false,
      failure: {
        status: 409,
        title: "Conflict",
        detail: "The task was changed elsewhere.",
        errors: {},
      },
    });
    render(<TaskForm api={api} mode={{ kind: "edit", task: existing }} onSaved={vi.fn()} onCancel={vi.fn()} />);

    await userEvent.click(screen.getByRole("button", { name: /save changes/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(/changed elsewhere/i);
    expect(screen.getByLabelText(/title/i)).toHaveValue(existing.title);
    expect(screen.getByLabelText(/description/i)).toHaveValue(existing.description);
  });

  it("sends the update and reports the saved task so the list can show it without a reload", async () => {
    const updated = { ...existing, title: "Revised title", status: "Completed" as const };
    const api = createTasksApi({ ok: true, data: updated });
    const onSaved = vi.fn();
    render(<TaskForm api={api} mode={{ kind: "edit", task: existing }} onSaved={onSaved} onCancel={vi.fn()} />);

    await userEvent.clear(screen.getByLabelText(/title/i));
    await userEvent.type(screen.getByLabelText(/title/i), "Revised title");
    await userEvent.selectOptions(screen.getByLabelText(/status/i), "Completed");
    await userEvent.click(screen.getByRole("button", { name: /save changes/i }));

    await waitFor(() => expect(api.update).toHaveBeenCalledWith(existing.id, expect.objectContaining({
      title: "Revised title",
      status: "Completed",
    })));
    expect(onSaved).toHaveBeenCalledWith(updated);
  });
});
