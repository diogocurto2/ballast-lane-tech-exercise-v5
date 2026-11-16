# Clean Architecture Standard

Layer responsibilities and dependency rules for this assessment.

## 1. Purpose and scope

This standard defines the responsibilities of each architectural layer, the allowed dependency directions, and how to verify them. It establishes **rules**, not the project structure: project names, folder layout, class design, database schema, and API contracts belong to the Spec Kit planning phase.

The assessment requires Clean Architecture principles, separation of concerns, and a business logic layer independent of the data layer and the API.

## 2. Mandatory rules

- Dependencies MUST point inward: `Domain` <- `Application` <- `Infrastructure` and `API`. A layer MUST NOT reference a layer that is further out.
- Business logic MUST remain independent of ASP.NET Core, Npgsql, PostgreSQL, and frontend frameworks. Domain and Application projects MUST NOT reference their assemblies.
- A layer MUST NOT access another layer's persistence or transport types directly. No DTOs from the API layer in the domain, no `NpgsqlConnection` in the application layer.
- Boundary interfaces MUST be owned by the consuming inner layer, not by the implementing outer layer.
- Composition roots MUST be the only place that wires infrastructure into the application.
- Cross-layer data MUST cross boundaries as explicit types (domain types or DTOs), not as loose dictionaries, `dynamic`, or serialized strings.
- Architecture MUST be verifiable: you MUST be able to inspect project references and demonstrate that the rules hold.

## 3. Layer responsibilities

| Layer | Responsibility | MUST NOT contain |
| --- | --- | --- |
| **Domain** | Entities, value objects, invariants, business rules that hold regardless of transport or storage. | HTTP, SQL, DI registration, serialization attributes, logging infrastructure. |
| **Application** | Use cases orchestrating domain rules, boundary interfaces (`ITaskRepository`, `IPasswordHasher`, `ITokenIssuer`), validation orchestration, DTO mapping for the boundary. | ASP.NET Core types, `Npgsql` types, raw SQL, HTTP status codes. |
| **Infrastructure** | Adapters implementing application boundaries: PostgreSQL access with Npgsql, password hashing, token generation, configuration binding. | Business rules. Business decisions must not migrate here. |
| **API / presentation** | HTTP endpoints, request/response DTOs, model binding, status-code mapping, authentication/authorization wiring, OpenAPI. Thin: translate and delegate. | Business rules, SQL, direct database access. |
| **Frontend** | User interface, client state, API access, form validation for user experience. | Business rules that must be authoritative server-side. |
| **Composition roots** | Construct the object graph: register application boundaries against infrastructure implementations, configure the host. | Business logic, conditional business branching, data access. |

## 4. Allowed dependency directions

```text
Domain          -> (no project dependencies)
Application     -> Domain
Infrastructure  -> Application, Domain
API             -> Application, Domain (NOT Infrastructure, except in the composition root)
Frontend        -> HTTP API contracts
```

Rules that follow:

- `Domain` MUST have no project references other than test projects.
- `Application` MUST depend only on `Domain` and framework-neutral packages.
- `Infrastructure` MAY reference `Application` and `Domain`, and the database driver.
- An API project MAY reference `Infrastructure` **only** to register implementations in its composition root. Business logic MUST NOT live in the API project, and moving it there to avoid calling a use case is prohibited.
- `Frontend` MUST depend only on the published HTTP contract, never on backend internals.

## 5. Dependency inversion and repository boundaries

- The application layer owns the boundary it needs. For example, `ITaskRepository` is declared in the application layer because use cases consume it.
- Infrastructure provides the adapter, such as `NpgsqlTaskRepository`. The dependency arrow points from infrastructure inward, never the reverse.
- A repository MUST express application intent, not mirror SQL. Prefer `ListByOwnerAsync(ownerId)` over `ExecuteQuery(string sql)`.
- Return types MUST be domain or application types, not database-specific types such as `NpgsqlDataReader`.
- Repository boundaries MUST NOT leak query builders, connections, or transactions to callers. If a caller needs atomicity, expose an explicit, intention-revealing operation or a documented transaction scope.

## 6. Practical examples

**Project references express the rules**

```xml
<!-- Domain: no project or framework dependencies -->
<Project Sdk="Microsoft.NET.Sdk" />

<!-- Application depends inward only -->
<ItemGroup>
  <ProjectReference Include="..\Domain\Domain.csproj" />
</ItemGroup>

<!-- Infrastructure implements the boundaries the application owns -->
<ItemGroup>
  <ProjectReference Include="..\Application\Application.csproj" />
  <PackageReference Include="Npgsql" Version="..." />
</ItemGroup>

<!-- API references infrastructure only for the composition root -->
<ItemGroup>
  <ProjectReference Include="..\Application\Application.csproj" />
  <ProjectReference Include="..\Infrastructure\Infrastructure.csproj" />
</ItemGroup>
```

**Use case with a boundary the application owns**

