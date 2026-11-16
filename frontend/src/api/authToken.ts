/**
 * Owns the bearer token for the current session in memory only, and broadcasts the single
 * invalidation event every transport uses. Persisting the token in browser storage would make it
 * readable after the tab closes and by any injected script, so a reload requires a fresh login
 * instead.
 *
 * This module deliberately depends on nothing, so the API client can report a rejected credential
 * without importing the React authentication provider, and the provider can subscribe without a
 * circular dependency.
 */
let accessToken: string | null = null;

type SessionInvalidationListener = () => void;

const invalidationListeners = new Set<SessionInvalidationListener>();

export function setAccessToken(token: string | null): void {
  accessToken = token;
}

export function getAccessToken(): string | null {
  return accessToken;
}

/**
 * Registers a callback that runs when the server rejects the current credential. The returned
 * function unsubscribes.
 */
export function subscribeToSessionInvalidation(listener: SessionInvalidationListener): () => void {
  invalidationListeners.add(listener);

  return () => {
    invalidationListeners.delete(listener);
  };
}

/**
 * Discards the credential and tells every subscriber the session is over. The token is cleared
 * before the notification, so a listener that issues another request cannot reuse it.
 */
export function invalidateSession(): void {
  accessToken = null;

  for (const listener of [...invalidationListeners]) {
    listener();
  }
}
