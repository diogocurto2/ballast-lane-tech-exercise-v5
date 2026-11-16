export type User = {
  id: string;
  email: string;
  createdAt: string;
};

export type Credentials = {
  email: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  tokenType: string;
  expiresAtUtc: string;
};

export type FieldErrors = Record<string, string[]>;

export type ApiFailure = {
  status: number;
  title: string;
  detail: string | null;
  errors: FieldErrors;
};

export type ApiResult<T> =
  | { ok: true; data: T }
  | { ok: false; failure: ApiFailure };

export interface UsersApi {
  register(credentials: Credentials): Promise<ApiResult<User>>;
  login(credentials: Credentials): Promise<ApiResult<LoginResponse>>;
  currentUser(): Promise<ApiResult<User>>;
}

export type TaskStatus = "Pending" | "InProgress" | "Completed";

export type TaskItem = {
  id: string;
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string;
  createdAt: string;
  updatedAt: string;
};

export type TaskDraft = {
  title: string;
  description: string | null;
  status: TaskStatus;
  dueDate: string;
};

export type TaskListState =
  | { status: "loading" }
  | { status: "success"; tasks: TaskItem[] }
  | { status: "error"; message: string };

export interface TasksApi {
  list(): Promise<ApiResult<TaskItem[]>>;
  getById(id: string): Promise<ApiResult<TaskItem>>;
  create(draft: TaskDraft): Promise<ApiResult<TaskItem>>;
  update(id: string, draft: TaskDraft): Promise<ApiResult<TaskItem>>;
  remove(id: string): Promise<ApiResult<void>>;
}

