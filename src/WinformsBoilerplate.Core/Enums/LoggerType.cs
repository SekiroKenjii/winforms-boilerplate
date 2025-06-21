namespace WinformsBoilerplate.Core.Enums;

/// <summary>
/// Represents the type of logger used in the application.
/// </summary>
public enum LoggerType
{
    /// <summary>
    /// Represents the logger which writes log messages to the UI control.
    /// </summary>
    Control = 0,

    /// <summary>
    /// Represents the logger which writes log messages to a file.
    /// </summary>
    File = 1,

    /// <summary>
    /// Represents the logger which captures and logs stack traces for debugging purposes. 
    /// </summary>
    StackTrace = 2,

    /// <summary>
    /// Represents the logger which captures and logs application freezes or hangs.
    /// </summary>
    Freezes = 3
}
