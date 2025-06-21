using System.Collections.Concurrent;
using System.Diagnostics;
using WinformsBoilerplate.Core.Entities.Systems;

namespace WinformsBoilerplate.Core.Abstractions.Services;

/// <summary>
/// Defines a contract for system-level services within the application.
/// </summary>
public interface ISystemService : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the current user has administrator privileges.
    /// </summary>
    bool IsAdministrator { get; }

    /// <summary>
    /// Performs a comprehensive system check to verify the operational status of the system.
    /// </summary>
    /// <remarks>This method checks various system components to ensure they are functioning correctly.  It
    /// does not return a value, but may log diagnostic information or raise exceptions  if issues are detected. Callers
    /// should ensure the system is in a stable state before  invoking this method.</remarks>
    void PerformSystemCheck();

    /// <summary>
    /// Executes a process with the specified start information, priority, and arguments.
    /// </summary>
    /// <remarks>This method provides a safe way to start and manage processes, ensuring proper resource
    /// handling and error reporting. The <see cref="SafeProcess"/> object returned allows for monitoring the process
    /// state, retrieving output, and handling termination.</remarks>
    /// <param name="startInfo">The start information for the process, including file path, arguments, and other settings.</param>
    /// <param name="priority">The optional priority class to assign to the process. If <see langword="null"/>, the default priority is used.</param>
    /// <param name="args">Additional arguments to pass to the process. These are appended to the arguments specified in <paramref
    /// name="startInfo"/>.</param>
    /// <returns>A <see cref="SafeProcess"/> instance representing the executed process. The caller can use this object to
    /// monitor and interact with the process.</returns>
    SafeProcess ExecuteProcess(SafeProcessStartInfo startInfo, ProcessPriorityClass? priority = null, params ReadOnlySpan<string> args);

    /// <summary>
    /// Monitors the specified file for changes and triggers the provided event handler when a change occurs.
    /// </summary>
    /// <remarks>If <paramref name="override"/> is <see langword="false"/> and the file is already being
    /// monitored,  the method will not replace the existing monitoring configuration.</remarks>
    /// <param name="filePath">The full path of the file to monitor. Must not be null or empty.</param>
    /// <param name="next">The event handler to invoke when a change is detected. Cannot be null.</param>
    /// <param name="override">A value indicating whether to override any existing file monitoring for the specified file. <see
    /// langword="true"/> to override; otherwise, <see langword="false"/>.</param>
    void WatchFileChanged(string filePath, FileSystemEventHandler next, bool @override = false);

    /// <summary>
    /// Copies a file from the specified source path to the specified destination path.
    /// </summary>
    /// <param name="sourceFilePath">The full path of the source file to be copied. Cannot be null or empty.</param>
    /// <param name="destinationFilePath">The full path where the file should be copied to. Cannot be null or empty.</param>
    /// <param name="override">A value indicating whether to overwrite the destination file if it already exists.  <see langword="true"/> to
    /// overwrite; otherwise, <see langword="false"/>.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result contains the full path  of the
    /// copied file if the operation succeeds; otherwise, <see langword="null"/>.</returns>
    Task<string?> CopyFile(string sourceFilePath, string destinationFilePath, bool @override = false);

    /// <summary>
    /// Saves the specified store object to the local storage.
    /// </summary>
    /// <remarks>This method persists the provided store object to the local storage. Ensure that the object 
    /// passed is serializable and valid for storage. If the operation fails, an exception may be thrown.</remarks>
    /// <param name="store">The store object to be saved. Cannot be null.</param>
    void SaveLocalStore(object store);

    /// <summary>
    /// Retrieves the current state of the local store as a thread-safe dictionary.
    /// </summary>
    /// <remarks>The local store is represented as a <see cref="ConcurrentDictionary{TKey, TValue}"/> where
    /// keys are strings and values are nullable objects. This method provides a snapshot of the store's contents,
    /// allowing safe concurrent access and manipulation.</remarks>
    /// <returns>A <see cref="ConcurrentDictionary{string, object?}"/> containing the key-value pairs stored in the local store.
    /// The dictionary may be empty if the store has no entries.</returns>
    ConcurrentDictionary<string, object?> ReadLocalStore();

    /// <summary>
    /// Cleans up the local data store by removing obsolete or temporary files.
    /// </summary>
    /// <remarks>This method is typically used to free up disk space and ensure the local store remains in a
    /// consistent state.  It should be called periodically or when the application determines that cleanup is
    /// necessary.</remarks>
    void CleanupLocalStore();
}
