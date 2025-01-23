using Serilog.Core;
using Serilog.Events;

namespace Core.Abstractions.Events;

public interface ICustomLoggerEvent : ILogEventSink
{
    Action<string, string>? OnRichTextBoxLogReceived { get; set; }

    Action<DateTimeOffset, LogEventLevel, string>? OnDataGridViewLogReceived { get; set; }
}
