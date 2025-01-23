using Core.Structs;
using System.Diagnostics;
using System.Reflection;
using static Core.Constants.Common;

namespace Core.Extensions;

/// <summary>
/// Provides extension methods for Exception class
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Default value for unknown information in stack trace details.
    /// </summary>
    private const string UNKNOWN = "Unknown";

    /// <summary>
    /// Converts an exception's stack trace into a structured format using CallStack objects.
    /// </summary>
    /// <param name="exception">The exception to process.</param>
    /// <returns>
    /// A dictionary where keys are exception messages and values are collections of CallStack objects
    /// representing the complete stack trace hierarchy, including inner exceptions.
    /// </returns>
    /// <remarks>
    /// This method processes the entire exception chain, including all inner exceptions.
    /// For each stack frame, it captures:
    /// <list type="bullet">
    ///     <item>Module information</item>
    ///     <item>Method name</item>
    ///     <item>Source file details</item>
    ///     <item>Line numbers</item>
    ///     <item>Memory addresses</item>
    /// </list>
    /// </remarks>
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
                    Type = currentException.GetType().FullName ?? UNKNOWN,
                    Inner = new() {
                        Module = method?.Module.Name ?? UNKNOWN,
                        Method = method?.Name ?? UNKNOWN,
                        File = Path.GetFileName(frame.GetFileName()) ?? UNKNOWN,
                        Line = frame.GetFileLineNumber(),
                        Address = frame.GetNativeOffset() >= 0
                            ? $"0x{frame.GetNativeOffset():X}"
                            : UNKNOWN
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
    /// <remarks>
    /// The formatted output includes:
    /// <list type="bullet">
    ///     <item>Exception messages</item>
    ///     <item>Column headers for stack frame information</item>
    ///     <item>Detailed stack frame data in aligned columns</item>
    ///     <item>Memory addresses in hexadecimal format</item>
    ///     <item>Module, file, and method information</item>
    /// </list>
    /// The output format follows:
    /// Address    Module                                    Unit            Line  Method
    /// </remarks>
    public static string ToFormattedString(this Exception exception)
    {
        StringWriter formattedStackWritter = new();
        Exception? currentException = exception;

        while (currentException != null)
        {
            formattedStackWritter.WriteLine($"{DefaultMessages.UNEXPECTED_ERROR}: {currentException.Message}");
            formattedStackWritter.WriteLine($"{"Address",-10} {"Module",-40} {"Unit",-15} {"Line",-5} Method");
            StackTrace stackTrace = new(currentException, true);
            StackFrame[] stackFrames = stackTrace.GetFrames();

            Array.ForEach(stackFrames, frame => {
                MethodBase? method = frame.GetMethod();
                string moduleName = method?.Module.Name ?? UNKNOWN;
                string methodName = method?.Name ?? UNKNOWN;
                string fileName = Path.GetFileName(frame.GetFileName()) ?? UNKNOWN;
                int lineNumber = frame.GetFileLineNumber();
                string address = frame.GetNativeOffset() >= 0
                    ? $"0x{frame.GetNativeOffset():X}"
                    : UNKNOWN;
                formattedStackWritter.WriteLine($"{address,-10} {moduleName,-40} {fileName,-15} {lineNumber,-5} {methodName}");
            });

            currentException = currentException.InnerException;

            if (currentException == null)
            {
                formattedStackWritter.WriteLine();
            }
        }

        return formattedStackWritter.ToString();
    }
}
