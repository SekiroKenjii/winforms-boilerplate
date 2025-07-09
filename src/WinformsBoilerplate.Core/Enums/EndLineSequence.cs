namespace WinformsBoilerplate.Core.Enums;

/// <summary>
/// Specifies the end-of-line sequence used in text processing.
/// </summary>
/// <remarks>
/// This enumeration defines the line-ending formats commonly used in text files:
/// <list type="bullet">
///     <item>
///         <term><see cref="LF"/></term>
///         <description>
///             Represents a line feed (LF) character, commonly used in Unix-based systems.
///         </description>
///     </item>
///     <item>
///         <term><see cref="CRLF"/></term>
///         <description>
///             Represents a carriage return
///             followed by a line feed (CRLF) sequence, commonly used in Windows-based systems.
///         </description>
///     </item>
/// </list>
/// Use this enumeration to specify or interpret the line-ending format in text-related operations.
/// </remarks>
public enum EndLineSequence
{
    /// <summary>
    /// Represents a line feed character used in text processing.
    /// </summary>
    /// <remarks>The <see langword="LF"/> constant is commonly used to denote a line feed character (ASCII
    /// code 10), which is often utilized in text files and streams to signify the end of a line.</remarks>
    LF,

    /// <summary>
    /// Represents a carriage return followed by a line feed (CRLF) sequence.
    /// </summary>
    /// <remarks>The CRLF sequence is commonly used as a line terminator in text files and network
    /// protocols.</remarks>
    CRLF
}
