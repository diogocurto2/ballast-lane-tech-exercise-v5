import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { TaskDetail } from "../components/TaskDetail";
import { TaskDelete } from "../components/TaskDelete";
import { ErrorMessage, Spinner } from "../components/Feedback";
import { tasksApi } from "../api/tasks";
import type { TaskItem } from "../api/types";

export function TaskDetailPage() {
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

      if (result.failure.status === 401) {
        navigate("/login");
        return;
      }

      setError("This task could not be found.");
    });

    return () => {
      cancelled = true;
    };
  }, [id, navigate]);

  if (error) {
    return (
      <main className="page">
        <ErrorMessage message={error} onRetry={() => navigate("/tasks")} />
      </main>
    );
  }

  return (
    <main className="page">
      {task ? (
        <>
          <TaskDetail task={task} />
          <TaskDelete
            api={tasksApi}
            task={task}
            onDeleted={(deletedId) => navigate("/tasks", { state: { deletedTaskId: deletedId } })}
          />
        </>
      ) : (
        <Spinner label="Loading task…" />
      )}
    </main>
  );
}