using Core.Abstractions.Events;
using Core.Abstractions.Services;
using Core.Abstractions.Stores;
using Core.Extensions;
using Core.Wrappers;
using static Core.Constants.Common;

namespace App.Handlers;

/// <summary>
/// Handles application-wide exception management and logging.
/// </summary>
/// <remarks>
/// This handler provides centralized exception handling for:
/// <list type="bullet">
///     <item>Main thread exceptions</item>
///     <item>Unhandled exceptions across all threads</item>
///     <item>Fatal exceptions requiring application shutdown</item>
/// </list>
/// </remarks>
/// <param name="logService">The logging service for recording exception details.</param>
/// <param name="eventStore">The event store for dispatching exception-related events.</param>
public class ExceptionHandler(ILogService logService, IEventStore eventStore)
{
    /// <summary>
    /// Registers exception handlers for the application.
    /// </summary>
    /// <remarks>
    /// Sets up handlers for:
    /// <list type="bullet">
    ///     <item>Thread exceptions in the main UI thread</item>
    ///     <item>Unhandled exceptions across the application domain</item>
    /// </list>
    /// </remarks>
    public void Register()
    {
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
        ThrowableAction<Exception>
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
        ThrowableAction<Exception>
            .Run(() => HandleUnexpectedException(eventArgs.ExceptionObject as Exception))
            .Catch(HandleFatalException);
    }

    /// <summary>
    /// Processes unexpected exceptions in a controlled manner.
    /// </summary>
    /// <param name="ex">The exception to handle. If null, creates a new exception with default message.</param>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///     <item>Logs the exception details</item>
    ///     <item>Dispatches events for UI notification</item>
    ///     <item>Formats the stack trace for display</item>
    /// </list>
    /// </remarks>
    private void HandleUnexpectedException(Exception? ex)
    {
        ex ??= new(DefaultMessages.UNEXPECTED_ERROR);

        logService.WriteStackTrace(ex);
        eventStore.Dispatch((IGlobalExceptionDialogEvent x) => x.OnShowGlobalExceptionDialog, ex.ToFormattedStackTrace());
    }

    /// <summary>
    /// Handles fatal exceptions that require application termination.
    /// </summary>
    /// <param name="ex">The fatal exception that occurred.</param>
    /// <remarks>
    /// This method:
    /// <list type="bullet">
    ///     <item>Logs the fatal exception</item>
    ///     <item>Displays a fatal error message to the user</item>
    ///     <item>Dispatches application shutdown event</item>
    /// </list>
    /// </remarks>
    private void HandleFatalException(Exception ex)
    {
        logService.WriteStackTrace(ex);

        _ = MessageBox.Show(
            DefaultMessages.FATAL_ERROR,
            Application.ProductName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Stop
        );

        eventStore.Dispatch((IMainFormEvent x) => x.OnShutdownApplication, false);
    }
}
