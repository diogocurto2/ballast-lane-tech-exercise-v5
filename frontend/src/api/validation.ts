export function validateEmail(value: string): string | null {
  const trimmed = value.trim();

  if (trimmed.length === 0) {
    return "Enter your email address.";
  }

  if (trimmed.length > 320) {
    return "Use an email address of 320 characters or fewer.";
  }

  const parts = trimmed.split("@");
  if (parts.length !== 2 || parts[0].length === 0 || !parts[1].includes(".")) {
    return "Enter a valid email address.";
  }

  return null;
}

export function validatePassword(value: string): string | null {
  if (value.length === 0) {
    return "Enter a password.";
  }

  if (value.length < 8) {
    return "Use a password of at least 8 characters.";
  }

  return null;
}
