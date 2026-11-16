import { request, USER_API_BASE_URL } from "./client";
import { getAccessToken, setAccessToken } from "./authToken";
import type { ApiResult, Credentials, LoginResponse, User, UsersApi } from "./types";

const USERS_PATH = "/api/users";

/**
 * Builds the User API client. A successful login records the bearer token in the shared session
 * store, which the Task API client reads, so components never handle the credential themselves.
 */
export function createUsersApi(): UsersApi {
  return {
    async register(credentials: Credentials): Promise<ApiResult<User>> {
      return request<User>(USER_API_BASE_URL, `${USERS_PATH}/register`, {
        method: "POST",
        body: credentials,
      });
    },

    async login(credentials: Credentials): Promise<ApiResult<LoginResponse>> {
      const result = await request<LoginResponse>(USER_API_BASE_URL, `${USERS_PATH}/login`, {
        method: "POST",
        body: credentials,
      });

      setAccessToken(result.ok ? result.data.accessToken : null);
      return result;
    },

    async currentUser(): Promise<ApiResult<User>> {
      return request<User>(USER_API_BASE_URL, `${USERS_PATH}/me`, { accessToken: getAccessToken() });
    },
  };
}

export const usersApi = createUsersApi();