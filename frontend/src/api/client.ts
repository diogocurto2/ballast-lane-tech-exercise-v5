import { getAccessToken, invalidateSession } from "./authToken";
import type { ApiFailure, ApiResult } from "./types";

const USER_API_BASE_URL = import.meta.env.VITE_USER_API_URL ?? "http://localhost:5081";
const TASK_API_BASE_URL = import.meta.env.VITE_TASK_API_URL ?? "http://localhost:5082";

/** True when a failure means the credential is no longer usable, so the caller must not report it. */
export function isSessionExpired(failure: ApiFailure): boolean {
  return failure.status === 401;
}

type RequestOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  accessToken?: string | null;
  signal?: AbortSignal;
};

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null;
}

async function toFailure(response: Response): Promise<ApiFailure> {
  let title = response.statusText || "The request failed.";
  let detail: string | null = null;
  let errors: Record<string, string[]> = {};

  try {
    const parsed: unknown = await response.json();
    if (isRecord(parsed)) {
      if (typeof parsed.title === "string") {
        title = parsed.title;
      }
      if (typeof parsed.detail === "string") {
        detail = parsed.detail;
      }
      if (isRecord(parsed.errors)) {
        errors = Object.fromEntries(
          Object.entries(parsed.errors)
            .filter((entry): entry is [string, string[]] => Array.isArray(entry[1]))
            .map((entry) => [entry[0], entry[1].map(String)]),
        );
      }
    }
  } catch {
    // A non-JSON body is not exposed to the user; the status text stands in for it.
  }

  return { status: response.status, title, detail, errors };
}

async function request<T>(baseUrl: string, path: string, options: RequestOptions = {}): Promise<ApiResult<T>> {
  const headers: Record<string, string> = { Accept: "application/json" };

  if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
  }

  if (options.accessToken) {
    headers.Authorization = `Bearer ${options.accessToken}`;
  }

  let response: Response;
  try {
    response = await fetch(`${baseUrl}${path}`, {
      method: options.method ?? "GET",
      headers,
      body: options.body === undefined ? undefined : JSON.stringify(options.body),
      signal: options.signal,
    });
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") {
      throw error;
    }

    return {
      ok: false,
      failure: {
        status: 0,
        title: "The server could not be reached.",
        detail: "Check that the API is running and try again.",
        errors: {},
      },
    };
  }

  if (response.status === 204) {
    return { ok: true, data: undefined as T };
  }

  if (!response.ok) {
    const failure = await toFailure(response);

    // One place decides that an authenticated credential is no longer usable, so every consumer
    // observes the same session end instead of each screen inventing its own redirect. Only the
    // credential still in use may end the session: a slow answer for a token that has since been
    // replaced says nothing about the session that replaced it.
    if (failure.status === 401 && options.accessToken === getAccessToken()) {
      invalidateSession();
    }

    return { ok: false, failure };
  }

  return { ok: true, data: (await response.json()) as T };
}

export { USER_API_BASE_URL, TASK_API_BASE_URL, request };
