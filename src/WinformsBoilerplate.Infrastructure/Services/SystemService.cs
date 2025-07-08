using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Serializers;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Core.Enums;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Core.Wrappers;
using static WinformsBoilerplate.Core.Constants.Common;

namespace WinformsBoilerplate.Infrastructure.Services;

public class SystemService(
    IJsonSerializer jsonSerializer,
    ILogService logService) : Disposable, ISystemService
{
    private readonly Dictionary<string, FileSystemWatcher> _watchers = [];

    /// <inheritdoc cref="ISystemService.IsAdministrator" />
    public bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    /// <inheritdoc cref="ISystemService.CheckAppSettingFile" />
    public ThrowableFunction<AppSetting?, Exception> CheckAppSettingFile()
    {
        string settingFile = Path.Combine(CommonHelpers.AppStartupPath(), Files.SETTING_FILE);

        return ThrowableFunction<AppSetting?>
            .Run(() => {
                using Stream stream = File.OpenRead(settingFile);

                if (stream.Length <= 0)
                {
                    return new();
                }

                using var reader = new StreamReader(stream);
                string settingContent = reader.ReadToEnd();

                return jsonSerializer.Deserialize<AppSetting>(settingContent);
            });
    }

    /// <inheritdoc cref="ISystemService.CopyFile(string, string, bool)" />
    public async Task<string?> CopyFile(string sourceFilePath, string destinationFilePath, bool @override = false)
    {
        string? sourcePath = Path.GetDirectoryName(sourceFilePath);

        if (string.IsNullOrEmpty(sourcePath))
        {
            return null;
        }

        string? destinationPath = Path.GetDirectoryName(destinationFilePath);

        if (string.IsNullOrEmpty(destinationPath))
        {
            return null;
        }

        if (!Directory.Exists(destinationPath))
        {
            _ = Directory.CreateDirectory(destinationPath);
        }

        if (File.Exists(destinationFilePath))
        {
            string destinationFile = Path.GetFileName(destinationFilePath);
            string destinationFileRenamed = Rename(destinationFile);

            return await ThrowableFunction<string>
                .RunAsync(async () => {
                    string bakFolder = Path.Combine(AppContext.BaseDirectory, Folders.BACKUP);

                    if (!Directory.Exists(bakFolder))
                    {
                        _ = Directory.CreateDirectory(bakFolder);
                    }

                    EndLineSequence endLine = DetectEndLineSequence(destinationFilePath);
                    logService.Info($"Destination file end of line sequence: {endLine}");

                    File.Copy(destinationFilePath, Path.Combine(bakFolder, destinationFileRenamed), true);
                    File.Copy(sourceFilePath, destinationFilePath, @override);

                    await AdjustEndLineSequence(destinationFilePath, endLine);

                    return destinationFilePath;
                })
                .CatchAsync(ex => logService.Error($"Error copying file from '{sourceFilePath}' to '{destinationFilePath}': {ex.ToFormattedString()}"));
        }

        return await ThrowableFunction<string>
            .RunAsync(async () => {
                EndLineSequence endLine = DetectEndLineSequence(destinationFilePath);
                logService.Info($"Destination file end of line sequence: {endLine}");

                File.Copy(sourceFilePath, destinationFilePath, @override);

                await AdjustEndLineSequence(destinationFilePath, endLine);

                return destinationFilePath;
            })
            .CatchAsync(ex => logService.Error($"Error copying file from '{sourceFilePath}' to '{destinationFilePath}': {ex.ToFormattedString()}"));
    }

    /// <inheritdoc cref="ISystemService.ExecuteProcess(SafeProcessStartInfo, ProcessPriorityClass?, ReadOnlySpan{string})" />
    public SafeProcess ExecuteProcess(SafeProcessStartInfo startInfo, ProcessPriorityClass? priority = null, params scoped ReadOnlySpan<string> args)
    {
        SafeProcess safeProcess = new(startInfo);

        args.ForEach((argument, _) => {
            string formattedArg = argument.Contains(Chars.SPACE) ? $"\"{argument}\"" : argument;

            if (!string.IsNullOrEmpty(safeProcess.Process.StartInfo.Arguments))
            {
                safeProcess.Process.StartInfo.Arguments += " ";
            }

            safeProcess.Process.StartInfo.Arguments += formattedArg;
        });

        if (priority.HasValue && IsAdministrator)
        {
            safeProcess.Process.PriorityClass = priority.Value;
        }

        ThrowableAction
            .Run(safeProcess.Start)
            .Catch(ex => logService.Error($"Error starting process: {ex.ToFormattedString()}"));

        return safeProcess;
    }

    /// <inheritdoc cref="ISystemService.PerformSystemCheck" />
    public void PerformSystemCheck()
    {
        logService.Info("Performing system check...");

        if (!IsAdministrator)
        {
            logService.Warn("The application is not running with administrator privileges. Some features may be limited.");
        }

        logService.Info("System check completed successfully.");
    }

    /// <inheritdoc cref="ISystemService.ReadLocalStore" />
    public ConcurrentDictionary<string, object?> ReadLocalStore()
    {
        string storeFile = GetContextFilePath(Files.LOCAL_STORE_FILE);

        if (string.IsNullOrEmpty(storeFile))
        {
            return [];
        }

        ConcurrentDictionary<string, object?>? store = ThrowableFunction<ConcurrentDictionary<string, object?>?>
            .Run(() => {
                using Stream stream = File.OpenRead(storeFile);

                if (stream.Length <= 0)
                {
                    return new();
                }

                using var reader = new StreamReader(stream);
                string storeContent = reader.ReadToEnd();

                return jsonSerializer.Deserialize<ConcurrentDictionary<string, object?>>(storeContent);
            })
            .Catch(ex => logService.Error($"Error reading local store: {ex.ToFormattedString()}"));

        return store ?? [];
    }

    /// <inheritdoc cref="ISystemService.SaveLocalStore(object)" />
    public void SaveLocalStore(object store)
    {
        string storeFile = GetContextFilePath(Files.LOCAL_STORE_FILE);

        if (string.IsNullOrEmpty(storeFile))
        {
            return;
        }

        string storeSerialized = jsonSerializer.Serialize(store);

        ThrowableAction
            .Run(() => File.WriteAllText(storeFile, storeSerialized))
            .Catch(ex => {
                logService.Error($"Error saving local store: {ex.ToFormattedString()}");

                throw ex;
            });
    }

    /// <inheritdoc cref="ISystemService.WatchFileChanged(string, FileSystemEventHandler, bool)" />
    public void WatchFileChanged(string filePath, FileSystemEventHandler next, bool @override = false)
    {
        if (!File.Exists(filePath))
        {
            logService.Warn($"File '{filePath}' does not exist. Cannot watch for changes.");

            return;
        }

        if (_watchers.TryGetValue(filePath, out FileSystemWatcher? oldWatcher))
        {
            if (!@override)
            {
                logService.Warn($"File watcher for '{filePath}' already exists. Use override to replace it.");

                return;
            }

            oldWatcher.Dispose();
            _ = _watchers.Remove(filePath);

            _ = _watchers.TryAdd(filePath, GetFileSystemWatcher(filePath, next));

            return;
        }

        _ = _watchers.TryAdd(filePath, GetFileSystemWatcher(filePath, next));
    }

    /// <inheritdoc cref="ISystemService.ShutdownApplication(Action?)" />
    public void ShutdownApplication(Action? onShutdown = null)
    {
        logService.Info("Shutting down application...");

        // Invoke the optional shutdown action if provided
        onShutdown?.Invoke();

        // Exit the application
        Application.Exit();
        Environment.Exit(0);
    }

    /// <inheritdoc cref="ISystemService.RestartApplication(Action?)" />
    public void RestartApplication(Action? onRestart = null)
    {
        logService.Info("Restarting application...");

        // Invoke the optional restart action if provided
        onRestart?.Invoke();

        // Restart the application and exit the current process
        Application.Restart();
        Environment.Exit(0);
    }

    /// <summary>
    /// Renames a file by appending either a specified padding string or a timestamp to its name.
    /// </summary>
    /// <remarks>This method does not modify the file on disk; it only generates a new name based on the
    /// provided input.</remarks>
    /// <param name="file">The full path or name of the file to be renamed. Must include an extension.</param>
    /// <param name="padding">An optional string to append to the file name. If not provided or empty, a timestamp in the format
    /// "yyyyMMddhhmmsstt" will be appended instead.</param>
    /// <returns>A string representing the new file name, including the original extension.</returns>
    private static string Rename(string file, string padding = "")
    {
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
        string ext = Path.GetExtension(file);

        if (!string.IsNullOrEmpty(padding))
        {
            return $"{fileNameWithoutExt}{padding}{ext}";
        }

        string currentTime = DateTime.Now.ToString("yyyyMMddhhmmsstt");

        return $"{fileNameWithoutExt}_{currentTime}{ext}";
    }

    /// <summary>
    /// Adjusts the end-of-line sequence in the specified file to match the provided <see cref="EndLineSequence"/>
    /// format.
    /// </summary>
    /// <remarks>This method reads the file's content, normalizes existing end-of-line sequences, and rewrites
    /// the file with the specified format. The file is saved using UTF-8 encoding without a byte order mark
    /// (BOM).</remarks>
    /// <param name="file">The path to the file whose end-of-line sequence will be adjusted. Cannot be null or empty.</param>
    /// <param name="endLine">The desired end-of-line sequence format. Use <see cref="EndLineSequence.CRLF"/> for carriage return and line
    /// feed or <see cref="EndLineSequence.LF"/> for line feed only.</param>
    /// <returns></returns>
    private static async Task AdjustEndLineSequence(string file, EndLineSequence endLine)
    {
        string srcContent = await File.ReadAllTextAsync(file);
        string normalized = Regex.Replace(
            input: srcContent,
            pattern: $"{Chars.RETURN}{Chars.LINE_FEED}|{Chars.RETURN}|{Chars.LINE_FEED}",
            replacement: $"{Chars.LINE_FEED}"
        );
        string adjustContent = endLine == EndLineSequence.CRLF
            ? normalized.Replace($"{Chars.LINE_FEED}", $"{Chars.RETURN}{Chars.LINE_FEED}")
            : normalized.Replace($"{Chars.LINE_FEED}", $"{Chars.LINE_FEED}");

        await File.WriteAllTextAsync(
            file,
            contents: adjustContent,
            new UTF8Encoding(false) // Disable BOM for UTF-8
        );
    }

    /// <summary>
    /// Detects the end-of-line sequence used in the specified text file.
    /// </summary>
    /// <remarks>This method reads up to 1024 characters from the beginning of the file to determine the
    /// end-of-line sequence. If no end-of-line sequence is detected within the first 1024 characters, the method
    /// defaults to <see cref="EndLineSequence.CRLF"/>.</remarks>
    /// <param name="file">The path to the file to analyze. The file must be encoded in UTF-8.</param>
    /// <returns>An <see cref="EndLineSequence"/> value indicating the type of end-of-line sequence detected: <see
    /// cref="EndLineSequence.CRLF"/> for carriage return followed by line feed, or <see cref="EndLineSequence.LF"/> for
    /// line feed only.</returns>
    private static EndLineSequence DetectEndLineSequence(string file)
    {
        using var reader = new StreamReader(file, Encoding.UTF8);
        char[] buffer = new char[1024];
        int charsRead = reader.Read(buffer, 0, buffer.Length);
        string txt = new(buffer, 0, charsRead);
        int idx = txt.IndexOf(Chars.LINE_FEED);

        return idx switch {
            > 0 when txt[idx - 1] == Chars.RETURN => EndLineSequence.CRLF,
            >= 0 => EndLineSequence.LF,
            _ => txt.Contains(Chars.RETURN)
                ? EndLineSequence.LF
                : EndLineSequence.CRLF
        };
    }

    /// <summary>
    /// Generates the full file path for a given file name within the specified base directory or the application's
    /// startup path.
    /// </summary>
    /// <remarks>If <paramref name="baseDir"/> is null, the method defaults to using the application's startup
    /// path. If the file does not exist at the generated path, the method creates an empty file at that
    /// location.</remarks>
    /// <param name="fileName">The name of the file for which the path is generated. Cannot be null or empty.</param>
    /// <param name="baseDir">The base directory to use for constructing the file path. If null, the application's startup path is used.</param>
    /// <returns>The full file path for the specified file name. If the file does not exist, it is created and the path to the
    /// newly created file is returned.</returns>
    private static string GetContextFilePath(string fileName, string? baseDir = null)
    {
        string filePath = Path.Combine(baseDir ?? CommonHelpers.AppStartupPath(), fileName);

        if (File.Exists(filePath))
        {
            return filePath;
        }

        using FileStream _ = File.Create(filePath);

        return filePath;
    }

    /// <summary>
    /// Creates and configures a <see cref="FileSystemWatcher"/> to monitor changes to a specific file.
    /// </summary>
    /// <remarks>The <see cref="FileSystemWatcher"/> is configured to monitor changes to the file's last write
    /// time, size,  name, attributes, creation time, and security settings. The <see
    /// cref="FileSystemWatcher.EnableRaisingEvents"/>  property is set to <see langword="true"/> by default.</remarks>
    /// <param name="filePath">The full path of the file to monitor. Must not be <see langword="null"/> or empty.</param>
    /// <param name="onFileChanged">The event handler to invoke when the file is changed, created, or deleted.  This delegate is attached to the
    /// <see cref="FileSystemWatcher.Changed"/>, <see cref="FileSystemWatcher.Created"/>,  and <see
    /// cref="FileSystemWatcher.Deleted"/> events.</param>
    /// <returns>A configured <see cref="FileSystemWatcher"/> instance that monitors the specified file. The caller is
    /// responsible for disposing the returned watcher when it is no longer needed.</returns>
    private static FileSystemWatcher GetFileSystemWatcher(string filePath, FileSystemEventHandler onFileChanged)
    {
        FileSystemWatcher watcher = new() {
            Path = Path.GetDirectoryName(filePath)!,
            Filter = Path.GetFileName(filePath),
            NotifyFilter = NotifyFilters.LastWrite |
                           NotifyFilters.Size |
                           NotifyFilters.FileName |
                           NotifyFilters.Attributes |
                           NotifyFilters.CreationTime |
                           NotifyFilters.Security,
            EnableRaisingEvents = true
        };
        watcher.Changed += onFileChanged;
        watcher.Created += onFileChanged;
        watcher.Deleted += onFileChanged;

        return watcher;
    }
}
