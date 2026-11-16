# Frontend Standards

React and TypeScript conventions for the assessment UI.

## 1. Purpose and scope

This standard defines how the frontend is structured and tested. React and TypeScript are the **proposed** baseline; the assessment allows a frontend framework of choice and requires responsive, user-friendly CRUD integration with structured components and clean state organization.

It does not define screens, routes, or components. Those belong to the Spec Kit planning phase. Dependencies MUST NOT be installed and the project MUST NOT be scaffolded until the plan is approved.

## 2. Mandatory rules

### 2.1 Component responsibilities

- Components MUST have a single, nameable responsibility. Split components that both fetch data and render complex presentation.
- Presentational components SHOULD receive data and callbacks through typed props and MUST NOT perform API calls.
- Container or page components MAY orchestrate data access and state, and MUST delegate rendering to presentational components.
- Components MUST be function components with typed props. Props MUST NOT be optional when a value is always required.
- Component files SHOULD be small; extract subcomponents rather than nesting deeply.

### 2.2 Separation of UI, state, and API access

- API access MUST be isolated in a dedicated module (client layer), not scattered through components.
- The API client MUST expose typed functions per resource operation, returning typed results or throwing typed errors.
- Application state MUST live in a predictable place: local component state for local concerns, and a shared store or context only when state is genuinely shared across routes.
- Derived values MUST be computed, not duplicated into state.
- The UI MUST NOT contain authoritative business rules. Validation duplicated for user experience MUST mirror, not replace, server validation.

### 2.3 Authentication state

- The frontend MUST track whether the user is authenticated and MUST render protected routes and actions accordingly.
- Session expiration MUST be handled: on a `401` response, the client MUST clear authentication state and redirect to login with a clear message.
- Tokens or session credentials MUST be handled per the chosen mechanism from [security.md](./security.md). If a bearer token is used, it MUST NOT be committed, logged, or embedded in URLs.
- Protected API calls MUST be centralized so authentication handling is implemented once.
- The UI MUST NOT decide authorization on behalf of the server. Hiding a control is usability, not security.

### 2.4 Responsive design

- Layouts MUST be usable from small mobile widths to desktop.
- Content MUST NOT require horizontal scrolling at common viewport widths.
- Tables and dense collections MUST degrade gracefully on narrow screens rather than overflow.
- Touch targets SHOULD be large enough to operate reliably on touch devices.

### 2.5 Accessibility and keyboard navigation

- Form inputs MUST have associated labels.
- Interactive elements MUST be reachable and operable by keyboard, in a logical order.
- Focus MUST be visible.
- Status and error messages MUST be conveyed to assistive technology, ideally through live regions.
- Buttons MUST be `<button>` elements; clickable `<div>` elements MUST NOT replace semantic controls.
- Color MUST NOT be the only signal for state or error.

### 2.6 Forms and validation feedback

- Forms MUST provide client-side validation for immediate feedback, and MUST surface server validation errors on submission.
- Error messages MUST be specific and actionable, and MUST be associated with the relevant field.
- Submit actions MUST be disabled or guarded while a request is in flight to prevent duplicate submissions.
- Field-level and form-level errors MUST be distinguished.

### 2.7 UI states

- Every data-driven view MUST handle: loading, empty, success, and error.
- Loading MUST be visible, not a frozen screen.
- Empty state MUST explain the situation and offer a next action where relevant.
- Errors MUST be human-readable and MUST NOT display raw HTTP bodies or stack traces.
- After a mutation, the view MUST reflect the new state without requiring a full page reload.

### 2.8 CRUD flows

- The frontend MUST implement create, read, update, and delete flows against the API, consistent with [api-standards.md](./api-standards.md).
- Destructive actions MUST require explicit confirmation.
- Optimistic updates MAY be used only where failure handling reverts the UI state.

### 2.9 State management dependencies

