using System.Diagnostics;
using System.Reflection;
using WinformsBoilerplate.Core.Entities.Systems;
using static WinformsBoilerplate.Core.Constants.Common;

namespace WinformsBoilerplate.Core.Extensions;

/// <summary>
/// Provides extension methods for Exception class
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Converts an exception's stack trace into a structured format using CallStack objects.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <returns>
    /// A dictionary where keys are exception messages and values are collections of CallStack objects
    /// representing the complete stack trace hierarchy, including inner exceptions.
    /// </returns>
    public static Dictionary<string, IEnumerable<CallStack>> ToFormattedStackTrace(this Exception exception)
    {
        Dictionary<string, IEnumerable<CallStack>> callStackPairs = [];
        Exception? currentException = exception;

        while (currentException != null)
        {
            StackTrace stackTrace = new(currentException, true);
            List<CallStack> callStack = [];
            StackFrame[] stackFrames = stackTrace.GetFrames();

            Array.ForEach(stackFrames, frame => {
                MethodBase? method = frame.GetMethod();

                callStack.Add(new() {
                    Type = currentException.GetType().FullName ?? Strings.UNKNOWN,
                    Inner = new() {
                        Module = method?.Module.Name ?? Strings.UNKNOWN,
                        Method = method?.Name ?? Strings.UNKNOWN,
                        File = Path.GetFileName(frame.GetFileName()) ?? Strings.UNKNOWN,
                        Line = frame.GetFileLineNumber(),
                        Address = frame.GetNativeOffset() >= 0
                            ? $"0x{frame.GetNativeOffset():X}"
                            : Strings.UNKNOWN
                    }
                });
            });

            callStackPairs.Add(currentException.Message, callStack);
            currentException = currentException.InnerException;
        }

        return callStackPairs;
    }

    /// <summary>
    /// Converts an exception's stack trace into a formatted string representation.
    /// </summary>
    /// <param name="exception">The exception to format.</param>
    /// <returns>
    /// A formatted string containing the complete stack trace information,
    /// including all inner exceptions, with aligned columns for readability.
    /// </returns>
    public static string ToFormattedString(this Exception exception)
    {
        StringWriter formattedStackWriter = new();
        Exception? currentException = exception;

        while (currentException != null)
        {
            formattedStackWriter.WriteLine($"{DefaultMessages.UNEXPECTED_ERROR}: {currentException.Message}");
            formattedStackWriter.WriteLine($"{"Address",-16} {"Module",-50} {"Unit",-25} {"Line",-6} Method");
            StackTrace stackTrace = new(currentException, fNeedFileInfo: true);
            StackFrame[] stackFrames = stackTrace.GetFrames();

            Array.ForEach(stackFrames, frame => {
                MethodBase? method = frame.GetMethod();
                string moduleName = method?.Module.Name ?? Strings.UNKNOWN;
                string methodName = method?.Name ?? Strings.UNKNOWN;
                string fileName = Path.GetFileName(frame.GetFileName()) ?? Strings.UNKNOWN;
                int lineNumber = frame.GetFileLineNumber();
                string address = frame.GetNativeOffset() >= 0
                    ? $"0x{frame.GetNativeOffset():X}"
                    : Strings.UNKNOWN;
                formattedStackWriter.WriteLine($"{address,-16} {moduleName,-50} {fileName,-25} {lineNumber,-6} {methodName}");
            });

            currentException = currentException.InnerException;

            if (currentException == null)
            {
                formattedStackWriter.WriteLine();
            }
        }

        return formattedStackWriter.ToString();
    }
}
