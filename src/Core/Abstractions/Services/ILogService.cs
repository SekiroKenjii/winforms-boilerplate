
using Core.Enums;
using Serilog.Core;
using Serilog.Events;

namespace Core.Abstractions.Services;

/// <summary>
/// Defines a service for handling application logging operations.
/// Provides separate loggers for control/UI operations and file-based logging,
/// along with methods to create, configure, and use these loggers.
/// </summary>
public interface ILogService : IDisposable
{
    /// <summary>
    /// Retrieves a logger instance based on the specified logger type.
    /// </summary>
    /// <param name="loggerType">The type of logger to retrieve.</param>
    /// <returns>A Logger instance if available for the specified type; otherwise, null.</returns>
    /// <remarks>
    /// Ensure the appropriate logger is created using CreateControlLogger or CreateFileLoggers
    /// before attempting to retrieve it.
    /// </remarks>
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
    /// This method must be called before attempting to use any file loggers.
    /// The file loggers are responsible for persisting log entries to disk
    /// and may include different configurations for various logging purposes:
    /// <list type="bullet">
    /// ///     <item>Application events logging</item>
    ///     <item>Error logging</item>
    ///     <item>Audit logging</item>
    ///     <item>Debug information logging</item>
    /// </list>
    /// Each logger may include its own rolling file policies and specific file formatting.
    /// </remarks>
    void CreateFileLoggers();

    /// <summary>
    /// Binds the control logger to a specific context type for contextual logging.
    /// </summary>
    /// <typeparam name="TContext">The type that provides the logging context.</typeparam>
    /// <remarks>
    /// This method should be called after ensuring the control logger has been created.
    /// The context type helps in categorizing and filtering log entries based on their source.
    /// Common context types might include form names, component types, or service identifiers.
    /// </remarks>
    void BindControlLoggerContext<TContext>();

    /// <summary>
    /// Writes a log entry with the specified level and message to the appropriate logger.
    /// </summary>
    /// <param name="logLevel">The severity level of the log entry (e.g., Information, Warning, Error).</param>
    /// <param name="message">The message to be logged.</param>
    /// <remarks>
    /// The log entry will be written to both control and file loggers if they are available.
    /// The log level determines how the message is handled, displayed, and potentially filtered:
    /// <list type="bullet">
    ///     <item>Verbose: Detailed debug information</item>
    ///     <item>Debug: Internal system details</item>
    ///     <item>Information: General operational messages</item>
    ///     <item>Warning: Non-critical issues</item>
    ///     <item>Error: Errors that need attention</item>
    ///     <item>Fatal: Critical errors that may cause system shutdown</item>
    /// </list>
    /// </remarks>
    void WriteLog(LogEventLevel logLevel, string message);

    /// <summary>
    /// Writes the stack trace of an exception to the log.
    /// </summary>
    /// <param name="ex">The exception whose stack trace should be logged.</param>
    /// <remarks>
    /// This method provides detailed exception logging including:
    /// <list type="bullet">
    ///     <item>Exception type and message</item>
    ///     <item>Stack trace showing the call hierarchy</item>
    ///     <item>Inner exception details (if present)</item>
    ///     <item>Additional exception properties</item>
    /// </list>
    /// The information is logged at the Error level and is written to all available loggers.
    /// </remarks>
    void WriteStackTrace(Exception ex);
}
