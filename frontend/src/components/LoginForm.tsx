import { useState } from "react";
import type { LoginResponse, UsersApi } from "../api/types";

type LoginFormProps = {
  api: UsersApi;
  onAuthenticated: (response: LoginResponse) => void;
};

type FieldErrors = { email: string | null; password: string | null; form: string | null };

const noErrors: FieldErrors = { email: null, password: null, form: null };

export function LoginForm({ api, onAuthenticated }: LoginFormProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors>(noErrors);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const emailError = email.trim().length === 0 ? "Enter your email address." : null;
    const passwordError = password.length === 0 ? "Enter a password." : null;

    if (emailError || passwordError) {
      setErrors({ email: emailError, password: passwordError, form: null });
      return;
    }

    setErrors(noErrors);
    setIsSubmitting(true);

    try {
      const result = await api.login({ email: email.trim(), password });

      if (result.ok) {
        onAuthenticated(result.data);
        return;
      }

      // One generic message: the server does not distinguish an unknown email from a wrong password,
      // and the interface must not imply a difference it cannot know.
      setErrors({
        email: result.failure.errors.email?.[0] ?? null,
        password: result.failure.errors.password?.[0] ?? null,
        form:
          result.failure.errors.email || result.failure.errors.password
            ? null
            : (result.failure.detail ?? result.failure.title),
      });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>Log in</h1>

      <div className="field">
        <label htmlFor="login-email">Email</label>
        <input
          id="login-email"
          name="email"
          type="email"
          autoComplete="username"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          aria-invalid={errors.email !== null}
          aria-describedby={errors.email ? "login-email-error" : undefined}
          required
        />
        {errors.email && (
          <p id="login-email-error" className="field-error" role="alert">
            {errors.email}
          </p>
        )}
      </div>

      <div className="field">
        <label htmlFor="login-password">Password</label>
        <input
          id="login-password"
          name="password"
          type="password"
          autoComplete="current-password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          aria-invalid={errors.password !== null}
          aria-describedby={errors.password ? "login-password-error" : undefined}
          required
        />
        {errors.password && (
          <p id="login-password-error" className="field-error" role="alert">
            {errors.password}
          </p>
        )}
      </div>

      {errors.form && (
        <p className="form-error" role="alert">
          {errors.form}
        </p>
      )}

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Logging in…" : "Log in"}
      </button>
    </form>
  );
}
