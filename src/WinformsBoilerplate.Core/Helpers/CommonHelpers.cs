using System.Globalization;

namespace WinformsBoilerplate.Core.Helpers;

/// <summary>
/// Common helper methods for various operations.
/// </summary>
public static class CommonHelpers
{
    /// <summary>
    /// Returns a string if it is not null or empty, otherwise returns a default value.
    /// </summary>
    /// <param name="str">a string to check</param>
    /// <param name="default">a default value to return if the string is null or empty</param>
    /// <returns></returns>
    public static string StringEmptyDefault(string? str, string @default = "")
    {
        return str ?? @default;
    }

    /// <summary>
    /// Converts a string to a DateTime object.
    /// If the conversion fails, it returns a default value or DateTime.MinValue.
    /// </summary>
    /// <param name="dateString">a string representation of a date</param>
    /// <param name="default">a default value to return if the conversion fails</param>
    /// <returns></returns>
    public static DateTime ConvertStringToDate(string dateString, DateTime? @default = null)
    {
        return DateTime.TryParse(dateString, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AssumeUniversal, out DateTime date)
            ? date
            : @default ?? DateTime.MinValue;
    }

    /// <summary>
    /// Returns the application startup path.
    /// </summary>
    /// <returns>the application startup path</returns>
    public static string AppStartupPath()
    {
        string? dirName = Path.GetDirectoryName(AppContext.BaseDirectory);

        if (string.IsNullOrEmpty(dirName))
        {
            return AppContext.BaseDirectory;
        }

        string lastDir = new DirectoryInfo(dirName).Name;

        return string.Equals(lastDir, "bin", StringComparison.InvariantCultureIgnoreCase)
            ? dirName[..^3]
            : dirName;
    }
}
