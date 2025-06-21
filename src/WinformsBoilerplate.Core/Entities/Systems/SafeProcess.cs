using System.Diagnostics;

namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Provides a wrapper around the standard <see cref="Process"/> class with additional safety features.
/// </summary>
/// <remarks>
/// This class encapsulates a Process object and adds safety mechanisms to control
/// process creation and execution in a more controlled manner.
/// </remarks>
public class SafeProcess(SafeProcessStartInfo startInfo)
{
    /// <summary>
    /// Gets or sets the underlying Process object that this class wraps.
    /// </summary>
    /// <value>
    /// A <see cref="Process"/> instance initialized with the startup information
    /// provided when this SafeProcess was constructed.
    /// </value>
    public Process Process { get; set; } = new() { StartInfo = startInfo.Info };

    /// <summary>
    /// Gets a value indicating whether the process was successfully started.
    /// </summary>
    /// <value>
    /// <c>true</c> if the process was started successfully; otherwise, <c>false</c>.
    /// This value is only meaningful after <see cref="Start"/> has been called.
    /// </value>
    public bool CanStart { get; private set; }

    /// <summary>
    /// Starts the process using the configured startup information.
    /// </summary>
    /// <remarks>
    /// This method attempts to start the underlying process and updates the
    /// <see cref="CanStart"/> property with the result of the operation.
    /// </remarks>
    public void Start()
    {
        CanStart = Process.Start();
    }

    /// <summary>
    /// Starts the process and asynchronously waits for it to exit.
    /// </summary>
    /// <remarks>This method initiates the process by calling <see cref="Start"/> and then waits for the
    /// process to complete using <see cref="System.Diagnostics.Process.WaitForExitAsync"/>. It is intended for
    /// scenarios where asynchronous execution is required.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task completes when the process exits.</returns>
    public async Task StartAndWaitForExit()
    {
        Start();

        await Process.WaitForExitAsync();
    }
}
