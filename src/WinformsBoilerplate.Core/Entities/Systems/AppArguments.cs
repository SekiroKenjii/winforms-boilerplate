namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Handles and stores command-line arguments passed to the application.
/// This class parses arguments in the format "--argument-name value" or standalone flags.
/// </summary>
/// <example>
/// Usage example:
/// <code>
/// string[] args = new[] { "--dpi-unaware", "true" };
/// var appArgs = new AppArguments().Map(args);
/// 
/// // Check if DPI unaware mode is enabled
/// if (appArgs.DpiUnaware.Key == "--dpi-unaware")
/// {
///     // Apply DPI unaware setting
///     // ...
/// }
/// </code>
/// </example>
public class AppArguments
{
    /// <summary>
    /// Controls whether the application should run in DPI unaware mode.
    /// </summary>
    /// <remarks>
    /// Key: "--dpi-unaware" <br/>
    /// Value: Optional value for the flag. If provided without value, an empty string is stored. <br/>
    /// When this flag is present, the application ignores system DPI settings.
    /// </remarks>
    public KeyValuePair<string, string>? DpiUnaware { get; set; }

    /// <summary>
    /// Parses an array of command-line arguments and maps them to properties.
    /// </summary>
    /// <param name="args">Command-line arguments array typically from Main method.</param>
    /// <returns>The current instance with populated properties.</returns>
    /// <remarks>
    /// The method processes arguments that start with "-" or "--" as commands.
    /// For each command, it looks for a subsequent value that doesn't start with "-".
    /// If no value is found or the next argument is another command, the command is treated as a flag.
    /// </remarks>
    public AppArguments Map(string[] args)
    {
        // Filter arguments that start with dash (commands/flags)
        IEnumerable<string> commands = Enumerable.Where(args, arg => arg.StartsWith('-'));

        foreach (string command in commands)
        {
            // Find position of current command and potential value
            int cmdIdx = Array.IndexOf(args, command);
            int nextIdx = cmdIdx + 1;

            // Check if there are no more arguments after the command
            if (nextIdx >= args.Length)
            {
                Save(command); // Save as flag without value
                continue;
            }

            string next = args[nextIdx];

            // If the next argument is another command (starts with -), 
            // then current command is treated as a flag
            if (next.StartsWith('-'))
            {
                Save(command); // Save as flag without value
                continue;
            }

            // Save command with its value
            Save(command, next);
        }

        return this;
    }

    /// <summary>
    /// Stores a command and its optional value to the appropriate property.
    /// </summary>
    /// <param name="cmd">The command string.</param>
    /// <param name="value">The value associated with the command, or empty string if none.</param>
    /// <remarks>
    /// Add new command mappings to the switch statement to support additional arguments.
    /// </remarks>
    private void Save(string cmd, string value = "")
    {
        switch (cmd)
        {
            case "--dpi-unaware":
                DpiUnaware = new(cmd, value);
                break;
            default:
                // Unrecognized commands are ignored
                break;
        }
    }
}
