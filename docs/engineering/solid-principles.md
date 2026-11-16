# SOLID Principles Standard

How SOLID principles are applied in this assessment, and where abstraction is not justified.

## 1. Purpose and scope

This standard defines the five SOLID principles, how they apply to controllers, application use cases, domain entities, persistence adapters, and tests, and how to tell justified design from unnecessary abstraction.

SOLID is a means to readable, changeable code in a small assessment. It is not a mandate to add interfaces, factories, or layers.

## 2. Mandatory rules

- Each type SHOULD have one reason to change (**SRP**). When a class mixes concerns such as validation, persistence, and HTTP concerns, split it.
- Behavior SHOULD be extended without editing working code where variation actually exists (**OCP**). Do not introduce extension points for variation that does not exist.
- Derived types MUST honor the contract of their base type (**LSP**): no strengthened preconditions, no weakened postconditions, no throwing for cases the base type allows.
- Consumers MUST NOT be forced to depend on members they do not use (**ISP**). Split a wide interface when a consumer only needs a subset.
- High-level policy MUST NOT depend on low-level detail; both MUST depend on abstractions (**DIP**). In practice: business rules depend on a boundary they own, and infrastructure implements that boundary.
- Interfaces MUST be introduced where there is a real seam: an external dependency, a testability boundary, or more than one implementation. See [clean-architecture.md](./clean-architecture.md) for where each boundary belongs.

## 3. The five principles

### 3.1 Single Responsibility Principle

**Definition:** a type has one reason to change, meaning one axis of responsibility.

```csharp
// One responsibility: evaluate task business rules.
public sealed class TaskValidator
{
    public ValidationResult Validate(TaskDraft draft) { /* rules only */ }
}

// Separate responsibility: persist.
public sealed class NpgsqlTaskRepository : ITaskRepository { /* SQL only */ }
```

**Common violations:** a controller that validates, maps, persists, and formats errors; a "service" that owns HTTP concerns and SQL; mapping logic duplicated across layers.

**Review checks:** can you name the one thing this type is responsible for? Does a business-rule change require editing a SQL file?

### 3.2 Open/Closed Principle

**Definition:** behavior is extended by adding code, not by editing working code.

```csharp
public interface ITaskStatusPolicy
{
    bool CanTransition(TaskStatus from, TaskStatus to);
}
```

**Common violations:** long `switch` statements over a type discriminator duplicated in several methods; editing a shared method and re-testing unrelated callers because there is no seam.

**Review checks:** when a new case appears, how many existing files must change? If the answer is one switch, that may be acceptable - record the decision.

### 3.3 Liskov Substitution Principle

**Definition:** a subtype can replace its base type without breaking callers.

```csharp
public interface ITaskRepository
{
    Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken);
}
```

A PostgreSQL implementation that throws `NotSupportedException` for `FindAsync` violates LSP. Return `null` for "not found", as the contract implies.

**Common violations:** implementations that throw for operations the interface implies are supported; fakes whose behavior differs from the production contract.

**Review checks:** does every implementation satisfy every member? Do tests of the abstraction pass against all implementations?

### 3.4 Interface Segregation Principle

**Definition:** no client is forced to depend on members it does not use.

```csharp
public interface ITaskReader
{
    Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskItem>> ListByOwnerAsync(Guid ownerId, CancellationToken cancellationToken);
}

public interface ITaskWriter
{
    Task<Guid> AddAsync(TaskDraft draft, CancellationToken cancellationToken);
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);
}
```

**Common violations:** a single `IRepository<T>` with twenty members where each consumer uses two; interfaces created only to satisfy a DI container.

**Review checks:** for each consumer, how many interface members does it actually call? Split when the ratio is poor and a real consumer need exists.

### 3.5 Dependency Inversion Principle

**Definition:** policy depends on an abstraction it owns; detail implements that abstraction.

```csharp
// Application owns the boundary.
namespace BallastLane.Application.Tasks;

public interface ITaskRepository
{
    Task<Guid> AddAsync(TaskDraft draft, CancellationToken cancellationToken);
}

// Infrastructure implements it; the dependency arrow points inward.
namespace BallastLane.Infrastructure.Persistence;

public sealed class NpgsqlTaskRepository : ITaskRepository { /* Npgsql + SQL */ }
```

**Common violations:** an application class referencing `NpgsqlConnection` or `HttpContext`; the interface living in the infrastructure project, which reverses the dependency.

**Review checks:** does the abstraction live with the consumer that needs it? Does the application project reference any infrastructure package?

## 4. Practical examples

