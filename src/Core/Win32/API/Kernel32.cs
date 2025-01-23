namespace Core.Win32.API;

public static partial class Kernel32
{
    /// <summary>
    /// Writes a string value to a specified section and key in an initialization file.
    /// </summary>
    /// <param name="appName">The section name in the initialization file.</param>
    /// <param name="keyName">The key name within the specified section.</param>
    /// <param name="string">The string value to write.</param>
    /// <param name="fileName">The path to the initialization file.</param>
    /// <returns>
    /// Nonzero if the operation succeeds, zero if it fails or if any of the input parameters are null or empty.
    /// Use GetLastError to get extended error information on failure.
    /// </returns>
    /// <remarks>
    /// This method performs null/empty validation on all parameters before calling the native Windows API.
    /// If any parameter is null or empty, the method returns 0 without making the API call.
    /// </remarks>
    public static long WritePrivateProfileString(
        string appName,
        string keyName,
        string @string,
        string fileName)
    {
        if (string.IsNullOrEmpty(appName) ||
            string.IsNullOrEmpty(keyName) ||
            string.IsNullOrEmpty(@string) ||
            string.IsNullOrEmpty(fileName))
        {
            return 0;
        }

        return WritePrivateProfileStringW(appName, keyName, @string, fileName);
    }

    /// <summary>
    /// Retrieves a string value from a specified section and key in an initialization file.
    /// </summary>
    /// <param name="appName">The section name in the initialization file.</param>
    /// <param name="keyName">The key name within the specified section.</param>
    /// <param name="default">The default value to return if the key is not found.</param>
    /// <param name="returnedString">A span of characters to receive the retrieved string.</param>
    /// <param name="size">The size of the returnedString buffer in characters.</param>
    /// <param name="fileName">The path to the initialization file.</param>
    /// <returns>
    /// The number of characters copied to the buffer (excluding the null terminator) if successful,
    /// or 0 if the operation fails or if any of the input parameters are invalid.
    /// </returns>
    /// <remarks>
    /// This method performs validation on all parameters before calling the native Windows API:
    /// <list type="bullet">
    /// <item><description>Checks for null or empty strings in appName, keyName, and fileName</description></item>
    /// <item><description>Verifies that returnedString is not null and has a length greater than 0</description></item>
    /// <item><description>Ensures size is greater than 0</description></item>
    /// </list>
    /// If any validation fails, the method returns 0 without making the API call.
    /// </remarks>
    public static int GetPrivateProfileString(
        string appName,
        string keyName,
        string @default,
        Span<char> returnedString,
        int size,
        string fileName)
    {
        if (string.IsNullOrEmpty(appName) ||
            string.IsNullOrEmpty(keyName) ||
            returnedString == null ||
            returnedString.Length == 0 ||
            size <= 0 ||
            string.IsNullOrEmpty(fileName))
        {
            return 0;
        }

        return GetPrivateProfileStringW(appName, keyName, @default, returnedString, size, fileName);
    }
}
