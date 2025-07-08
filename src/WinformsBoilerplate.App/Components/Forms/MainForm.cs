using System.ComponentModel;
using WinformsBoilerplate.App.Shared.Forms;
using WinformsBoilerplate.Core.Abstractions.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Core.Extensions;

namespace WinformsBoilerplate.App.Components.Forms;

public partial class MainForm : StandardFormBase, IMainForm
{
    private readonly ILogService _logService;
    private readonly IEventStore _eventStore;
    private readonly ISystemService _systemService;
    private readonly ISessionStore _sessionStore;
    private readonly IAppSettingService _appSettingService;

    private bool _forceExit;
    private IEnumerable<IDisposable>? _adjustedCtrls;

    public MainForm(
        ILogService logService,
        IEventStore eventStore,
        ISystemService systemService,
        ISessionStore sessionStore,
        IAppSettingService appSettingService)
    {
        _logService = logService;
        _eventStore = eventStore;
        _systemService = systemService;
        _sessionStore = sessionStore;
        _appSettingService = appSettingService;

        InitializeComponent();

        _logService.BindLoggerToControl<MainForm>();
    }

    /// <inheritdoc cref="IMainForm.OnShutdownApplication" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<bool>? OnShutdownApplication { get; set; }

    /// <inheritdoc cref="IMainForm.OnSettingChanges" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action? OnSettingChanges { get; set; }

    /// <inheritdoc cref="IMainForm.OnLongTaskRun" />
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<bool>? OnLongTaskRun { get; set; }

    /// <inheritdoc cref="IComponentEvent.InitializeComponentEvents" />
    public override void InitializeComponentEvents()
    {
        _eventStore.Add<IMainForm>(
            new TeardownLogic(nameof(OnShutdownApplication), ShutdownApplication),
            new TeardownLogic(nameof(OnSettingChanges), SettingChanges),
            new TeardownLogic(nameof(OnLongTaskRun), LongTaskRun)
        );
    }

    /// <inheritdoc cref="IComponent.DisposeUnmanagedResources" />
    public override void DisposeUnmanagedResources()
    {
        if (_adjustedCtrls is not null)
        {
            _adjustedCtrls.ForEach((ctrl, _) => ctrl.Dispose());
            _adjustedCtrls = null;
        }

        // Cleanup stuffs before shutdown to prevent data leakage
        _eventStore.Dispose();
        _sessionStore.Dispose();
        _logService.Dispose();
        ntySysTray.Dispose();
    }

    private void ShutdownApplication(bool isRestart)
    {
        _forceExit = true;

        Close();

        if (isRestart)
        {
            _systemService.RestartApplication();

            return;
        }

        _systemService.ShutdownApplication();
    }

    private void SettingChanges()
    {
        // Implement logic to handle setting changes
    }

    private void LongTaskRun(bool state)
    {
        // Implement logic to handle long-running tasks
    }

    /// <inheritdoc cref="Form.OnClosing" />
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (_forceExit || !_appSettingService.Value.Misc.HideAppInNotificationArea)
        {
            return;
        }

        e.Cancel = true;

        DisplaySysTrayIconWithBalloon();
    }

    /// <inheritdoc cref="Form.OnLoad" />
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ntySysTray.Visible = _appSettingService.Value.Misc.AlwaysDisplayIconInNotificationArea;
    }

    /// <inheritdoc cref="Form.OnActivated" />
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        if (WindowState != FormWindowState.Minimized)
        {
            return;
        }

        if (_appSettingService.Value.Misc.HideAppInNotificationArea)
        {
            DisplaySysTrayIconWithBalloon();
        }
    }

    /// <summary>
    /// Displays the system tray icon and shows a balloon tip notification.
    /// </summary>
    /// <remarks>This method hides the main application window, makes the system tray icon visible,  and
    /// displays a balloon tip with a predefined message. The balloon tip is shown for  5 seconds and provides
    /// information about the application's continued operation.</remarks>
    private void DisplaySysTrayIconWithBalloon()
    {
        Hide();

        ntySysTray.Visible = true;
        ntySysTray.ShowBalloonTip(
            timeout: 5000,
            tipTitle: "Application is still running",
            tipText: "Application will continue to run so that you can...",
            tipIcon: ToolTipIcon.Info
        );
    }

    private void NtySysTray_DoubleClick(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Normal;
        ntySysTray.Visible = _appSettingService.Value.Misc.AlwaysDisplayIconInNotificationArea;

        Show();
        BringToFront();
    }
}