```csharp
namespace BallastLane.Application.Tasks;

public interface ITaskRepository
{
    Task<Guid> AddAsync(TaskDraft draft, Guid ownerId, CancellationToken cancellationToken);
}

public sealed class CreateTaskUseCase
{
    private readonly ITaskRepository _tasks;
    private readonly TaskValidator _validator;

    public CreateTaskUseCase(ITaskRepository tasks, TaskValidator validator)
    {
        _tasks = tasks;
        _validator = validator;
    }

    public async Task<Guid> ExecuteAsync(TaskDraft draft, Guid ownerId, CancellationToken cancellationToken)
    {
        var validation = _validator.Validate(draft);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);
        return await _tasks.AddAsync(draft, ownerId, cancellationToken);
    }
}
```

**Composition root: the only infrastructure-aware place in the API project**

```csharp
builder.Services.AddScoped<ITaskRepository, NpgsqlTaskRepository>();
builder.Services.AddScoped<CreateTaskUseCase>();
```

**Boundary mapping at the edge**

```csharp
[HttpPost]
public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
{
    var id = await _createTask.ExecuteAsync(request.ToDraft(), User.ToAuthenticatedUser().UserId, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id }, new { id });
}
```

**Framework-neutral business rule** - `TaskValidator` uses no ASP.NET Core, Npgsql, or serialization types, so it is testable without a host and reusable by both APIs.

## 7. DTO mapping

- Request and response DTOs MUST live at the API boundary and MUST be distinct from domain entities.
- Mapping MUST occur at the boundary: request DTO to an application input type, and application output to response DTO.
- Domain entities MUST NOT be serialized directly to clients. This prevents over-posting and accidental leakage of internal fields.
- Mapping MUST be explicit and testable. Reflection-based bulk mappers MAY be used only if their behavior is covered by tests; plain methods are acceptable and often clearer.

## 8. Application use cases

- Each use case SHOULD represent one operation with one intention, such as "create a task for the authenticated user".
- A use case MUST enforce business rules or delegate to the domain object that owns them. It MUST NOT depend on the HTTP context for identity; the caller passes a validated user identity.
- A use case MUST be callable from a test without a web host.
- Authorization decisions that depend on resource ownership MUST be expressed in the use case or domain, so both API hosts and tests enforce the same rule.

## 9. Framework-independent business rules

- Domain and application code MUST NOT use `[FromBody]`, `IActionResult`, `HttpContext`, `ILogger<T>` from ASP.NET Core, `Npgsql` types, or EF-style attributes.
- Time, randomness, and identity generation MUST be injected or passed in when they affect behavior, so tests are deterministic.
- Business exceptions SHOULD be framework-neutral; the API layer maps them to status codes and ProblemDetails.

## 10. Composition roots and multiple hosts

The assessment requires two APIs: one for data CRUD, and one for user creation, login, and public/protected endpoints.

- Each host project MUST reference the shared application and domain projects.
- Each host MUST register the infrastructure adapters it requires in its own composition root.
- Sharing core libraries MUST NOT be achieved by copying business logic into each host, or by putting shared logic in a shared "common" project that references both hosts.
- A host-specific concern (authentication middleware, CORS policy) stays in that host. A shared concern (task validation) stays in the application or domain layer.
- If both hosts run in one process for a single deployment, that MUST be a deliberate decision recorded in the plan, not an accident of project references.

## 11. Prohibited practices and anti-patterns

- Application or domain projects referencing `Npgsql`, `Microsoft.AspNetCore.*`, or frontend packages.
- Domain entities implementing persistence or transport concerns.
- Business rules duplicated in controllers and use cases.
- Infrastructure chosen by name inside the application layer (`new NpgsqlTaskRepository()`).
- A "shared" project that references outer layers to satisfy convenience.
- Returning domain entities directly as API responses.
- Bypassing a use case from a controller to "save a layer" for a single endpoint.
- An API project owning SQL.

## 12. Verification and review criteria

| Check | Method |
| --- | --- |
| Project references | Inspect each `.csproj`; confirm only allowed directions exist. |
| Framework independence | Confirm `Domain` and `Application` reference no ASP.NET Core or Npgsql assemblies. |
| Boundary ownership | Confirm each interface lives in the consuming layer. |
| Automated check | SHOULD add an architecture test that fails when a prohibited reference appears, so the rule is enforced and not merely documented. |
| Composition root | Confirm the only infrastructure usage in a host project is registration. |
| Multi-host sharing | Confirm both hosts consume the same application libraries and contain no duplicated business rules. |
| DTO isolation | Confirm no endpoint returns a domain entity directly. |

## 13. Deferred decisions

The following are **not** established here and MUST be decided in the Spec Kit planning phase: the final project and solution structure, naming, class design, database schema, API contracts, and whether both API hosts are separate projects or separate route groups.

## 14. References

- SOLID and abstraction boundaries: [solid-principles.md](./solid-principles.md)
- Persistence adapter rules: [database-standards.md](./database-standards.md)
- API boundary rules: [api-standards.md](./api-standards.md)
- Test organization by layer: [testing-and-tdd.md](./testing-and-tdd.md)
- Coding conventions: [coding-standards.md](./coding-standards.md)
