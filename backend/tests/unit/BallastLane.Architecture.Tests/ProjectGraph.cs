using System.Xml.Linq;

namespace BallastLane.Architecture.Tests;

/// <summary>
/// Reads the project files so tests assert the dependency declarations themselves. Assembly-level
/// references are not sufficient: the compiler omits one that no code in the assembly uses yet, which
/// would let a declared violation pass unnoticed.
/// </summary>
internal static class ProjectGraph
{
    private const string SolutionFileName = "BallastLane.sln";

    internal static string Root { get; } = FindRoot();

    internal static ProjectFile Load(string relativePath) =>
        new(relativePath, Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            $"The repository root was not found: no ancestor directory contains {SolutionFileName}.");
    }
}

internal sealed class ProjectFile
{
    private readonly XDocument _document;

    internal ProjectFile(string relativePath, string absolutePath)
    {
        RelativePath = relativePath;
        AbsolutePath = absolutePath;
        _document = XDocument.Load(absolutePath);
    }

    internal string RelativePath { get; }

    internal string AbsolutePath { get; }

    internal string AssemblyName =>
        Path.GetFileNameWithoutExtension(AbsolutePath);

    /// <summary>
    /// Project references declared by the file, normalized to repository-relative paths.
    /// </summary>
    internal IReadOnlyList<string> ProjectReferences =>
        _document
            .Descendants("ProjectReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .Where(value => value.Length > 0)
            .Select(Normalize)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

    internal IReadOnlyList<string> PackageReferences =>
        _document
            .Descendants("PackageReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .Where(value => value.Length > 0)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

    internal IReadOnlyList<string> FrameworkReferences =>
        _document
            .Descendants("FrameworkReference")
            .Select(element => (string?)element.Attribute("Include") ?? string.Empty)
            .Where(value => value.Length > 0)
            .ToArray();

    private string Normalize(string include)
    {
        var combined = Path.GetFullPath(
            Path.Combine(Path.GetDirectoryName(AbsolutePath)!, include.Replace('/', Path.DirectorySeparatorChar)));

        return Path
            .GetRelativePath(ProjectGraph.Root, combined)
            .Replace(Path.DirectorySeparatorChar, '/');
    }
}
