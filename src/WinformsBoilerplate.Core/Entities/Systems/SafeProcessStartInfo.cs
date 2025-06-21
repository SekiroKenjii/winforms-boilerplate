using System.Diagnostics;

namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Provides a safer wrapper around the standard <see cref="ProcessStartInfo"/> class with additional validation.
/// </summary>
/// <remarks>
/// This class encapsulates a ProcessStartInfo object and adds safety checks when setting
/// properties that could lead to runtime errors if improperly configured.
/// </remarks>
public class SafeProcessStartInfo
{
    /// <summary>
    /// Gets or sets the underlying ProcessStartInfo object that this class wraps.
    /// </summary>
    /// <value>
    /// A <see cref="ProcessStartInfo"/> instance that contains the configuration for starting a process.
    /// </value>
    public ProcessStartInfo Info { get; set; } = new();

    /// <summary>
    /// Sets the working directory for the process with validation to ensure the directory exists.
    /// </summary>
    /// <param name="dir">The directory path to set as the working directory.</param>
    /// <returns>
    /// <c>true</c> if the directory exists and was successfully set as the working directory;
    /// <c>false</c> if the directory does not exist and the working directory was not changed.
    /// </returns>
    /// <remarks>
    /// This method performs a safety check to verify that the specified directory exists
    /// before setting it as the working directory, preventing potential runtime errors.
    /// </remarks>
    public bool SetWorkingDir(string dir)
    {
        if (!Directory.Exists(dir))
        {
            return false;
        }

        Info.WorkingDirectory = dir;

        return true;
    }
}
