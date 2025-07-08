using System.ComponentModel;

namespace WinformsBoilerplate.Core.Abstractions.Components.Forms;

/// <summary>
/// Represents the main form of the application.
/// </summary>
public interface IMainForm : IForm
{
    /// <summary>
    /// Occurs when the application is requested to shut down.
    /// The boolean parameter indicates whether to restart the application (true) or perform a graceful exit (false).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    Action<bool>? OnShutdownApplication { get; set; }

    /// <summary>
    /// Occurs when application settings have changed and dependent components should refresh.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    Action? OnSettingChanges { get; set; }

    /// <summary>
    /// Occurs when a long-running background task begins or ends.
    /// The boolean parameter indicates whether the task is starting (true) or stopping (false).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    Action<bool>? OnLongTaskRun { get; set; }
}
