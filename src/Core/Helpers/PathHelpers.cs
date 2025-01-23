namespace Core.Helpers;

public static class PathHelpers
{
    public static string GetStartupPath(string appStartupPath)
    {
        string? dirName = Path.GetDirectoryName(appStartupPath);

        if (string.IsNullOrEmpty(dirName))
        {
            return appStartupPath;
        }

        string lastDir = new DirectoryInfo(dirName).Name;

        return string.Equals(lastDir, "bin", StringComparison.InvariantCultureIgnoreCase)
            ? dirName.Remove(dirName.Length - 3)
            : dirName;
    }
}
