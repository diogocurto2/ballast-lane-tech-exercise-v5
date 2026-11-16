import { EmptyState, ErrorMessage, Spinner } from "./Feedback";
import type { TaskItem, TaskListState } from "../api/types";

type TaskListProps = {
  state: TaskListState;
  onSelect: (task: TaskItem) => void;
  onCreate: () => void;
  onRetry?: () => void;
};

/**
 * Renders exactly one of the four view states, so a failed load can never look like an empty or
 * successful one.
 */
export function TaskList({ state, onSelect, onCreate, onRetry }: TaskListProps) {
  if (state.status === "loading") {
    return <Spinner label="Loading tasks…" />;
  }

  if (state.status === "error") {
    return <ErrorMessage message={state.message} onRetry={onRetry} />;
  }

  if (state.tasks.length === 0) {
    return (
      <EmptyState
        message="No tasks yet."
        action={
          <button type="button" onClick={onCreate}>
            Create your first task
          </button>
        }
      />
    );
  }

  return (
    <ul className="task-list">
      {state.tasks.map((task) => (
        <li key={task.id}>
          <button type="button" className="task-row" onClick={() => onSelect(task)}>
            <span className="task-title">{task.title}</span>
            <span className="task-status" data-status={task.status}>
              {task.status}
            </span>
            <span className="task-due">{task.dueDate}</span>
          </button>
        </li>
      ))}
    </ul>
  );
}
