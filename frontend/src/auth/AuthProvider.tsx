import { useCallback, useMemo, useState, type ReactNode } from "react";
import type { Credentials, User, UsersApi } from "../api/types";
import { AuthContext, type AuthState } from "./AuthContext";

type AuthProviderProps = {
  api: UsersApi;
  children: ReactNode;
};

export function AuthProvider({ api, children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);

  const login = useCallback(
    async (credentials: Credentials) => {
      setError(null);
      const result = await api.login(credentials);

      if (!result.ok) {
        setError(result.failure.detail ?? result.failure.title);
        return result;
      }

      const me = await api.currentUser();

      if (me.ok) {
        setUser(me.data);
      } else if (me.failure.status === 401) {
        setUser(null);
        setError("Your session expired. Log in again.");
        return { ok: false as const, failure: me.failure };
      }

      return result;
    },
    [api],
  );

  const logout = useCallback(() => {
    setUser(null);
    setError(null);
  }, []);

  const refreshCurrentUser = useCallback(async () => {
    const result = await api.currentUser();

    if (result.ok) {
      setUser(result.data);
      return;
    }

    // An expired or rejected credential clears the session rather than leaving a stale identity.
    if (result.failure.status === 401) {
      logout();
      setError("Your session expired. Log in again.");
      return;
    }

    setError(result.failure.detail ?? result.failure.title);
  }, [api, logout]);

  const value = useMemo<AuthState>(
    () => ({
      isAuthenticated: user !== null,
      user,
      error,
      login,
      logout,
      refreshCurrentUser,
    }),
    [user, error, login, logout, refreshCurrentUser],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
