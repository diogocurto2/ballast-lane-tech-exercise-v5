/**
 * Holds the bearer token for the current session in memory only. Persisting it in browser storage
 * would make it readable after the tab closes and by any injected script, so a reload requires a fresh
 * login instead.
 */
let accessToken: string | null = null;

export function setAccessToken(token: string | null): void {
  accessToken = token;
}

export function getAccessToken(): string | null {
  return accessToken;
}
