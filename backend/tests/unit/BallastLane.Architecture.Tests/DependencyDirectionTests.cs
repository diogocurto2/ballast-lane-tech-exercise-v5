namespace BallastLane.Architecture.Tests;

/// <summary>
/// Enforces the dependency direction the plan declares, so a layer cannot quietly acquire a project,
/// package, or framework reference it must not have.
/// </summary>
public sealed class DependencyDirectionTests
{
    private const string DomainProject = "backend/src/Domain/BallastLane.Domain.csproj";
    private const string ApplicationProject = "backend/src/Application/BallastLane.Application.csproj";
    private const string InfrastructureProject = "backend/src/Infrastructure/BallastLane.Infrastructure.csproj";
    private const string SharedWebProject = "backend/src/SharedWeb/BallastLane.SharedWeb.csproj";
    private const string UserApiProject = "backend/src/UserApi/BallastLane.UserApi.csproj";
    private const string TaskApiProject = "backend/src/TaskApi/BallastLane.TaskApi.csproj";
    private const string MigratorProject = "backend/src/Migrator/BallastLane.Migrator.csproj";

    private static readonly string[] ProhibitedPackageFragments =
    [
        "EntityFrameworkCore",
        "EntityFramework",
        "Dapper",
        "MediatR",
        "Mediator",
    ];

    private static IReadOnlyList<ProjectFile> AllProjects { get; } =
    [
        ProjectGraph.Load(DomainProject),
        ProjectGraph.Load(ApplicationProject),
        ProjectGraph.Load(InfrastructureProject),
        ProjectGraph.Load(SharedWebProject),
        ProjectGraph.Load(UserApiProject),
        ProjectGraph.Load(TaskApiProject),
        ProjectGraph.Load(MigratorProject),
    ];

    [Fact]
    public void Domain_declares_no_project_reference()
    {
        Assert.Empty(ProjectGraph.Load(DomainProject).ProjectReferences);
    }

    [Fact]
    public void Application_declares_only_Domain()
    {
        Assert.Equal([DomainProject], ProjectGraph.Load(ApplicationProject).ProjectReferences);
    }

    [Fact]
    public void Infrastructure_declares_Application_and_Domain()
    {
        Assert.Equal(
            [ApplicationProject, DomainProject],
            ProjectGraph.Load(InfrastructureProject).ProjectReferences);
    }

    [Fact]
    public void SharedWeb_declares_Application_and_Domain_only()
    {
        Assert.Equal(
            [ApplicationProject, DomainProject],
            ProjectGraph.Load(SharedWebProject).ProjectReferences);
    }

    [Fact]
    public void Infrastructure_is_declared_by_the_three_composition_roots_only()
    {
        var referencing = AllProjects
            .Where(project => project.ProjectReferences.Contains(InfrastructureProject, StringComparer.Ordinal))
            .Select(project => project.RelativePath)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [MigratorProject, TaskApiProject, UserApiProject],
            referencing);

        Assert.DoesNotContain(
            InfrastructureProject,
            ProjectGraph.Load(ApplicationProject).ProjectReferences);
        Assert.DoesNotContain(
            InfrastructureProject,
            ProjectGraph.Load(DomainProject).ProjectReferences);
    }

    [Fact]
    public void Domain_and_Application_declare_no_web_or_database_package()
    {
        foreach (var project in new[] { ProjectGraph.Load(DomainProject), ProjectGraph.Load(ApplicationProject) })
        {
            Assert.Empty(project.PackageReferences);
            Assert.Empty(project.FrameworkReferences);
        }
    }

    [Fact]
    public void Domain_and_Application_do_not_reference_web_or_database_assemblies()
    {
        var assemblies = new[] { LayerAssemblies.Domain, LayerAssemblies.Application };

        foreach (var assembly in assemblies)
        {
            var forbidden = assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name ?? string.Empty)
                .Where(name =>
                    name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
                    || name.StartsWith("Npgsql", StringComparison.Ordinal))
                .ToArray();

            Assert.True(
                forbidden.Length == 0,
                $"{assembly.GetName().Name} must not reference: {string.Join(", ", forbidden)}");
        }
    }

    [Fact]
    public void No_project_declares_a_prohibited_library()
    {
        var offenders = AllProjects
            .SelectMany(project => project.PackageReferences
                .Where(package => ProhibitedPackageFragments.Any(fragment =>
                    package.Contains(fragment, StringComparison.OrdinalIgnoreCase)))
                .Select(package => $"{project.RelativePath} -> {package}"))
            .ToArray();

        Assert.True(offenders.Length == 0, $"Prohibited libraries declared: {string.Join(", ", offenders)}");
    }

    [Fact]
    public void No_assembly_references_a_prohibited_library()
    {
        var assemblies = new[]
        {
            LayerAssemblies.Domain,
            LayerAssemblies.Application,
            LayerAssemblies.Infrastructure,
            LayerAssemblies.SharedWeb,
            LayerAssemblies.UserApi,
            LayerAssemblies.TaskApi,
            LayerAssemblies.Migrator,
        };

        var offenders = assemblies
            .SelectMany(assembly => assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name ?? string.Empty)
                .Where(name => ProhibitedPackageFragments.Any(fragment =>
                    name.Contains(fragment, StringComparison.OrdinalIgnoreCase)))
                .Select(name => $"{assembly.GetName().Name} -> {name}"))
            .ToArray();

        Assert.True(offenders.Length == 0, $"Prohibited libraries referenced: {string.Join(", ", offenders)}");
    }
}
