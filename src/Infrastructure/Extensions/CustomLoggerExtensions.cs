using Infrastructure.Controls.Logger;
using Serilog;
using Serilog.Formatting;

namespace Infrastructure.Extensions;

public static class CustomLoggerExtensions
{
    public static LoggerConfiguration WriteToRichTextBoxControl(
        this LoggerConfiguration configuration, ITextFormatter? formatter = null)
    {
        return configuration.WriteTo.Sink(CustomLogger.MakeRichTextBoxLoggerSink(formatter));
    }

    public static LoggerConfiguration WriteToDataGridViewControl(
        this LoggerConfiguration configuration, string? outputFormat = null)
    {
        return configuration.WriteTo.Sink(CustomLogger.MakeDataGridViewLoggerSink(outputFormat));
    }
}
