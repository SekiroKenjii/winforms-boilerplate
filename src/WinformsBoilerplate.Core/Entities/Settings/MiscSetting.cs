namespace WinformsBoilerplate.Core.Entities.Settings;

/// <summary>
/// Represents miscellaneous application settings related to application behavior, window visibility, or notification area
/// preferences, etc.
/// </summary>
public class MiscSetting
{
    /// <summary>
    /// Gets or sets a value indicating whether the application should start automatically when the operating system
    /// boots.
    /// </summary>
    public bool StartAppOnOSBoot { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application window should be displayed automatically after the
    /// operating system has booted.
    /// </summary>
    public bool ShowWindowAfterOSBooted { get; set; }

    /// <summary>
    /// If enabled, the main window will be hidden as soon as user switch to another window.
    /// </summary>
    public bool AlwaysHideWindow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application icon should always be displayed in the notification
    /// area.
    /// </summary>
    public bool AlwaysDisplayIconInNotificationArea { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the application should remain in the notification area when closed or
    /// minimized.
    /// </summary>
    public bool HideAppInNotificationArea { get; set; }
}
