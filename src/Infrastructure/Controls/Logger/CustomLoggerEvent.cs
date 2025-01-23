using Core.Abstractions.Events;
using Serilog.Events;
using Serilog.Formatting;

namespace Infrastructure.Controls.Logger;

public class CustomLoggerEvent(ITextFormatter textFormatter, bool isGridLogger = false)
    : ICustomLoggerEvent
{
    public Action<string, string>? OnRichTextBoxLogReceived { get; set; }

    public Action<DateTimeOffset, LogEventLevel, string>? OnDataGridViewLogReceived { get; set; }

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