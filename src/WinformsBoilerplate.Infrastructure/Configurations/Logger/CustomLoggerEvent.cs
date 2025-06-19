using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace WinformsBoilerplate.Infrastructure.Configurations.Logger;

/// <summary>
/// Provides a custom logger event interface for handling log events in a WinForms application.
/// </summary>
public interface ICustomLoggerEvent : ILogEventSink
{
    /// <summary>
    /// Gets or sets the action to be invoked when a log event is received for a RichTextBox control.
    /// </summary>
    Action<string, string>? OnRichTextBoxLogReceived { get; set; }

    /// <summary>
    /// Gets or sets the action to be invoked when a log event is received for a DataGridView control.
    /// </summary>
    Action<DateTimeOffset, LogEventLevel, string>? OnDataGridViewLogReceived { get; set; }
}

public class CustomLoggerEvent(ITextFormatter textFormatter, bool isGridLogger = false) : ICustomLoggerEvent
{
    /// <inheritdoc />
    public Action<string, string>? OnRichTextBoxLogReceived { get; set; }

    /// <inheritdoc />
    public Action<DateTimeOffset, LogEventLevel, string>? OnDataGridViewLogReceived { get; set; }

    /// <inheritdoc />
    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        if (textFormatter == null)
        {
            throw new InvalidOperationException("Missing Log Formatter");
        }

        var renderSpace = new StringWriter();
        textFormatter.Format(logEvent, renderSpace);

        if (isGridLogger)
        {
            OnDataGridViewLogReceived?.Invoke(logEvent.Timestamp, logEvent.Level, renderSpace.ToString());

            return;
        }

        _ = logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? contextProperty);

        OnRichTextBoxLogReceived?.Invoke(contextProperty?.ToString().Trim('"') ?? "", renderSpace.ToString());
    }
}
