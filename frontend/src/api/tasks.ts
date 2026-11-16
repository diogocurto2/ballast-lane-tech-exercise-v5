import { request, TASK_API_BASE_URL } from "./client";
import { getAccessToken } from "./authToken";
import type { ApiResult, TaskDraft, TaskItem, TasksApi } from "./types";

const TASKS_PATH = "/api/tasks";

/**
 * Builds the Task API client. It shares the bearer token obtained at login with the User API client,
 * so protected calls are authenticated in one place.
 */
export function createTasksApi(getAccessToken: () => string | null): TasksApi {
  return {
    list(): Promise<ApiResult<TaskItem[]>> {
      return request<TaskItem[]>(TASK_API_BASE_URL, TASKS_PATH, { accessToken: getAccessToken() });
    },

    getById(id: string): Promise<ApiResult<TaskItem>> {
      return request<TaskItem>(TASK_API_BASE_URL, `${TASKS_PATH}/${id}`, {
        accessToken: getAccessToken(),
      });
    },

    create(draft: TaskDraft): Promise<ApiResult<TaskItem>> {
      return request<TaskItem>(TASK_API_BASE_URL, TASKS_PATH, {
        method: "POST",
        body: draft,
        accessToken: getAccessToken(),
      });
    },

    update(id: string, draft: TaskDraft): Promise<ApiResult<TaskItem>> {
      return request<TaskItem>(TASK_API_BASE_URL, `${TASKS_PATH}/${id}`, {
        method: "PUT",
        body: draft,
        accessToken: getAccessToken(),
      });
    },

    remove(id: string): Promise<ApiResult<void>> {
      return request<void>(TASK_API_BASE_URL, `${TASKS_PATH}/${id}`, {
        method: "DELETE",
        accessToken: getAccessToken(),
      });
    },
  };
}

export const tasksApi = createTasksApi(getAccessToken);
