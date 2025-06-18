namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines a contract for components that manage unmanaged resources,
/// ensuring consistent handling and disposal patterns.
/// </summary>
/// <remarks>
/// Implement this interface when your component interacts with unmanaged resources,
/// such as operating system handles, native libraries, or unmanaged memory.
/// Proper implementation ensures resource safety and prevents memory leaks.
/// </remarks>
public interface IComponent : IDisposable
{
    /// <summary>
    /// Gets the unmanaged handle or pointer associated with this component.
    /// </summary>
    /// <remarks>
    /// This handle typically represents a resource allocated externally,
    /// such as an OS handle or a pointer from unmanaged code.
    /// </remarks>
    IntPtr Handle { get; }

    /// <summary>
    /// Releases unmanaged resources explicitly.
    /// </summary>
    /// <remarks>
    /// Call this method explicitly when unmanaged resources should be released independently
    /// of the managed disposal logic (e.g., when a custom release procedure is needed).
    /// </remarks>
    void DisposeUnmanagedResources();
}
