namespace Core.Structs;

/// <summary>
/// Represents the inner details of a call stack entry in the application.
/// </summary>
/// <param name="Module">The name of the module where the call originated.</param>
/// <param name="Method">The name of the method in the call stack.</param>
/// <param name="File">The source file name containing the code.</param>
/// <param name="Line">The line number in the source file.</param>
/// <param name="Address">The memory address of the call.</param>
public record struct CallStackInner(
    string Module,
    string Method,
    string File,
    int Line,
    string Address
);

/// <summary>
/// Represents a complete call stack entry with type information and inner details.
/// </summary>
/// <param name="Type">The type of the call stack entry (e.g., exception type, event type).</param>
/// <param name="Inner">The detailed information about the call stack entry.</param>
public record struct CallStack(string Type, CallStackInner Inner);
