import { createContext } from "react";

import type { ApiResult, Credentials, LoginResponse, User } from "../api/types";

export type AuthState = {
  isAuthenticated: boolean;
  user: User | null;
  error: string | null;
  /** Authenticates and records the session. The result is returned so a page can react to failure. */
  login: (credentials: Credentials) => Promise<ApiResult<LoginResponse>>;
  logout: () => void;
  refreshCurrentUser: () => Promise<void>;
};

export const AuthContext = createContext<AuthState | null>(null);
