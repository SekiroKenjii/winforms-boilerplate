using App.Shared.Forms;
using Core.Abstractions.Services;
using Core.Abstractions.Stores;
using Core.Enums;
using Serilog.Events;

namespace App.Forms;

public partial class MainForm : StandardBaseForm
{
    private readonly IEventStore _eventStore;
    private readonly ILogService _logService;

    private bool _forceExit;

    public MainForm(IEventStore eventStore, ILogService logService)
    {
        _eventStore = eventStore;
        _logService = logService;

        InitializeComponent();

        _logService.BindControlLoggerContext<MainForm>();

        InitializeFormEvents();

        _logService.WriteLog(LogEventLevel.Information, "Application initialized successfully.");
    }

    private void ShutdownApplication(bool isRestart)
    {
        _forceExit = true;

        _logService.GetLogger(LoggerType.File)?.Information("Application is shutting down...");

        _eventStore.Flush();
        _logService.Dispose();
        ntySysTray.Dispose();

        if (isRestart)
        {
            Application.Restart();
        }

        Environment.Exit(0);
    }

    private void TsmiExit_Click(object sender, EventArgs e)
    {
        ShutdownApplication(false);
    }

    private void TsmiSysTrayExit_Click(object sender, EventArgs e)
    {
        ShutdownApplication(false);
    }

    private void MainForm_Shown(object sender, EventArgs e)
    {
        if (WindowState != FormWindowState.Minimized)
        {
            return;
        }

        ShowSysTrayIcon();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_forceExit)
        {
            return;
        }

        e.Cancel = true;
        ShowSysTrayIcon();
    }

    private void ShowSysTrayIcon()
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
        Show();
        WindowState = FormWindowState.Normal;
        ntySysTray.Visible = false;
    }
}
