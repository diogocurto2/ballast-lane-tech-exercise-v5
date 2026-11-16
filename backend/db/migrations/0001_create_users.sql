-- Accounts. Email uniqueness is case-insensitive, enforced by a unique index on lower(email), so the
-- application never has to win a race to protect it.
CREATE TABLE users (
    id uuid NOT NULL,
    email text NOT NULL,
    password_hash text NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE UNIQUE INDEX users_email_lower_uq ON users (lower(email));
