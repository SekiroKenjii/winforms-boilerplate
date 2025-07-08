# Logging in WinformsBoilerplate

This document explains how the logging infrastructure is set up in the WinformsBoilerplate application, which integrates the custom `ILogService` with Microsoft.Extensions.Logging.

## Logging Architecture

The application uses two logging systems that work together:

1. **ILogService** - A custom logging service specifically designed for this application
   - Provides methods like `Info()`, `Debug()`, `Error()`, etc.
   - Supports UI controls integration for displaying logs in the application
   - Uses Serilog under the hood for file logging and other sinks

2. **Microsoft.Extensions.Logging** - The standard .NET logging framework
   - Provides the `ILogger<T>` interface for logging
   - Supports structured logging with message templates
   - Enables integration with various logging providers

## How They Work Together

The integration is accomplished through a custom logger provider `LogServiceLoggerProvider` that bridges Microsoft.Extensions.Logging with our custom ILogService. This means:

- Logs written via `ILogger<T>` will be routed to the appropriate ILogService methods
- Both logging systems can be used side by side
- The UI controls will show logs from both systems
- All logs will be written to the same log files

## When to Use Each

### Use Microsoft.Extensions.Logging (ILogger<T>) when:

- Working with code that follows modern .NET patterns
- You want to take advantage of structured logging with message templates
- You want category-based logging with the type as the category
- Integrating with third-party libraries that use ILogger

Example:
```csharp
public class MyComponent
{
    private readonly ILogger<MyComponent> _logger;

    public MyComponent(ILogger<MyComponent> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("Operation started at {Time}", DateTime.UtcNow);
        // ...
    }
}
```

### Use ILogService when:

- You need direct access to UI control-based logging
- You want to use the specific methods for your application's logging needs
- You need to maintain compatibility with existing code

Example:
```csharp
public class MyService
{
    private readonly ILogService _logService;

    public MyService(ILogService logService)
    {
        _logService = logService;
    }

    public void ProcessData()
    {
        _logService.Info("Processing data...");
        // ...
    }
}
```

## Configuration

The logging system is configured in `Program.cs` where:

1. Microsoft.Extensions.Logging is configured with our custom provider:
   ```csharp
   .ConfigureLogging((hostContext, logging) => {
       logging.ClearProviders();
       logging.AddLogService();
   })
   ```

2. The existing `ILogService` is initialized as before:
   ```csharp
   services.AddLogger(sp);
   ```
