using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Events;
using System.Collections.Concurrent;
using System.Text;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Extensions;

namespace WinformsBoilerplate.Infrastructure.Logging;

/// <summary>
/// Configuration options for the LogServiceLoggerProvider.
/// </summary>
public class LogServiceLoggerOptions
{
    /// <summary>
    /// Gets or sets the minimum log level. Logs below this level will be filtered out.
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Gets or sets a value indicating whether to initialize loggers during provider construction.
    /// </summary>
    public bool InitializeLoggersOnStartup { get; set; } = true;

    /// <summary>
    /// Gets or sets a dictionary of log level overrides for specific categories.
    /// </summary>
    public IDictionary<string, LogLevel> LogLevelOverrides { get; set; } = new Dictionary<string, LogLevel>();
}

/// <summary>
/// Provides an adapter between Microsoft.Extensions.Logging and the application's ILogService.
/// </summary>
public class LogServiceLoggerProvider : Disposable, ILoggerProvider
{
    private readonly ILogService _logService;
    private readonly LogServiceLoggerOptions _options;
    private readonly ConcurrentDictionary<string, LogServiceLogger> _loggers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LogServiceLoggerProvider"/> class.
    /// </summary>
    /// <param name="logService">The log service to use for logging.</param>
    /// <param name="options">The options for configuring the logger provider.</param>
    public LogServiceLoggerProvider(ILogService logService, IOptions<LogServiceLoggerOptions> options)
    {
        _logService = logService;
        _options = options.Value;

        if (_options.InitializeLoggersOnStartup)
        {
            try
            {
                _logService.CreateControlLogger();
                _logService.CreateFileLoggers();
            }
            catch (Exception ex)
            {
                // Use Console.Error as a fallback since logging isn't available yet
                Console.Error.WriteLine($"Failed to initialize loggers: {ex.ToFormattedString()}");
            }
        }
    }

    /// <summary>
    /// Creates a logger with the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>An ILogger instance.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        ObjectDisposedException.ThrowIf(Disposed, nameof(LogServiceLoggerProvider));
        return _loggers.GetOrAdd(categoryName, name => new LogServiceLogger(name, _logService, GetLogLevelForCategory(name)));
    }

    /// <summary>
    /// Disposes the provider and optionally releases managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!Disposed && disposing)
        {
            _loggers.Clear();
        }
    }

    private LogLevel GetLogLevelForCategory(string categoryName)
    {
        // Check for exact category match
        if (_options.LogLevelOverrides.TryGetValue(categoryName, out LogLevel level))
        {
            return level;
        }

        // Check for prefix match (e.g., "System." would match "System.Net.Http")
        foreach (KeyValuePair<string, LogLevel> kvp in _options.LogLevelOverrides)
        {
            if (categoryName.StartsWith(kvp.Key + ".", StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return _options.MinimumLogLevel;
    }
}

/// <summary>
/// An implementation of Microsoft.Extensions.Logging.ILogger that delegates to ILogService.
/// </summary>
public class LogServiceLogger : ILogger
{
    private readonly string _categoryName;
    private readonly ILogService _logService;
    private readonly LogLevel _minimumLogLevel;

    // For logging scopes
    private static readonly AsyncLocal<Stack<LogScope>> _scopeStack = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LogServiceLogger"/> class.
    /// </summary>
    /// <param name="categoryName">The category name for this logger.</param>
    /// <param name="logService">The log service to delegate logging to.</param>
    /// <param name="minimumLogLevel">The minimum log level for this logger.</param>
    public LogServiceLogger(string categoryName, ILogService logService, LogLevel minimumLogLevel)
    {
        _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _minimumLogLevel = minimumLogLevel;
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {

        ArgumentNullException.ThrowIfNull(state);

        var scope = new LogScope(state);
        GetScopeStack()?.Push(scope);

        return scope;
    }

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>True if the log level is enabled; otherwise, false.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minimumLogLevel;
    }

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">Id of the event.</param>
    /// <param name="state">The entry to be written. Can be also an object.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a string message of the state and exception.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(formatter);

        try
        {
            string message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null)
            {
                return;
            }

            // Add scope information if present
            Stack<LogScope>? scopeStack = GetScopeStack();
            if (scopeStack != null && scopeStack.Count > 0)
            {
                var sb = new StringBuilder();
                _ = sb.Append(message)
                      .Append(" => ");

                bool isFirst = true;
                foreach (LogScope scope in scopeStack)
                {
                    if (!isFirst)
                    {
                        _ = sb.Append(" => ");
                    }

                    _ = sb.Append(scope.State);
                    isFirst = false;
                }

                message = sb.ToString();
            }

            // Add the category name to the message
            string formattedMessage = $"[{_categoryName}] {message}";

            // Map Microsoft.Extensions.Logging.LogLevel to Serilog.Events.LogEventLevel
            LogEventLevel level = logLevel switch {
                LogLevel.Trace => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Critical => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };

            _logService.WriteLog(level, formattedMessage);

            if (exception != null && logLevel >= LogLevel.Error)
            {
                _logService.WriteStackTraceLog(exception);
            }
        }
        catch (Exception ex)
        {
            // To prevent logging failures from affecting the application, swallow the exception
            // but write to console for diagnostic purposes
            Console.Error.WriteLine($"Error while logging: {ex}");
        }
    }

    private static Stack<LogScope>? GetScopeStack()
    {
        var stack = _scopeStack.Value;
        if (stack == null)
        {
            stack = new Stack<LogScope>();
            _scopeStack.Value = stack;
        }

        return stack;
    }

    /// <summary>
    /// Represents a logical operation scope.
    /// </summary>
    private sealed class LogScope : IDisposable
    {
        public object State { get; }

        public LogScope(object state)
        {
            State = state;
        }

        public void Dispose()
        {
            var stack = GetScopeStack();
            if (stack != null && stack.Count > 0)
            {
                stack.Pop();
            }
        }

        public override string ToString()
        {
            return State?.ToString() ?? string.Empty;
        }
    }
}
