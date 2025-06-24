using Microsoft.Extensions.DependencyInjection;
using WinformsBoilerplate.Core.Abstractions.Components.Forms;
using WinformsBoilerplate.Core.Abstractions.Host;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Wrappers;
using static WinformsBoilerplate.Core.Constants.Common;

namespace WinformsBoilerplate.App.Handlers;

/// <summary>
/// Handles exceptions that occur within the application.
/// </summary>
public class ExceptionHandler : IHandler
{
    private ILogService? _logService;
    private IEventStore _eventStore;

    /// <inheritdoc />
    public void Invoke(IServiceProvider sp)
    {
        _logService = sp.GetRequiredService<ILogService>();
        _eventStore = sp.GetRequiredService<IEventStore>();

        Application.ThreadException += MainThreadExceptionHandler;

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
    }

    /// <summary>
    /// Handles exceptions occurring on the main application thread.
    /// </summary>
    /// <param name="sender">The source of the thread exception.</param>
    /// <param name="eventArgs">Contains the exception details.</param>
    /// <remarks>
    /// Attempts to handle the exception gracefully, falling back to fatal exception
    /// handling if the initial handling fails.
    /// </remarks>
    private void MainThreadExceptionHandler(object? sender, ThreadExceptionEventArgs eventArgs)
    {
        ThrowableAction
            .Run(() => HandleUnexpectedException(eventArgs.Exception))
            .Catch(HandleFatalException);
    }

    /// <summary>
    /// Handles unhandled exceptions from any thread in the application domain.
    /// </summary>
    /// <param name="sender">The source of the unhandled exception.</param>
    /// <param name="eventArgs">Contains the exception details and termination status.</param>
    /// <remarks>
    /// Processes exceptions that weren't caught by other exception handling mechanisms.
    /// Falls back to fatal exception handling if the initial handling fails.
    /// </remarks>
    private void UnhandledExceptionHandler(object? sender, UnhandledExceptionEventArgs eventArgs)
    {
        ThrowableAction
            .Run(() => HandleUnexpectedException(eventArgs.ExceptionObject as Exception))
            .Catch(HandleFatalException);
    }

    /// <summary>
    /// Processes unexpected exceptions in a controlled manner.
    /// </summary>
    /// <param name="ex">The exception to handle. If null, creates a new exception with default message.</param>
    private void HandleUnexpectedException(Exception? ex)
    {
        ex ??= new Exception(DefaultMessages.UNEXPECTED_ERROR);

        _logService?.WriteStackTraceLog(ex);
        //_eventStore?.Dispatch((IExceptionDialog x) => x.OnShowExceptionDialog, ex.ToFormattedStackTrace());
    }

    /// <summary>
    /// Handles fatal exceptions that require application termination.
    /// </summary>
    /// <param name="ex">The fatal exception that occurred.</param>
    private void HandleFatalException(Exception ex)
    {
        _logService?.WriteStackTraceLog(ex);

        _ = MessageBox.Show(
            DefaultMessages.FATAL_ERROR,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Stop
        );

        _eventStore?.Dispatch((IMainForm x) => x.OnShutdownApplication, false);
    }
}
