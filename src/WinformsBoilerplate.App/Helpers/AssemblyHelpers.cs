using System.Diagnostics;
using System.Reflection;

namespace WinformsBoilerplate.App.Helpers;

public static class AssemblyHelpers
{
    /// <summary>
    /// Validates that required library DLLs exist and meet the application's minimum version requirement.
    /// </summary>
    /// <param name="requiredVersion">The minimum version expected for the libraries.</param>
    /// <returns><c>true</c> if all libraries are found and meet the version requirement; otherwise, <c>false</c>.</returns>
    public static bool ValidateLibVersions(out Version requiredVersion)
    {
        Version productVersion = new(Application.ProductVersion);
        requiredVersion = productVersion;

        // Define fallback strategies in order of preference
        var pathScopes = new List<Func<string>>
        {
            () => $@"{Application.StartupPath}\lib\{Application.ProductName}.Core.dll",
            () => $@"{Application.StartupPath}\lib\{Application.ProductName}.Infrastructure.dll",

            () => $@"{Application.StartupPath}\{Application.ProductName}.Core.dll",
            () => $@"{Application.StartupPath}\{Application.ProductName}.Infrastructure.dll",

            () => $@"{Application.StartupPath}\Core.dll",
            () => $@"{Application.StartupPath}\Infrastructure.dll"
        };

        // Group every two items into one strategy
        var fallbackScopes = pathScopes
            .Select((getter, index) => new { Index = index, Getter = getter })
            .GroupBy(x => x.Index / 2)
            .Select(g => g
                .Select(x => x.Getter())
                .Where(x => File.Exists(x))
                .ToList())
            .ToList();

        foreach (List<string>? libPaths in fallbackScopes)
        {
            if (libPaths.Count < 2)
            {
                continue;
            }

            IEnumerable<Version> versions = libPaths.Select(path => {
                string? fileVersion = FileVersionInfo.GetVersionInfo(path).FileVersion;
                return Version.TryParse(fileVersion, out Version? parsed) ? parsed : new Version(1, 0, 0, 0);
            });

            // Check if all versions meet or exceed productVersion
            if (versions.All(v => v >= productVersion))
            {
                return true;
            }
        }

        return false;
    }

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
    private static Assembly? LoadRequestingAssembly(object? sender, ResolveEventArgs args)
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
