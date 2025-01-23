using System.Runtime.InteropServices;

namespace Core.Win32.API;

/// <summary>Provides access to <see href="https://learn.microsoft.com/en-us/windows/win32/api/winbase/">Windows Kernel32 API</see> functions for working with initialization files (.ini).</summary>
public static partial class Kernel32
{
    /// <summary>Copies a string into the specified section of an initialization file.</summary>
    /// <param name="lpAppName">
    /// The name of the section to which the string will be copied.
    /// If the section does not exist, it is created.
    /// The name of the section is case-independent; the string can be any combination of uppercase and lowercase letters.
    /// </param>
    /// <param name="lpKeyName">
    /// The name of the key to be associated with a string.
    /// If the key does not exist in the specified section, it is created.
    /// If this parameter is <c>NULL</c>, the entire section, including all entries within the section, is deleted.
    /// </param>
    /// <param name="lpString">
    /// A null-terminated string to be written to the file.
    /// If this parameter is <c>NULL</c>, the key pointed to by the <i>lpKeyName</i> parameter is deleted.
    /// </param>
    /// <param name="lpFileName">
    /// The name of the initialization file.
    /// If the file already exists and consists of Unicode characters, the function writes Unicode characters to the file.
    /// Otherwise, the function writes ANSI characters.
    /// </param>
    /// <returns>
    /// If the function successfully copies the string to the initialization file, the return value is nonzero.
    /// If the function fails, or if it flushes the cached version of the most recently accessed initialization file, the return value is zero.
    /// To get extended error information, call <see href="https://learn.microsoft.com/en-us/windows/desktop/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
    /// </returns>
    [LibraryImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial long WritePrivateProfileStringW(
        string lpAppName,
        string lpKeyName,
        string lpString,
        string lpFileName
    );

    /// <summary>Retrieves a string from the specified section in an initialization file.</summary>
    /// <param name="lpAppName">
    /// The name of the section containing the key name.
    /// If this parameter is <c>NULL</c>, the <c>GetPrivateProfileString</c> function copies all section names in the file to the supplied buffer.
    /// </param>
    /// <param name="lpKeyName">
    /// The name of the key whose associated string is to be retrieved.
    /// If this parameter is <c>NULL</c>, all key names in the section specified by the <i>lpAppName</i> parameter are copied to the buffer specified by the <i>lpReturnedString</i> parameter.
    /// </param>
    /// <param name="lpDefault">
    /// A default string.
    /// If the <i>lpKeyName</i> key cannot be found in the initialization file, <c>GetPrivateProfileString</c> copies the default string to the <i>lpReturnedString</i> buffer.
    /// If this parameter is <c>NULL</c>, the default is an empty string, "".
    /// Avoid specifying a default string with trailing blank characters.
    /// The function inserts a null character in the <i>lpReturnedString</i> buffer to strip any trailing blanks.
    /// </param>
    /// <param name="lpReturnedString">A pointer to the buffer that receives the retrieved string.</param>
    /// <param name="nSize">The size of the buffer pointed to by the <i>lpReturnedString</i> parameter, in characters.</param>
    /// <param name="lpFileName">
    /// The name of the initialization file.
    /// If this parameter does not contain a full path to the file, the system searches for the file in the Windows directory.
    /// </param>
    /// <returns>
    /// The return value is the number of characters copied to the buffer, not including the terminating null character.
    /// If neither <i>lpAppName</i> nor <i>lpKeyName</i> is <c>NULL</c> and the supplied destination buffer is too small to hold the requested string,
    /// the string is truncated and followed by a null character, and the return value is equal to
    /// <i>nSize</i> minus one.
    /// If either <i>lpAppName</i> or <i>lpKeyName</i> is <c>NULL</c> and the supplied destination
    /// buffer is too small to hold all the strings,
    /// the last string is truncated and followed by two null characters.
    /// In this case, the return value is equal to <i>nSize</i> minus two.
    /// In the event the initialization file specified by <i>lpFileName</i> is not found, or contains invalid values,
    /// this function will set <c>errorno</c> with a value of '0x2' (File Not Found).
    /// To retrieve extended error information, call <see href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
    /// </returns>
    [LibraryImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial int GetPrivateProfileStringW(
        string lpAppName,
        string lpKeyName,
        string lpDefault,
        Span<char> lpReturnedString,
        int nSize,
        string lpFileName
    );
}
