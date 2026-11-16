import { describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { TaskDelete } from "../../src/components/TaskDelete";
import type { ApiResult, TaskItem, TasksApi } from "../../src/api/types";

const task: TaskItem = {
  id: "66666666-6666-4666-8666-666666666666",
  title: "Delete me",
  description: null,
  status: "Pending",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:00:00Z",
};

function createTasksApi(result: ApiResult<void>): TasksApi {
  return {
    list: vi.fn(),
    getById: vi.fn(),
    create: vi.fn(),
    update: vi.fn(),
    remove: vi.fn().mockResolvedValue(result),
  } as unknown as TasksApi;
}

describe("TaskDelete", () => {
  it("requires a confirmation before any delete request is sent", async () => {
    const api = createTasksApi({ ok: true, data: undefined });
    render(<TaskDelete api={api} task={task} onDeleted={vi.fn()} />);

    await userEvent.click(screen.getByRole("button", { name: /delete task/i }));

    expect(api.remove).not.toHaveBeenCalled();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
  });

  it("sends no request when the confirmation is cancelled", async () => {
    const api = createTasksApi({ ok: true, data: undefined });
    render(<TaskDelete api={api} task={task} onDeleted={vi.fn()} />);

    await userEvent.click(screen.getByRole("button", { name: /delete task/i }));
    await userEvent.click(screen.getByRole("button", { name: /cancel/i }));

    expect(api.remove).not.toHaveBeenCalled();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
  });

  it("removes the task from the list after a confirmed delete", async () => {
    const api = createTasksApi({ ok: true, data: undefined });
    const onDeleted = vi.fn();
    render(<TaskDelete api={api} task={task} onDeleted={onDeleted} />);

    await userEvent.click(screen.getByRole("button", { name: /delete task/i }));
    await userEvent.click(screen.getByRole("button", { name: /yes, delete/i }));

    await waitFor(() => expect(api.remove).toHaveBeenCalledWith(task.id));
    expect(onDeleted).toHaveBeenCalledWith(task.id);
  });

  it("keeps the task and shows an error when the delete fails", async () => {
    const api = createTasksApi({
      ok: false,
      failure: { status: 404, title: "Resource not found", detail: "Task was not found.", errors: {} },
    });
    const onDeleted = vi.fn();
    render(<TaskDelete api={api} task={task} onDeleted={onDeleted} />);

    await userEvent.click(screen.getByRole("button", { name: /delete task/i }));
    await userEvent.click(screen.getByRole("button", { name: /yes, delete/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(/could not be deleted/i);
    expect(onDeleted).not.toHaveBeenCalled();
  });
});
