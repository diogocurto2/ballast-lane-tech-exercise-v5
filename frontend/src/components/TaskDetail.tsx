import { Link } from "react-router-dom";
import type { TaskItem } from "../api/types";

type TaskDetailProps = {
  task: TaskItem;
};

export function TaskDetail({ task }: TaskDetailProps) {
  return (
    <article className="task-detail">
      <h2>{task.title}</h2>

      <dl>
        <dt>Status</dt>
        <dd>{task.status}</dd>

        <dt>Due date</dt>
        <dd>{task.dueDate}</dd>

        <dt>Description</dt>
        <dd>{task.description ?? "No description."}</dd>
      </dl>

      <div className="form-actions">
        <Link to={`/tasks/${task.id}/edit`}>Edit task</Link>
        <Link to="/tasks">Back to tasks</Link>
      </div>
    </article>
  );
}

