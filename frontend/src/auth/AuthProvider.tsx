import { useCallback, useEffect, useMemo, useRef, useState, type ReactNode } from "react";
import type { Credentials, User, UsersApi } from "../api/types";
import { setAccessToken, subscribeToSessionInvalidation } from "../api/authToken";
import { AuthContext, type AuthState } from "./AuthContext";

type AuthProviderProps = {
  api: UsersApi;
  children: ReactNode;
};

const SESSION_EXPIRED_MESSAGE = "Your session has expired. Log in again.";

export function AuthProvider({ api, children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Mirrors the identity so the invalidation callback can tell an established session from a
  // sign-in that has not completed, without re-subscribing on every render.
  const establishedUser = useRef<User | null>(null);

  const applyUser = useCallback((next: User | null) => {
    establishedUser.current = next;
    setUser(next);
  }, []);

  /** Ends the session everywhere: the in-memory credential, the identity, and any notice. */
  const endSession = useCallback(
    (notice: string | null) => {
      setAccessToken(null);
      applyUser(null);
      setError(notice);
    },
    [applyUser],
  );

  const logout = useCallback(() => endSession(null), [endSession]);

  useEffect(
    () =>
      subscribeToSessionInvalidation(() => {
        // A rejection during sign-in is reported by the form that caused it, so the session notice
        // is reserved for a session that had already been established.
        if (establishedUser.current === null) {
          applyUser(null);
          return;
        }

        endSession(SESSION_EXPIRED_MESSAGE);
      }),
    [applyUser, endSession],
  );

  const login = useCallback(
    async (credentials: Credentials) => {
      setError(null);
      const result = await api.login(credentials);

      if (!result.ok) {
        return result;
      }

      // A token alone is not a session: the identity behind it must resolve before the interface
      // may treat the user as signed in.
      const me = await api.currentUser();

      if (!me.ok) {
        endSession(null);
        return {
          ok: false as const,
          failure:
            me.failure.status === 401
              ? { ...me.failure, detail: SESSION_EXPIRED_MESSAGE }
              : me.failure,
        };
      }

      applyUser(me.data);
      return result;
    },
    [api, applyUser, endSession],
  );

  const refreshCurrentUser = useCallback(async () => {
    const result = await api.currentUser();

    if (result.ok) {
      applyUser(result.data);
      return;
    }

    // An expired or rejected credential clears the session rather than leaving a stale identity.
    if (result.failure.status === 401) {
      endSession(SESSION_EXPIRED_MESSAGE);
      return;
    }

    setError(result.failure.detail ?? result.failure.title);
  }, [api, applyUser, endSession]);

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
