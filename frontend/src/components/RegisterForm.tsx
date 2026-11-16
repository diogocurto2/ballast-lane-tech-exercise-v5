import { useState } from "react";
import { validateEmail, validatePassword } from "../api/validation";
import type { User, UsersApi } from "../api/types";

type RegisterFormProps = {
  api: UsersApi;
  onRegistered: (user: User) => void;
};

type FieldErrors = { email: string | null; password: string | null; form: string | null };

const noErrors: FieldErrors = { email: null, password: null, form: null };

export function RegisterForm({ api, onRegistered }: RegisterFormProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<FieldErrors>(noErrors);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const emailError = validateEmail(email);
    const passwordError = validatePassword(password);

    if (emailError || passwordError) {
      setErrors({ email: emailError, password: passwordError, form: null });
      return;
    }

    setErrors(noErrors);
    setIsSubmitting(true);

    try {
      const result = await api.register({ email: email.trim(), password });

      if (result.ok) {
        onRegistered(result.data);
        return;
      }

      setErrors({
        email: result.failure.errors.email?.[0] ?? null,
        password: result.failure.errors.password?.[0] ?? null,
        form: result.failure.detail ?? result.failure.title,
      });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>Create an account</h1>

      <div className="field">
        <label htmlFor="register-email">Email</label>
        <input
          id="register-email"
          name="email"
          type="email"
          autoComplete="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          aria-invalid={errors.email !== null}
          aria-describedby={errors.email ? "register-email-error" : undefined}
          required
        />
        {errors.email && (
          <p id="register-email-error" className="field-error" role="alert">
            {errors.email}
          </p>
        )}
      </div>

      <div className="field">
        <label htmlFor="register-password">Password</label>
        <input
          id="register-password"
          name="password"
          type="password"
          autoComplete="new-password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          aria-invalid={errors.password !== null}
          aria-describedby={errors.password ? "register-password-error" : "register-password-hint"}
          required
        />
        <p id="register-password-hint" className="field-hint">
          Minimum length 8 characters.
        </p>
        {errors.password && (
          <p id="register-password-error" className="field-error" role="alert">
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
        {isSubmitting ? "Creating account…" : "Create account"}
      </button>
    </form>
  );
}
