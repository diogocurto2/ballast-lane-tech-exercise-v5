import { describe, expect, it, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { TaskList } from "../../src/components/TaskList";
import type { TaskItem } from "../../src/api/types";

const task: TaskItem = {
  id: "33333333-3333-4333-8333-333333333333",
  title: "Write the specification",
  description: "Cover traceability",
  status: "InProgress",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:30:00Z",
};

describe("TaskList states", () => {
  it("shows a visible loading state", () => {
    render(<TaskList state={{ status: "loading" }} onSelect={vi.fn()} onCreate={vi.fn()} />);

    expect(screen.getByRole("status")).toHaveTextContent(/loading/i);
  });

  it("shows an empty state with a next action", async () => {
    const onCreate = vi.fn();
    render(<TaskList state={{ status: "success", tasks: [] }} onSelect={vi.fn()} onCreate={onCreate} />);

    expect(screen.getByText(/no tasks yet/i)).toBeInTheDocument();
    await userEvent.click(screen.getByRole("button", { name: /create your first task/i }));
    expect(onCreate).toHaveBeenCalled();
  });

  it("lists the tasks on success", () => {
    render(<TaskList state={{ status: "success", tasks: [task] }} onSelect={vi.fn()} onCreate={vi.fn()} />);

    expect(screen.getByText("Write the specification")).toBeInTheDocument();
    expect(screen.getByText("InProgress")).toBeInTheDocument();
    expect(screen.getByText("2026-11-20")).toBeInTheDocument();
  });

  it("shows a human-readable error with a retry action", async () => {
    const onRetry = vi.fn();
    render(
      <TaskList
        state={{ status: "error", message: "Could not load tasks." }}
        onSelect={vi.fn()}
        onCreate={vi.fn()}
        onRetry={onRetry}
      />,
    );

    expect(screen.getByRole("alert")).toHaveTextContent(/could not load tasks/i);
    expect(screen.queryByText(/at BallastLane/)).not.toBeInTheDocument();
    await userEvent.click(screen.getByRole("button", { name: /retry/i }));
    expect(onRetry).toHaveBeenCalled();
  });
});
