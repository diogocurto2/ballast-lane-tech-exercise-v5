import { useState } from "react";
import { isSessionExpired } from "../api/client";
import type { TaskItem, TasksApi } from "../api/types";
import { ConfirmDialog } from "./ConfirmDialog";

type TaskDeleteProps = {
  api: TasksApi;
  task: TaskItem;
  onDeleted: (id: string) => void;
};

/**
 * Destructive action with an explicit confirmation step, so a single click cannot delete a task.
 */
export function TaskDelete({ api, task, onDeleted }: TaskDeleteProps) {
  const [isConfirming, setIsConfirming] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleConfirm() {
    setIsDeleting(true);
    setError(null);

    try {
      const result = await api.remove(task.id);

      if (result.ok) {
        setIsConfirming(false);
        onDeleted(task.id);
        return;
      }

      // A rejected credential ends the session centrally; reporting a delete failure would be wrong.
      if (isSessionExpired(result.failure)) {
        return;
      }

      setError("The task could not be deleted. It may have been removed already.");
      setIsConfirming(false);
    } finally {
      setIsDeleting(false);
    }
  }

  return (
    <div className="task-delete">
      <button type="button" onClick={() => setIsConfirming(true)}>
        Delete task
      </button>

      {error && (
        <p className="form-error" role="alert">
          {error}
        </p>
      )}

      {isConfirming && (
        <ConfirmDialog
          title="Delete this task?"
          message={`"${task.title}" will be permanently removed.`}
          confirmLabel="Yes, delete"
          cancelLabel="Cancel"
          isBusy={isDeleting}
          onConfirm={() => void handleConfirm()}
          onCancel={() => setIsConfirming(false)}
        />
      )}
    </div>
  );
}