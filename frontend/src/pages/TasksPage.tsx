import { useCallback, useEffect, useReducer, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { TaskList } from "../components/TaskList";
import { tasksApi } from "../api/tasks";
import { initialTaskStore, tasksReducer } from "../state/tasksReducer";
import { useAuth } from "../auth/useAuth";
import type { TaskItem } from "../api/types";

export function TasksPage() {
  const [store, dispatch] = useReducer(tasksReducer, initialTaskStore);
  const [requestKey, setRequestKey] = useState(0);
  const navigate = useNavigate();
  const auth = useAuth();
  const location = useLocation();

  // A task created on the create page arrives through router state, so the list shows it immediately
  // instead of refetching the whole collection.
  const routeState = location.state as
    | { createdTask?: TaskItem; updatedTask?: TaskItem; deletedTaskId?: string }
    | null;

  const createdTask = routeState?.createdTask;
  const updatedTask = routeState?.updatedTask;
  const deletedTaskId = routeState?.deletedTaskId;

  useEffect(() => {
    if (createdTask) {
      dispatch({ type: "added", task: createdTask });
      navigate("/tasks", { replace: true, state: null });
    }
  }, [createdTask, navigate]);

  useEffect(() => {
    if (updatedTask) {
      dispatch({ type: "updated", task: updatedTask });
      navigate("/tasks", { replace: true, state: null });
    }
  }, [updatedTask, navigate]);

  useEffect(() => {
    if (deletedTaskId) {
      dispatch({ type: "removed", id: deletedTaskId });
      navigate("/tasks", { replace: true, state: null });
    }
  }, [deletedTaskId, navigate]);

  useEffect(() => {
    let cancelled = false;
    dispatch({ type: "loading" });

    void tasksApi.list().then((result) => {
      if (cancelled) {
        return;
      }

      if (result.ok) {
        dispatch({ type: "loaded", tasks: result.data });
        return;
      }

      if (result.failure.status === 401) {
        auth.logout();
        navigate("/login");
        return;
      }

      dispatch({ type: "failed", message: result.failure.detail ?? result.failure.title });
    });

    return () => {
      cancelled = true;
    };
  }, [requestKey, auth, navigate]);

  const handleSelect = useCallback(
    (task: TaskItem) => navigate(`/tasks/${task.id}`),
    [navigate],
  );

  const handleCreate = useCallback(() => navigate("/tasks/new"), [navigate]);

  return (
    <main className="page">
      <header className="page-header">
        <h1>Your tasks</h1>
        <button type="button" onClick={handleCreate}>
          New task
        </button>
        <button type="button" onClick={auth.logout}>
          Log out
        </button>
      </header>

      <TaskList
        state={store}
        onSelect={handleSelect}
        onCreate={handleCreate}
        onRetry={() => setRequestKey((key) => key + 1)}
      />
    </main>
  );
}


