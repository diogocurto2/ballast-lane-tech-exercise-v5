import { Link } from "react-router-dom";
import { RegisterForm } from "../components/RegisterForm";
import { useAuth } from "../auth/useAuth";
import { usersApi } from "../api/users";
import { useState } from "react";

export function RegisterPage() {
  const auth = useAuth();
  const [registeredEmail, setRegisteredEmail] = useState<string | null>(null);

  if (registeredEmail) {
    return (
      <main className="page">
        <h1>Account created</h1>
        <p role="status">You can now log in as {registeredEmail}.</p>
        <p className="page-note">
          <Link to="/login">Go to login</Link>
        </p>
      </main>
    );
  }

  return (
    <main className="page">
      {auth.error && (
        <p className="notice" role="alert">
          {auth.error}
        </p>
      )}
      <RegisterForm api={usersApi} onRegistered={(user) => setRegisteredEmail(user.email)} />
      <p className="page-note">
        Already have an account? <Link to="/login">Log in</Link>
      </p>
    </main>
  );
}
