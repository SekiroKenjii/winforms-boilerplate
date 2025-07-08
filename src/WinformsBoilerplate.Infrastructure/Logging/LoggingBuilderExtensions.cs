using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WinformsBoilerplate.Core.Abstractions.Services;

namespace WinformsBoilerplate.Infrastructure.Logging;

/// <summary>
/// Extension methods for adding the LogServiceLogger to an ILoggingBuilder.
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds a LogServiceLogger provider to the logging builder.
    /// </summary>
    /// <param name="builder">The logging builder to configure.</param>
    public static void AddLogService(this ILoggingBuilder builder, Action<LogServiceLoggerOptions> configureOptions)
    {
        _ = builder.Services.Configure(configureOptions);

        builder.Services.Add(
            ServiceDescriptor.Singleton<IConfigureOptions<LoggerFilterOptions>>(sp => {
                LogServiceLoggerOptions op = sp.GetRequiredService<IOptions<LogServiceLoggerOptions>>().Value;

                return new ConfigureOptions<LoggerFilterOptions>(options => options.MinLevel = op.MinimumLogLevel);
            }));

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, LogServiceLoggerProvider>(sp =>
                new LogServiceLoggerProvider(
                    sp.GetRequiredService<ILogService>(),
                    sp.GetRequiredService<IOptions<LogServiceLoggerOptions>>()
                )));
    }
}
