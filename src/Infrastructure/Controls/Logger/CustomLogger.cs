using Core.Abstractions.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Infrastructure.Controls.Logger;

public static class CustomLogger
{
    private static readonly ITextFormatter DefaultTextFormatter =
        new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss} {Level} {Message:lj}{NewLine}{Exception}");
    private static readonly ITextFormatter DefaultGridTextFormatter =
        new MessageTemplateTextFormatter("{Message:lj}{Exception}");
    private static readonly ICustomLoggerEvent DefaultTextLogEvent = new CustomLoggerEvent(DefaultTextFormatter);
    private static readonly ICustomLoggerEvent DefaultGridLogEvent =
        new CustomLoggerEvent(DefaultGridTextFormatter, true);

    public static ICustomLoggerEvent RichTextBoxLogEventSink { get; set; } = DefaultTextLogEvent;

    public static ICustomLoggerEvent DataGridViewLogEventSink { get; set; } = DefaultGridLogEvent;

    public static ICustomLoggerEvent MakeRichTextBoxLoggerSink(ITextFormatter? formatter = null)
    {
        if (formatter == null)
        {
            return RichTextBoxLogEventSink;
        }

        RichTextBoxLogEventSink = new CustomLoggerEvent(formatter);

        return RichTextBoxLogEventSink;
    }

    public static ICustomLoggerEvent MakeDataGridViewLoggerSink(string? outputFormat = null)
    {
        if (string.IsNullOrEmpty(outputFormat))
        {
            return DataGridViewLogEventSink;
        }

        DataGridViewLogEventSink = new CustomLoggerEvent(new MessageTemplateTextFormatter(outputFormat), true);

        return DataGridViewLogEventSink;
    }
}
