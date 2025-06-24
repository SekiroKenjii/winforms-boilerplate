using WinformsBoilerplate.Core.Entities.Contracts;

namespace WinformsBoilerplate.Core.Entities.Settings;

/// <summary>
/// Represents miscellaneous application settings related to application behavior, window visibility, or notification area
/// preferences, etc.
/// </summary>
public class MiscSettings : ObservableEntityBase
{
    private bool _startAppOnOSBoot;
    private bool _showWindowAfterOSBooted;
    private bool _alwaysHideWindow;
    private bool _alwaysDisplayIconInNotificationArea;
    private bool _hideAppInNotificationArea;

    /// <summary>
    /// Gets or sets a value indicating whether the application should start automatically when the operating system
    /// boots.
    /// </summary>
    public bool StartAppOnOSBoot
    {
        get => _startAppOnOSBoot;
        set => SetProperty(field: ref _startAppOnOSBoot, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application window should be displayed automatically after the
    /// operating system has booted.
    /// </summary>
    public bool ShowWindowAfterOSBooted
    {
        get => _showWindowAfterOSBooted;
        set => SetProperty(ref _showWindowAfterOSBooted, value);
    }

    /// <summary>
    /// If enabled, the main window will be hidden as soon as user switch to another window.
    /// </summary>
    public bool AlwaysHideWindow
    {
        get => _alwaysHideWindow;
        set => SetProperty(ref _alwaysHideWindow, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application icon should always be displayed in the notification
    /// area.
    /// </summary>
    public bool AlwaysDisplayIconInNotificationArea
    {
        get => _alwaysDisplayIconInNotificationArea;
        set => SetProperty(ref _alwaysDisplayIconInNotificationArea, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application should remain in the notification area when closed or
    /// minimized.
    /// </summary>
    public bool HideAppInNotificationArea
    {
        get => _hideAppInNotificationArea;
        set => SetProperty(ref _hideAppInNotificationArea, value);
    }
}
