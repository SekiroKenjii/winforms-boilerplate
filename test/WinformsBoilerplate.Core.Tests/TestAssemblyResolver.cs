using System.Reflection;
using System.Runtime.Loader;

namespace WinformsBoilerplate.Core.Tests;

/// <summary>
/// Module initializer to set up assembly resolution for test projects.
/// </summary>
internal static class ModuleInitializer
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void Initialize()
    {
        // Set up assembly resolver before doing anything else
        AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        AssemblyLoadContext.Default.Resolving += OnAssemblyResolving;
    }

    private static Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
    {
        return TryResolveAssembly(args.Name);
    }

    private static Assembly? OnAssemblyResolving(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        return TryResolveAssembly(assemblyName.FullName);
    }

    private static Assembly? TryResolveAssembly(string assemblyName)
    {
        try
        {
            // Handle Microsoft.Extensions assemblies specifically
            if (assemblyName.StartsWith("Microsoft.Extensions.", StringComparison.OrdinalIgnoreCase))
            {
                var simpleName = assemblyName.Split(',')[0];

                // Try to find the assembly in the current directory
                var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (currentDir != null)
                {
                    var assemblyPath = Path.Combine(currentDir, simpleName + ".dll");
                    if (File.Exists(assemblyPath))
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                }

                // Try to load from the already loaded assemblies
                var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name?.Equals(simpleName, StringComparison.OrdinalIgnoreCase) == true);

                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }
            }
        }
        catch
        {
            // Ignore resolution errors
        }

        return null;
    }
}
