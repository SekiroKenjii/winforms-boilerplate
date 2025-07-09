using System.Reflection;

namespace WinformsBoilerplate.Core.Constants;

public static class Common
{
    public const string APP_UNIQUE_NAME = "Global_Unique_Name_2DA1CE79-B3FA-4F7E-86FA-9E3583F573EB_WinformsBoilerplate";

    public static class Attributes
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public;
    }

    public static class DefaultMessages
    {
        public const string COMMON_ERROR = "Something went wrong on our end";
        public const string UNEXPECTED_ERROR = "An unexpected error occurred";
        public const string FATAL_ERROR = "A fatal error occurred. The program will be terminated";
    }

    public static class Executables
    {
        public const string GIT = "git.exe";
        public const string EXPLORER = "explorer.exe";
        public const string NOTEPAD = "notepad.exe";
    }

    public static class Strings
    {
        public const string UNKNOWN = "Unknown";
        public const string UNDEFINED = "Undefined";
    }

    public static class Chars
    {
        public const char SEPARATOR = '|';
        public const char SPACE = ' ';
        public const char TAB = '\t';
        public const char LINE_FEED = '\n';
        public const char RETURN = '\r';
    }
}
