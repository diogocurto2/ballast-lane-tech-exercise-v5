import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { TaskForm } from "../components/TaskForm";
import { ErrorMessage, Spinner } from "../components/Feedback";
import { isSessionExpired } from "../api/client";
import { tasksApi } from "../api/tasks";
import type { TaskItem } from "../api/types";

/**
 * Edit page. It loads the task first so the form is pre-populated with current values, and hands the
 * saved task to the detail route through router state so the view updates without a refetch.
 */
export function TaskEditPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [task, setTask] = useState<TaskItem | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      return;
    }

    let cancelled = false;

    void tasksApi.getById(id).then((result) => {
      if (cancelled) {
        return;
      }

      if (result.ok) {
        setTask(result.data);
        return;
      }

      // A rejected credential is handled centrally; a stale form must not be reported as a load error.
      if (isSessionExpired(result.failure)) {
        return;
      }

      setError("This task could not be loaded for editing.");
    });

    return () => {
      cancelled = true;
    };
  }, [id]);

  if (error) {
    return (
      <main className="page">
        <ErrorMessage message={error} onRetry={() => navigate("/tasks")} />
      </main>
    );
  }

  if (!task) {
    return (
      <main className="page">
        <Spinner label="Loading task…" />
      </main>
    );
  }

  return (
    <main className="page">
      <TaskForm
        api={tasksApi}
        mode={{ kind: "edit", task }}
        onSaved={(saved) => navigate(`/tasks/${saved.id}`, { state: { updatedTask: saved } })}
        onCancel={() => navigate(`/tasks/${task.id}`)}
      />
    </main>
  );
}