using System.Diagnostics;

namespace Core.Abstractions.Services;

/// <summary>
/// Provides system-level operations for process execution and file management.
/// </summary>
public interface ISystemService
{
    /// <summary>
    /// Executes a process with the specified file name and optional priority.
    /// </summary>
    /// <param name="fileName">The name or path of the file to execute.</param>
    /// <param name="priority">Optional process priority class to set for the executed process.</param>
    /// <param name="args">Command-line arguments to pass to the process.</param>
    /// <returns>A Process object representing the started process.</returns>
    Process ExecuteProcess(string fileName, ProcessPriorityClass? priority = null, params string[] args);

    /// <summary>
    /// Monitors a file for changes.
    /// </summary>
    /// <param name="filePath">The full path of the file to watch for changes.</param>
    void WatchFileChanged(string filePath);

    /// <summary>
    /// Moves a file from the source location to the destination location.
    /// </summary>
    /// <param name="sourceFilePath">The path of the file to move.</param>
    /// <param name="destinationFilePath">The destination path where the file should be moved to.</param>
    /// <param name="override">If true, overwrites the destination file if it exists.</param>
    /// <returns>The path of the moved file, or null if the operation fails.</returns>
    string? MoveFile(string sourceFilePath, string destinationFilePath, bool @override = false);
}
