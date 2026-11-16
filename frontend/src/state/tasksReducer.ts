import type { TaskItem, TaskStatus } from "../api/types";

export type TaskAction =
  | { type: "loading" }
  | { type: "loaded"; tasks: TaskItem[] }
  | { type: "failed"; message: string }
  | { type: "added"; task: TaskItem }
  | { type: "updated"; task: TaskItem }
  | { type: "removed"; id: string };

export type TaskStore =
  | { status: "loading" }
  | { status: "error"; message: string }
  | { status: "success"; tasks: TaskItem[] };

export const initialTaskStore: TaskStore = { status: "loading" };

const statusOrder: Record<TaskStatus, number> = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
};

function sortTasks(tasks: TaskItem[]): TaskItem[] {
  return [...tasks].sort((left, right) => {
    const byDueDate = left.dueDate.localeCompare(right.dueDate);
    if (byDueDate !== 0) {
      return byDueDate;
    }

    const byCreatedAt = left.createdAt.localeCompare(right.createdAt);
    return byCreatedAt !== 0 ? byCreatedAt : left.id.localeCompare(right.id);
  });
}

/**
 * Reduces task list state. Every mutation updates the list locally so the view reflects the new state
 * without a reload, while the server remains the authority.
 */
export function tasksReducer(state: TaskStore, action: TaskAction): TaskStore {
  switch (action.type) {
    case "loading":
      return { status: "loading" };

    case "loaded":
      return { status: "success", tasks: sortTasks(action.tasks) };

    case "failed":
      return { status: "error", message: action.message };

    case "added":
      return state.status === "success"
        ? { status: "success", tasks: sortTasks([...state.tasks, action.task]) }
        : state;

    case "updated":
      return state.status === "success"
        ? {
            status: "success",
            tasks: sortTasks(state.tasks.map((task) => (task.id === action.task.id ? action.task : task))),
          }
        : state;

    case "removed":
      return state.status === "success"
        ? { status: "success", tasks: state.tasks.filter((task) => task.id !== action.id) }
        : state;

    default:
      return state;
  }
}

export { statusOrder };