A single create-task flow, showing where each principle appears.

```text
CreateTaskController  (SRP: HTTP translation only)
        |
        v
CreateTaskUseCase     (DIP: depends on ITaskRepository, an abstraction it owns)
        |
        +--> TaskValidator        (SRP: business rules only)
        +--> ITaskRepository      (ISP: read and write boundaries are separate)
                 ^
                 |
          NpgsqlTaskRepository    (LSP: honors the full contract; no NotSupportedException)
```

```csharp
// DIP + ISP: the boundary is owned by the application layer.
public interface ITaskWriter
{
    Task<Guid> AddAsync(TaskDraft draft, Guid ownerId, CancellationToken cancellationToken);
}

// SRP: the use case orchestrates; it does not validate, map, or persist itself.
public sealed class CreateTaskUseCase
{
    private readonly ITaskWriter _writer;
    private readonly TaskValidator _validator;

    public CreateTaskUseCase(ITaskWriter writer, TaskValidator validator)
    {
        _writer = writer;
        _validator = validator;
    }

    public async Task<Guid> ExecuteAsync(TaskDraft draft, Guid ownerId, CancellationToken cancellationToken)
    {
        var validation = _validator.Validate(draft);
        if (!validation.IsValid) throw new ValidationException(validation.Errors);
        return await _writer.AddAsync(draft, ownerId, cancellationToken);
    }
}
```

```csharp
// LSP: returns null for "not found" exactly as the contract states.
public sealed class NpgsqlTaskRepository : ITaskReader, ITaskWriter
{
    public async Task<TaskItem?> FindAsync(Guid id, CancellationToken cancellationToken) { /* ... */ }
    public async Task<Guid> AddAsync(TaskDraft draft, Guid ownerId, CancellationToken cancellationToken) { /* ... */ }
}
```

`OCP` appears only where variation is real. Here the status-transition rule is a likely seam; a hypothetical `ITaskValidatorFactory` with one implementation is not added.

## 5. Application to each concern

| Concern | How SOLID applies |
| --- | --- |
| Controllers | One responsibility: translate HTTP to an application call and back. Depend on application use cases, never on repositories or SQL directly. |
| Application use cases | One use case per operation, orchestrating domain rules and a boundary interface. Depend on abstractions, not infrastructure types. |
| Domain entities and rules | Framework-independent. No attributes or types from ASP.NET Core or Npgsql. Behavior belongs here, not in controllers. |
| Persistence adapters | Implement the boundary the application owns. One responsibility: map between the domain and the database. No business rules. |
| Tests | Depend on the same abstractions as production code. Each test type has one responsibility: assert one behavior. |

## 6. SOLID versus unnecessary abstraction

- You MUST NOT add an interface when there is one implementation and no testability or dependency-inversion need. Concretely: a repository boundary is justified because it inverts the database dependency; a `ITaskValidatorFactory` with one implementation is not.
- You MUST NOT add a generic repository, a unit-of-work wrapper, a mediator, or an extra layer without a named problem it solves. See [database-standards.md](./database-standards.md) for prohibited persistence libraries.
- You SHOULD prefer composition of small concrete types over deep inheritance.
- You SHOULD record the justification when you do add an abstraction, in the evidence record or a decision note.

## 7. Prohibited practices and anti-patterns

- Interfaces with a single implementation and no boundary purpose.
- "God" services that own HTTP, business rules, and SQL.
- Inheritance used for code reuse rather than substitutable behavior.
- Returning `null` in one implementation and throwing in another for the same contract case.
- Abstractions that expose infrastructure types (`NpgsqlConnection`, `HttpContext`) through their signatures.
- Applying SOLID mechanically until a small application has more interfaces than behaviors.

## 8. Verification and review criteria

| Check | Question |
| --- | --- |
| SRP | Can each type's single responsibility be named in one phrase? |
| OCP | Is extension possible without editing passing code, where variation exists? |
| LSP | Do all implementations honor the full contract? |
| ISP | Does each consumer use most of the interface it depends on? |
| DIP | Do business-rule projects reference only their own abstractions? |
| Justification | Is every abstraction tied to a seam, a test boundary, or a recorded decision? |
| Proportionality | Is the design the simplest that satisfies the behavior? |

## 9. References

- Layer boundaries and dependency direction: [clean-architecture.md](./clean-architecture.md)
- Coding conventions: [coding-standards.md](./coding-standards.md)
- Test design and doubles: [testing-and-tdd.md](./testing-and-tdd.md)
- Persistence boundary and prohibited libraries: [database-standards.md](./database-standards.md)
