using Core.Abstractions.Services;
using Core.Constants;
using Core.Enums;
using Core.Helpers;
using Infrastructure.Extensions;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace Infrastructure.Services;

/// <summary>
/// Implements the ILogService interface to provide comprehensive logging functionality
/// for both UI controls and file-based logging operations.
/// </summary>
public class LogService : ILogService
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

    private bool _disposed;

    /// <inheritdoc/>
    public void BindControlLoggerContext<TContext>()
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
        string startupPath = PathHelpers.GetStartupPath(Application.StartupPath);

        string logFolder = Path.Combine(startupPath, Folders.LOG);
        _fileLogger = CreateLoggerConfiguration(false, Path.Combine(logFolder, "app-log-.txt")).CreateLogger();

        string stackTraceFolder = Path.Combine(logFolder, Folders.STACK_TRACE);
        _stackTraceLogger = CreateLoggerConfiguration(false, Path.Combine(stackTraceFolder, "app-stack-trace-.txt")).CreateLogger();

        string freezeFolder = Path.Combine(logFolder, Folders.FREEZE_LOG);
        _freezeLogger = CreateLoggerConfiguration(false, Path.Combine(freezeFolder, "app-freeze-log-.txt")).CreateLogger();
    }

    /// <inheritdoc/>
    public void WriteLog(LogEventLevel logLevel, string message)
    {
        ILogger? ctrlLoggerCtx = _ctrlLoggerCtx ?? _ctrlLogger;

        switch (logLevel)
        {
            case LogEventLevel.Information:
                ctrlLoggerCtx?.Information(message);
                _fileLogger?.Information(message);
                break;
            case LogEventLevel.Warning:
                ctrlLoggerCtx?.Warning(message);
                _fileLogger?.Warning(message);
                break;
            case LogEventLevel.Error:
                ctrlLoggerCtx?.Error(message);
                _fileLogger?.Error(message);
                break;
            case LogEventLevel.Debug:
                ctrlLoggerCtx?.Debug(message);
                _fileLogger?.Debug(message);
                break;
            case LogEventLevel.Fatal:
                ctrlLoggerCtx?.Fatal(message);
                _fileLogger?.Fatal(message);
                break;
            default:
                ctrlLoggerCtx?.Verbose(message);
                _fileLogger?.Verbose(message);
                break;
        }
    }

    /// <inheritdoc/>
    public void WriteStackTrace(Exception ex)
    {
        throw new NotImplementedException();
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

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }

        _ctrlLogger?.Dispose();
        _fileLogger?.Dispose();
        _stackTraceLogger?.Dispose();
        _freezeLogger?.Dispose();

        _disposed = true;
    }
}
