using System.ComponentModel;
using WinformsBoilerplate.App.Shared.Forms;
using WinformsBoilerplate.Core.Abstractions.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Entities.Systems;

namespace WinformsBoilerplate.App.Components.Forms;

public partial class MainForm : BaseStandardForm, IMainForm
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<bool>? OnShutdownApplication { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action? OnSettingChanges { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Action<bool>? OnLongTaskRun { get; set; }

    private readonly ILogService _logService;
    private readonly IEventStore _eventStore;
    private readonly ISystemService _systemService;
    private readonly AppSettings _appSettings;

    private bool _forceExit;
    private IEnumerable<IDisposable>? _adjustedCtrls;

    public MainForm(
        ILogService logService,
        IEventStore eventStore,
        ISystemService systemService,
        AppSettings appSettings)
    {
        _logService = logService;
        _eventStore = eventStore;
        _systemService = systemService;
        _appSettings = appSettings;

        InitializeComponent();

        _logService.BindLoggerToControl<MainForm>();
    }

    /// <inheritdoc cref="IComponentEvent.InitializeFormEvents" />
    public override void InitializeFormEvents()
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
            foreach (IDisposable ctrl in _adjustedCtrls)
            {
                ctrl.Dispose();
            }

            _adjustedCtrls = null;
        }

        _eventStore.Flush();
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

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_forceExit || !_appSettings.Misc.HideAppInNotificationArea)
        {
            return;
        }

        e.Cancel = true;

        DisplaySysTrayIconWithBalloon();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        ntySysTray.Visible = _appSettings.Misc.AlwaysDisplayIconInNotificationArea;
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        if (WindowState != FormWindowState.Minimized)
        {
            return;
        }

        if (_appSettings.Misc.HideAppInNotificationArea)
        {
            DisplaySysTrayIconWithBalloon();
        }
    }

    private void DisplaySysTrayIconWithBalloon()
    {
        Hide();
        ntySysTray.Visible = true;
        ntySysTray.ShowBalloonTip(
            timeout: 5000,
            tipTitle: "Merge Engine is still running",
            tipText: "Merge Engine will continue to run so that you can...",
            tipIcon: ToolTipIcon.Info
        );
    }
}
