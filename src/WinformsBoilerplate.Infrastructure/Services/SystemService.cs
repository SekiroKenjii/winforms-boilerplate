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
    public ThrowableFunction<AppSettings?, Exception> CheckAppSettingFile()
    {
        string settingFile = Path.Combine(CommonHelpers.AppStartupPath(), Files.SETTING_FILE);

        return ThrowableFunction<AppSettings?>
            .Run(() => {
                using Stream stream = File.OpenRead(settingFile);

                if (stream.Length <= 0)
                {
                    return new();
                }

                using var reader = new StreamReader(stream);
                string settingContent = reader.ReadToEnd();

                return jsonSerializer.Deserialize<AppSettings>(settingContent);
            });
    }

    /// <inheritdoc cref="ISystemService.CreateDefaultSettingFile(bool)" />
    public bool CreateDefaultSettingFile(bool @override = false)
    {
        string settingFile = Path.Combine(CommonHelpers.AppStartupPath(), Files.SETTING_FILE);

        if (File.Exists(settingFile) && !@override)
        {
            logService.Warn($"Setting file '{settingFile}' already exists. Use override to replace it.");

            return true;
        }

        string settingSerialized = jsonSerializer.Serialize<AppSettings>(new());

        return ThrowableFunction<bool>
            .Run(() => {
                File.WriteAllText(settingFile, settingSerialized);

                return true;
            })
            .Catch(ex => logService.Error($"Error creating default setting file: {ex.ToFormattedString()}"));
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

        // Cleanup session store before shutdown to prevent data leakage
        // sessionStore.Clear();

        // Exit the application
        Application.Exit();
        Environment.Exit(0);
    }

    /// <inheritdoc cref="ISystemService.RestartApplication(Action?)" />
    public void RestartApplication(Action? onRestart = null)
    {
        logService.Info("Restarting application...");

        onRestart?.Invoke();

        // Cleanup session store before shutdown to prevent data leakage
        // sessionStore.Clear();

        // Restart the application and exit the current process
        Application.Restart();
        Environment.Exit(0);
    }

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
