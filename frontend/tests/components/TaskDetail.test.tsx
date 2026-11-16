import { describe, expect, it } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { TaskDetail } from "../../src/components/TaskDetail";
import type { TaskItem } from "../../src/api/types";

const task: TaskItem = {
  id: "33333333-3333-4333-8333-333333333333",
  title: "Write the specification",
  description: "Cover traceability",
  status: "Completed",
  dueDate: "2026-11-20",
  createdAt: "2026-11-16T09:00:00Z",
  updatedAt: "2026-11-16T09:30:00Z",
};

describe("TaskDetail", () => {
  it("renders a task's fields", () => {
    render(
      <MemoryRouter>
        <TaskDetail task={task} />
      </MemoryRouter>,
    );

    expect(screen.getByRole("heading", { name: "Write the specification" })).toBeInTheDocument();
    expect(screen.getByText("Cover traceability")).toBeInTheDocument();
    expect(screen.getByText("Completed")).toBeInTheDocument();
    expect(screen.getByText("2026-11-20")).toBeInTheDocument();
  });

  it("shows an explicit placeholder when the description is absent", () => {
    render(
      <MemoryRouter>
        <TaskDetail task={{ ...task, description: null }} />
      </MemoryRouter>,
    );

    expect(screen.getByText(/no description/i)).toBeInTheDocument();
  });
});
