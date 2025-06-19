using System.Reflection;

namespace WinformsBoilerplate.App;

/// <summary>
/// Provides a reference to the assembly containing the application.
/// </summary>
public static class AssemblyReference
{
    /// <summary>
    /// Gets the assembly containing the application.
    /// </summary>
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
