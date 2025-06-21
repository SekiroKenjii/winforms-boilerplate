using Serilog.Core;
using Serilog.Events;
using System.Runtime.CompilerServices;
using WinformsBoilerplate.Core.Enums;

namespace WinformsBoilerplate.Core.Abstractions.Services;

/// <summary>
/// Provides functionality for logging messages, managing loggers, and binding loggers to controls.
/// </summary>
/// <remarks>
/// This interface defines methods for creating and retrieving loggers, writing log messages at various
/// levels, and handling exceptions through stack trace logging. It supports binding loggers to UI controls and provides
/// contextual information such as caller member names and line numbers for enhanced debugging.
/// </remarks>
public interface ILogService : IDisposable
{
    /// <summary>
    /// Retrieves a logger instance based on the specified logger type.
    /// </summary>
    /// <param name="loggerType">The type of logger to retrieve. Must be a valid <see cref="LoggerType"/> value.</param>
    /// <returns>A <see cref="Logger"/> instance corresponding to the specified <paramref name="loggerType"/>,  or <see
    /// langword="null"/> if no logger of the specified type is available.</returns>
    Logger? GetLogger(LoggerType loggerType);

    /// <summary>
    /// Creates and initializes the control logger with appropriate sinks and configuration.
    /// </summary>
    /// <remarks>
    /// This method must be called before attempting to use the control logger.
    /// The control logger is typically used for UI/control-related logging operations
    /// and may include specific sinks for UI display.
    /// </remarks>
    void CreateControlLogger();

    /// <summary>
    /// Creates and initializes multiple file loggers with appropriate sinks and configurations.
    /// </summary>
    /// <remarks>
    /// This method sets up loggers that write log messages to files. It is typically used to enable 
    /// persistent logging for diagnostic or auditing purposes. Ensure that the necessary file system  permissions are
    /// granted before calling this method.
    /// </remarks>
    void CreateFileLoggers();

    /// <summary>
    /// Binds the specified logger to a control identified by its name within the given context.
    /// </summary>
    /// <remarks>
    /// This method associates a logger with a control, enabling logging for operations or events
    /// related to the control. If <paramref name="logger"/> is null, no logging will occur for the specified
    /// control.
    /// </remarks>
    /// <typeparam name="TContext">The type of the context in which the control resides.</typeparam>
    /// <param name="controlName">The name of the control to bind the logger to. Cannot be null or empty.</param>
    /// <param name="logger">The logger instance to bind to the control. If null, logging will be disabled for the control.</param>
    void BindLoggerToControl<TContext>(string controlName, Logger? logger);

    /// <summary>
    /// Writes a log entry with the specified log level and message.
    /// </summary>
    /// <remarks>
    /// Use this method to record diagnostic or informational messages at a specific log level. 
    /// Ensure that the <paramref name="message"/> parameter is meaningful and concise to aid in debugging or
    /// monitoring.
    /// </remarks>
    /// <param name="logLevel">The severity level of the log entry. This determines how the log entry is categorized.</param>
    /// <param name="message">The message to be logged. Cannot be null or empty.</param>
    void WriteLog(LogEventLevel logLevel, string message);

    /// <summary>
    /// Logs an informational message along with optional caller details.
    /// </summary>
    /// <remarks>
    /// This method is typically used for logging informational messages during application
    /// execution.  The optional <paramref name="line"/> and <paramref name="caller"/> parameters are automatically
    /// populated  by the compiler to provide context about the source of the log entry.
    /// </remarks>
    /// <param name="message">The message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the calling member. Defaults to "unknown" if not provided.</param>
    void Info(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Logs an error message along with optional caller information, including the line number and member name.
    /// </summary>
    /// <remarks>
    /// This method is intended for logging error messages with contextual information about the
    /// caller. The <paramref name="line"/> and <paramref name="caller"/> parameters are automatically populated  using
    /// the <see cref="System.Runtime.CompilerServices.CallerLineNumberAttribute"/> and  <see
    /// cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/> attributes, respectively.
    /// </remarks>
    /// <param name="message">The error message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the member that invoked the method. Defaults to the caller's member name.</param>
    void Error(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Logs a warning message along with optional caller information for debugging purposes.
    /// </summary>
    /// <remarks>
    /// This method is typically used to log warnings during application execution, providing
    /// additional context  about the source of the warning through the optional <paramref name="line"/> and <paramref
    /// name="caller"/> parameters.
    /// </remarks>
    /// <param name="message">The warning message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the member that invoked the method. Defaults to the caller's member name.</param>
    void Warn(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Logs a debug message along with optional caller information, including the line number and member name.
    /// </summary>
    /// <remarks>
    /// This method is intended for debugging purposes and can be used to trace execution flow or log
    /// diagnostic information. The <paramref name="line"/> and <paramref name="caller"/> parameters are automatically
    /// populated by the compiler  when the method is called, unless explicitly overridden.
    /// </remarks>
    /// <param name="message">The debug message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the calling member. Defaults to "unknown" if not provided.</param>
    void Debug(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Logs a fatal error message, including optional caller information.
    /// </summary>
    /// <remarks>
    /// This method is intended for logging critical errors that require immediate attention. Caller
    /// information, such as the line number and member name, is automatically captured to aid in debugging.
    /// </remarks>
    /// <param name="message">The error message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the member that invoked the method. Defaults to "unknown" if not provided.</param>
    void Fatal(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Logs a verbose-level message along with optional caller information for debugging purposes.
    /// </summary>
    /// <remarks>
    /// This method is typically used for detailed logging during development or debugging. The
    /// <paramref name="line"/> and <paramref name="caller"/> parameters are automatically populated using the <see
    /// cref="System.Runtime.CompilerServices.CallerLineNumberAttribute"/> and <see
    /// cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/> attributes, respectively.
    /// </remarks>
    /// <param name="message">The message to log. Cannot be null or empty.</param>
    /// <param name="line">The line number in the source code where the method was called. Defaults to the caller's line number.</param>
    /// <param name="caller">The name of the calling member. Defaults to "unknown" if not provided.</param>
    void Verbose(string message, [CallerLineNumber] int line = 0, [CallerMemberName] string caller = "unknown");

    /// <summary>
    /// Writes the stack trace of the specified exception to the application's trace log.
    /// </summary>
    /// <remarks>
    /// This method is typically used for debugging or error tracking purposes. Ensure that the
    /// exception provided contains meaningful information, as the stack trace will be logged as-is.
    /// </remarks>
    /// <param name="ex">The exception whose stack trace will be logged. Cannot be <see langword="null"/>.</param>
    void WriteStackTraceLog(Exception ex);
}
