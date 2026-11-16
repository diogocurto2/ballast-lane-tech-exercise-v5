import { Navigate, useLocation } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "./useAuth";

type RequireAuthProps = {
  children: ReactNode;
};

/**
 * Redirects an anonymous visitor to the login page, preserving where they were going. This is a
 * usability guard only: the API still enforces authentication on every protected request.
 */
export function RequireAuth({ children }: RequireAuthProps) {
  const auth = useAuth();
  const location = useLocation();

  if (!auth.isAuthenticated) {
    return <Navigate to="/login" state={{ from: location.pathname }} replace />;
  }

  return <>{children}</>;
}
