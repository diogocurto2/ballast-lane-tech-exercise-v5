import { useNavigate } from "react-router-dom";
import { TaskForm } from "../components/TaskForm";
import { tasksApi } from "../api/tasks";

/**
 * Create page. The saved task is handed to the list through router state so the new row appears
 * without refetching the collection.
 */
export function TaskFormPage() {
  const navigate = useNavigate();

  return (
    <main className="page">
      <TaskForm
        api={tasksApi}
        mode={{ kind: "create" }}
        onSaved={(task) => navigate("/tasks", { state: { createdTask: task } })}
        onCancel={() => navigate("/tasks")}
      />
    </main>
  );
}
