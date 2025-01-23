using System.Reflection;

namespace App.Handlers;

/// <summary>
/// Provides functionality for handling assembly resolution and loading in the application domain.
/// </summary>
public static class AssemblyHandler
{
    /// <summary>
    /// Registers the assembly resolution handler for the current application domain.
    /// </summary>
    /// <remarks>
    /// This method should be called during application startup to ensure proper
    /// assembly resolution throughout the application's lifetime.
    /// </remarks>
    public static void ResolveCurrentDomainAssembly()
    {
        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadRequestingAssembly);
    }

    /// <summary>
    /// Handles assembly resolution requests by attempting to load the requested assembly.
    /// </summary>
    /// <param name="sender">The source of the assembly resolve event.</param>
    /// <param name="args">Event data containing assembly resolution details.</param>
    /// <returns>
    /// The loaded assembly if found and loaded successfully; otherwise, null.
    /// </returns>
    /// <remarks>
    /// The method attempts to resolve assemblies in the following order:
    /// <list type="number">
    ///     <item>Checks if the assembly is already loaded in the current domain</item>
    ///     <item>Attempts to load from the requesting assembly's directory as a DLL</item>
    ///     <item>Attempts to load from the requesting assembly's directory as an EXE</item>
    /// </list>
    /// If none of these attempts succeed, the method returns null, indicating the assembly could not be resolved.
    /// </remarks>
    public static Assembly? LoadRequestingAssembly(object? sender, ResolveEventArgs args)
    {
        // Check if assembly is already loaded
        Assembly? loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);

        if (loadedAssembly != null)
        {
            return loadedAssembly;
        }

        if (args.RequestingAssembly == null)
        {
            return null;
        }

        // Attempt to load from the requesting assembly's directory
        string folderPath = Path.GetDirectoryName(args.RequestingAssembly.Location) ?? string.Empty;
        string assemblyPath = Path.Combine(folderPath, $"{new AssemblyName(args.Name).Name}.dll");

        if (!File.Exists(assemblyPath))
        {
            // Try as executable if DLL is not found
            assemblyPath = Path.ChangeExtension(assemblyPath, ".exe");

            if (!File.Exists(assemblyPath))
            {
                return null;
            }
        }

        return Assembly.Load(assemblyPath);
    }
}
