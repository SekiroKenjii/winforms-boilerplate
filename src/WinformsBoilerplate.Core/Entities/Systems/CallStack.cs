namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Represents a call stack frame.
/// </summary>
public class CallStack
{
    /// <summary>
    /// The type of the call stack frame.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The inner call stack frame.
    /// </summary>
    public CallStackInner? Inner { get; set; }
}

/// <summary>
/// Represents the inner details of a call stack frame.
/// </summary>
public class CallStackInner
{
    /// <summary>
    /// The name of the module containing the call stack frame.
    /// </summary>
    public required string Module { get; set; }

    /// <summary>
    /// The name of the method containing the call stack frame.
    /// </summary>
    public required string Method { get; set; }

    /// <summary>
    /// The file path of the source file containing the call stack frame.
    /// </summary>
    public required string File { get; set; }

    /// <summary>
    /// The line number in the source file where the call stack frame is located.
    /// </summary>
    public required int Line { get; set; }

    /// <summary>
    /// The memory address of the call stack frame.
    /// </summary>
    public required string Address { get; set; }
}
