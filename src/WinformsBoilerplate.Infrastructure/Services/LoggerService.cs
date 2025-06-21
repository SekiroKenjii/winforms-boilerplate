using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using System.Runtime.CompilerServices;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Enums;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Infrastructure.Configurations.Logger;

namespace WinformsBoilerplate.Infrastructure.Services;

public class LoggerService : Disposable, ILogService
{
    /// <summary>
    /// Default template format for standard log messages.
    /// </summary>
    private const string TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    /// <summary>
    /// Template format for CSV-style log messages.
    /// </summary>
    private const string CSV_TEMPLATE = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz},{Message:lj}{NewLine}";

    /// <summary>
    /// Logger instance with context for control/UI logging.
    /// </summary>
    private ILogger? _ctrlLoggerCtx;

    /// <summary>
    /// Main control logger instance.
    /// </summary>
    private Logger? _ctrlLogger;

    /// <summary>
    /// Main file logger instance for general application logging.
    /// </summary>
    private Logger? _fileLogger;

    /// <summary>
    /// Specialized logger instance for stack trace logging.
    /// </summary>
    private Logger? _stackTraceLogger;

    /// <summary>
    /// Specialized logger instance for application freeze events.
    /// </summary>
    private Logger? _freezeLogger;

    /// <inheritdoc/>
    public void BindLoggerToControl<TContext>(string controlName, Logger? logger)
    {
        _ctrlLoggerCtx = _ctrlLogger?.ForContext<TContext>();
    }

    /// <inheritdoc/>
    public void CreateControlLogger()
    {
        _ctrlLogger =
            CreateLoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteToDataGridViewControl()
            .WriteToRichTextBoxControl(new MessageTemplateTextFormatter(TEMPLATE))
            .CreateLogger();
    }

    /// <inheritdoc/>
    public void CreateFileLoggers()
    {
        string startupPath = CommonHelpers.AppStartupPath();

        string logFolder = Path.Combine(startupPath, Folders.LOG);
        _fileLogger = CreateLoggerConfiguration(false, Path.Combine(logFolder, "app-log-.txt")).CreateLogger();

        string stackTraceFolder = Path.Combine(logFolder, Folders.STACK_TRACE);
        _stackTraceLogger = CreateLoggerConfiguration(false, Path.Combine(stackTraceFolder, "app-stack-trace-.txt")).CreateLogger();

        string freezeFolder = Path.Combine(logFolder, Folders.FREEZE_LOG);
        _freezeLogger = CreateLoggerConfiguration(false, Path.Combine(freezeFolder, "app-freeze-log-.txt")).CreateLogger();
    }

    /// <inheritdoc/>
    public void Debug(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Debug(_msg);
        _fileLogger?.Debug(_msg);
    }

    /// <inheritdoc/>
    public void Error(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Error(_msg);
        _fileLogger?.Error(_msg);
    }

    /// <inheritdoc/>
    public void Fatal(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Fatal(_msg);
        _fileLogger?.Fatal(_msg);
    }

    /// <inheritdoc/>
    public Logger? GetLogger(LoggerType loggerType)
    {
        return loggerType switch {
            LoggerType.Control => _ctrlLogger,
            LoggerType.File => _fileLogger,
            LoggerType.StackTrace => _stackTraceLogger,
            LoggerType.Freezes => _freezeLogger,
            _ => null
        };
    }

    /// <inheritdoc/>
    public void Info(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Information(_msg);
        _fileLogger?.Information(_msg);
    }

    /// <inheritdoc/>
    public void Verbose(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Verbose(_msg);
        _fileLogger?.Verbose(_msg);
    }

    /// <inheritdoc/>
    public void Warn(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown")
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;
        string _msg = GetMessage(message, line, caller);

        ctrlLoggerCtx?.Warning(_msg);
        _fileLogger?.Warning(_msg);
    }

    /// <inheritdoc/>
    public void WriteLog(LogEventLevel logLevel, string message)
    {
        switch (logLevel)
        {
            case LogEventLevel.Information:
                Info(message);
                break;
            case LogEventLevel.Warning:
                Warn(message);
                break;
            case LogEventLevel.Error:
                Error(message);
                break;
            case LogEventLevel.Debug:
                Debug(message);
                break;
            case LogEventLevel.Fatal:
                Fatal(message);
                break;
            default:
                Verbose(message);
                break;
        }
    }

    /// <inheritdoc/>
    public void WriteStackTraceLog(Exception ex)
    {
        throw new NotImplementedException();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        _ctrlLogger?.Dispose();
        _fileLogger?.Dispose();
        _stackTraceLogger?.Dispose();
        _freezeLogger?.Dispose();
        _ctrlLoggerCtx = null;
    }

    /// <summary>
    /// Creates a base logger configuration with specified parameters.
    /// </summary>
    /// <param name="infinite">If true, configures for infinite rolling interval.</param>
    /// <param name="path">Optional file path for file-based logging.</param>
    /// <returns>A LoggerConfiguration instance.</returns>
    private static LoggerConfiguration CreateLoggerConfiguration(bool infinite = false, string? path = null)
    {
        LoggerConfiguration logConfig = new LoggerConfiguration().MinimumLevel.Verbose();
        RollingInterval rollingInterval = infinite ? RollingInterval.Infinite : RollingInterval.Day;
        MessageTemplateTextFormatter formatter = infinite ? new(CSV_TEMPLATE) : new(TEMPLATE);

        return !string.IsNullOrEmpty(path)
            ? logConfig.WriteTo.File(
                path: path,
                rollingInterval: rollingInterval,
                rollOnFileSizeLimit: true,
                formatter: formatter
            )
            : logConfig;
    }

    /// <summary>
    /// Formats a message string with caller information and line number.
    /// </summary>
    /// <param name="message">The message to format. Cannot be null or empty.</param>
    /// <param name="line">The line number associated with the message.</param>
    /// <param name="caller">The name of the calling method or source. Cannot be null or empty.</param>
    /// <returns>A formatted string that includes the caller name, line number, and message.</returns>
    private static string GetMessage(string message, int line, string caller) => $"[{caller}] [{line}] {message}";
}
