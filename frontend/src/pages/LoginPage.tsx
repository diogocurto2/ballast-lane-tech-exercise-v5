import { Link, useNavigate } from "react-router-dom";
import { LoginForm } from "../components/LoginForm";
import { useAuth } from "../auth/useAuth";
import { usersApi } from "../api/users";
import type { UsersApi } from "../api/types";

export function LoginPage() {
  const auth = useAuth();
  const navigate = useNavigate();

  // Route the form's login through the auth context so the session is recorded in one place.
  const api: UsersApi = {
    register: usersApi.register,
    currentUser: usersApi.currentUser,
    login: (credentials) => auth.login(credentials),
  };

  return (
    <main className="page">
      <LoginForm api={api} onAuthenticated={() => navigate("/tasks")} />
      {auth.error && (
        <p className="form-error" role="alert">
          {auth.error}
        </p>
      )}
      <p>
        Need an account? <Link to="/register">Register</Link>
      </p>
    </main>
  );
}
