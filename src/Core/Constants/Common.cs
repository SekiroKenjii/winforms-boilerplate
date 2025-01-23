using System.Reflection;

namespace Core.Constants;

public static class Common
{
    public static class Attributes
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public;
    }

    public static class DefaultMessages
    {
        public const string COMMON_ERROR = "Something went wrong on our end.";
        public const string UNEXPECTED_ERROR = "An unexpected error occurred.";
        public const string FATAL_ERROR = "A fatal error occurred. The program will be terminated.";
    }
}
