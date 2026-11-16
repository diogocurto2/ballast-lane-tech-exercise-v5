import { useState } from "react";
import { isSessionExpired } from "../api/client";
import type { TaskDraft, TaskItem, TaskStatus, TasksApi } from "../api/types";

type TaskFormProps = {
  api: TasksApi;
  mode: { kind: "create" } | { kind: "edit"; task: TaskItem };
  onSaved: (task: TaskItem) => void;
  onCancel: () => void;
};

type FieldErrors = {
  title: string | null;
  dueDate: string | null;
  description: string | null;
  status: string | null;
  form: string | null;
};

const statuses: TaskStatus[] = ["Pending", "InProgress", "Completed"];

const calendarDatePattern = /^\d{4}-\d{2}-\d{2}$/;

const noErrors: FieldErrors = {
  title: null,
  dueDate: null,
  description: null,
  status: null,
  form: null,
};

function initialValues(mode: TaskFormProps["mode"]): TaskDraft {
  if (mode.kind === "edit") {
    return {
      title: mode.task.title,
      description: mode.task.description,
      status: mode.task.status,
      dueDate: mode.task.dueDate,
    };
  }

  return { title: "", description: null, status: "Pending", dueDate: "" };
}

function validate(values: TaskDraft): FieldErrors {
  const errors: FieldErrors = { ...noErrors };

  if (values.title.trim().length === 0) {
    errors.title = "Title is required.";
  }

  if (values.dueDate.trim().length === 0) {
    errors.dueDate = "A due date is required.";
  } else if (!calendarDatePattern.test(values.dueDate.trim())) {
    errors.dueDate = "Enter the due date as a calendar date (YYYY-MM-DD).";
  }

  return errors;
}

/**
 * One form for creating and editing. Client validation mirrors the server's rules for immediate
 * feedback; the server stays authoritative and its field errors appear on the same fields.
 */
export function TaskForm({ api, mode, onSaved, onCancel }: TaskFormProps) {
  const [values, setValues] = useState<TaskDraft>(() => initialValues(mode));
  const [errors, setErrors] = useState<FieldErrors>(noErrors);
  const [isSaving, setIsSaving] = useState(false);

  const isEdit = mode.kind === "edit";

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const validationErrors = validate(values);
    if (validationErrors.title || validationErrors.dueDate) {
      setErrors(validationErrors);
      return;
    }

    setErrors(noErrors);
    setIsSaving(true);

    try {
      const trimmedDescription = values.description?.trim() ?? "";
      const payload: TaskDraft = {
        ...values,
        title: values.title.trim(),
        description: trimmedDescription.length > 0 ? trimmedDescription : null,
      };

      const result = isEdit ? await api.update(mode.task.id, payload) : await api.create(payload);

      if (result.ok) {
        onSaved(result.data);
        return;
      }

      // A rejected credential is handled centrally, so the form must not report it as its own error.
      if (isSessionExpired(result.failure)) {
        return;
      }

      setErrors({
        title: result.failure.errors.title?.[0] ?? null,
        dueDate: result.failure.errors.dueDate?.[0] ?? null,
        description: result.failure.errors.description?.[0] ?? null,
        status: result.failure.errors.status?.[0] ?? null,
        form: result.failure.detail ?? result.failure.title,
      });
    } finally {
      setIsSaving(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>{isEdit ? "Edit task" : "New task"}</h1>

      <div className="field">
        <label htmlFor="task-title">Title</label>
        <input
          id="task-title"
          value={values.title}
          onChange={(event) => setValues({ ...values, title: event.target.value })}
          aria-invalid={errors.title !== null}
          aria-describedby={errors.title ? "task-title-error" : undefined}
          required
        />
        {errors.title && (
          <p id="task-title-error" className="field-error" role="alert">
            {errors.title}
          </p>
        )}
      </div>

      <div className="field">
        <label htmlFor="task-description">Description</label>
        <textarea
          id="task-description"
          value={values.description ?? ""}
          onChange={(event) => setValues({ ...values, description: event.target.value })}
          aria-invalid={errors.description !== null}
          aria-describedby={errors.description ? "task-description-error" : undefined}
        />
        {errors.description && (
          <p id="task-description-error" className="field-error" role="alert">
            {errors.description}
          </p>
        )}
      </div>

      <div className="field">
        <label htmlFor="task-due-date">Due date</label>
        <input
          id="task-due-date"
          type="date"
          value={values.dueDate}
          onChange={(event) => setValues({ ...values, dueDate: event.target.value })}
          aria-invalid={errors.dueDate !== null}
          aria-describedby={errors.dueDate ? "task-due-date-error" : undefined}
          required
        />
        {errors.dueDate && (
          <p id="task-due-date-error" className="field-error" role="alert">
            {errors.dueDate}
          </p>
        )}
      </div>

      <div className="field">
        <label htmlFor="task-status">Status</label>
        <select
          id="task-status"
          value={values.status}
          onChange={(event) => setValues({ ...values, status: event.target.value as TaskStatus })}
        >
          {statuses.map((status) => (
            <option key={status} value={status}>
              {status}
            </option>
          ))}
        </select>
      </div>

      {errors.form && (
        <p className="form-error" role="alert">
          {errors.form}
        </p>
      )}

      <div className="form-actions">
        <button type="submit" disabled={isSaving}>
          {isSaving ? "Saving…" : isEdit ? "Save changes" : "Create task"}
        </button>
        <button type="button" onClick={onCancel} disabled={isSaving}>
          Cancel
        </button>
      </div>
    </form>
  );
}
