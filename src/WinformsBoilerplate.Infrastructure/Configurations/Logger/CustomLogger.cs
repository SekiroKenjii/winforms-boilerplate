using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace WinformsBoilerplate.Infrastructure.Configurations.Logger;

/// <summary>
/// Provides custom logging configurations for the application.
/// </summary>
public static class CustomLogger
{
    private static readonly ITextFormatter s_defaultTextFormatter =
        new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss} {Level} {Message:lj}{NewLine}{Exception}");
    private static readonly ITextFormatter s_defaultGridTextFormatter =
        new MessageTemplateTextFormatter("{Message:lj}{Exception}");
    private static readonly ICustomLoggerEvent s_defaultTextLogEvent = new CustomLoggerEvent(s_defaultTextFormatter);
    private static readonly ICustomLoggerEvent s_defaultGridLogEvent =
        new CustomLoggerEvent(s_defaultGridTextFormatter, true);

    public static ICustomLoggerEvent RichTextBoxLogEventSink { get; set; } = s_defaultTextLogEvent;

    public static ICustomLoggerEvent DataGridViewLogEventSink { get; set; } = s_defaultGridLogEvent;

    /// <summary>
    /// Configures Serilog to write logs to a RichTextBox control.
    /// </summary>
    /// <param name="configuration">The logger configuration.</param>
    /// <param name="formatter">The text formatter to use.</param>
    /// <returns>The updated logger configuration.</returns>
    public static LoggerConfiguration WriteToRichTextBoxControl(
        this LoggerConfiguration configuration, ITextFormatter? formatter = null)
    {
        return configuration.WriteTo.Sink(MakeRichTextBoxLoggerSink(formatter));
    }

    /// <summary>
    /// Configures Serilog to write logs to a DataGridView control.
    /// </summary>
    /// <param name="configuration">The logger configuration.</param>
    /// <param name="outputFormat">The output format for the logs.</param>
    /// <returns>The updated logger configuration.</returns>
    public static LoggerConfiguration WriteToDataGridViewControl(
        this LoggerConfiguration configuration, string? outputFormat = null)
    {
        return configuration.WriteTo.Sink(MakeDataGridViewLoggerSink(outputFormat));
    }

    /// <summary>
    /// Configures Serilog to write logs to a RichTextBox control with a default formatter.
    /// </summary>
    /// <param name="configuration">The logger configuration.</param>
    /// <returns>The instance of the custom logger event.</returns>
    private static ICustomLoggerEvent MakeRichTextBoxLoggerSink(ITextFormatter? formatter = null)
    {
        if (formatter == null)
        {
            return RichTextBoxLogEventSink;
        }

        RichTextBoxLogEventSink = new CustomLoggerEvent(formatter);

        return RichTextBoxLogEventSink;
    }

    /// <summary>
    /// Configures Serilog to write logs to a DataGridView control with a specified output format.
    /// </summary>
    /// <param name="outputFormat">The output format for the logs.</param>
    /// <returns>The instance of the custom logger event.</returns>
    private static ICustomLoggerEvent MakeDataGridViewLoggerSink(string? outputFormat = null)
    {
        if (string.IsNullOrEmpty(outputFormat))
        {
            return DataGridViewLogEventSink;
        }

        DataGridViewLogEventSink = new CustomLoggerEvent(new MessageTemplateTextFormatter(outputFormat), true);

        return DataGridViewLogEventSink;
    }
}
