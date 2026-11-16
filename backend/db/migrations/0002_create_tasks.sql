-- Personal tasks. Ownership is the authorization boundary, so owner_id is never null and always
-- indexed ahead of the list ordering columns.
CREATE TABLE tasks (
    id uuid NOT NULL,
    owner_id uuid NOT NULL,
    title text NOT NULL,
    description text NULL,
    status text NOT NULL DEFAULT 'Pending',
    due_date date NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT pk_tasks PRIMARY KEY (id),
    CONSTRAINT fk_tasks_users FOREIGN KEY (owner_id) REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT ck_tasks_title_length CHECK (length(btrim(title)) BETWEEN 1 AND 200),
    CONSTRAINT ck_tasks_description_length CHECK (description IS NULL OR length(description) <= 2000),
    CONSTRAINT ck_tasks_status CHECK (status IN ('Pending', 'InProgress', 'Completed'))
);

CREATE INDEX ix_tasks_owner_due_created ON tasks (owner_id, due_date, created_at);