- Global state libraries MUST NOT be added without a demonstrated need. Component state, hooks, and context are sufficient for this assessment's scope.
- Data-fetching libraries MAY be used if the plan records the justification and the dependency is maintained.
- Each added dependency MUST be justified by a named problem it solves.

### 2.10 Testing and linting

- Frontend code MUST pass lint and type checking with TypeScript strict mode; see [coding-standards.md](./coding-standards.md).
- Normal use of the application SHOULD produce no warnings or errors in the browser console. The assessment lists this as optional but desired, and it is a strong signal of correct React usage such as keyed lists, effect cleanup, and controlled inputs.
- Component tests MUST cover meaningful behavior: rendering of states, form validation feedback, and user interactions for CRUD flows.
- At least one user-flow test SHOULD cover login and a full CRUD sequence.
- Tests MUST NOT assert on implementation details such as internal state or CSS class names where a user-visible assertion is possible.
- Tests MUST be reproducible and independent.

## 3. Practical examples

**Typed API client separated from UI**

```ts
export async function createTask(input: CreateTaskInput): Promise<TaskItem> {
  const response = await fetch("/api/tasks", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(input),
  });
  if (response.status === 401) throw new UnauthorizedError();
  if (!response.ok) throw await ApiError.fromResponse(response);
  return (await response.json()) as TaskItem;
}
```

**Explicit UI states**

```tsx
if (status === "loading") return <Spinner label="Loading tasks" />;
if (status === "error") return <ErrorMessage message="Could not load tasks." onRetry={refetch} />;
if (items.length === 0) return <EmptyState message="No tasks yet." action={<NewTaskButton />} />;
return <TaskList items={items} />;
```

**Accessible field with error**

```tsx
<label htmlFor="title">Title</label>
<input id="title" aria-describedby="title-error" aria-invalid={!!error} value={title} onChange={...} />
{error && <p id="title-error" role="alert">{error}</p>}
```

## 4. Prohibited practices and anti-patterns

- `any`, untyped props, or `as` used to silence type errors.
- API calls scattered across components with no client layer.
- Storing derived values in state and letting them drift.
- Using the UI to enforce authorization.
- Rendering raw error objects or stack traces to users.
- Missing loading or empty states, producing blank screens.
- Adding a state library or UI framework for a handful of forms.
- Clickable `<div>` elements instead of semantic controls.
- Tests asserting CSS classes or internal state instead of user-visible behavior.
- Tokens stored in `localStorage` without a recorded decision and its trade-offs.
- Installing dependencies or scaffolding the project before the plan is approved.

## 5. Verification and review criteria

| Check | Question |
| --- | --- |
| Separation | Is API access isolated from components, and state placed at the right scope? |
| Typing | Does TypeScript strict pass with no `any`? |
| Auth handling | Is `401` handled centrally, with state cleared and the user redirected? |
| States | Are loading, empty, success, and error each implemented? |
| Accessibility | Are labels, keyboard operation, focus visibility, and live-region errors present? |
| Responsiveness | Does the layout work at mobile and desktop widths? |
| CRUD | Are all four flows implemented and connected to the API? |
| Tests | Do component and flow tests assert user-visible behavior and pass reproducibly? |
| Dependencies | Is each added dependency justified? |

## 6. Deferred decisions

The frontend framework confirmation, router, state approach, test runner, styling solution, and credential storage location for tokens are planning decisions. They MUST be recorded in the plan before scaffolding.

## 7. References

- Typing and component conventions: [coding-standards.md](./coding-standards.md)
- Authentication model and credential rules: [security.md](./security.md)
- API contracts consumed by the UI: [api-standards.md](./api-standards.md)
- Test levels and independence: [testing-and-tdd.md](./testing-and-tdd.md)
- Build and lint automation: [delivery-and-ci.md](./delivery-and-ci.md)
- Assessment frontend criteria: [technical-exercise-v5.md](../assessment/technical-exercise-v5.md)
