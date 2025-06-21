using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Serializers;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Constants;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Core.Enums;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Core.Wrappers;

namespace WinformsBoilerplate.Infrastructure.Services;

public class SystemService(
    IJsonSerializer jsonSerializer,
    ILogService logService) : Disposable, ISystemService
{
    private readonly Dictionary<string, FileSystemWatcher> _watchers = [];

    /// <inheritdoc />
    public bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    /// <inheritdoc />
    public void CleanupLocalStore()
    {
        // Implement cleanup logic here
        logService.Info("Cleaning up local store...");

        ThrowableAction
            .Run(() => {
                string localStorePath = CommonHelpers.AppStartupPath();

                if (Directory.Exists(localStorePath))
                {
                    Directory.Delete(localStorePath, true);
                    logService.Info("Local store cleaned successfully.");

                    return;
                }

                logService.Warn("Local store directory does not exist. No cleanup needed.");
            })
            .Catch(ex => logService.Error($"Error during local store cleanup: {ex.ToFormattedString()}"));
    }

    /// <inheritdoc />
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
                    logService.Info($"destination file end of line sequence: {endLine.ToString()}");

                    File.Copy(destinationFilePath, Path.Combine(bakFolder, destinationFileRenamed), true);
                    File.Copy(sourceFilePath, destinationFilePath, @override);

                    await AdjustEndLineSequence(destinationFilePath, endLine);

                    return destinationFilePath;
                })
                .CatchAsync(ex => logService.Error(ex.ToFormattedString()));
        }

        return await ThrowableFunction<string>
            .RunAsync(async () => {
                EndLineSequence endLine = DetectEndLineSequence(destinationFilePath);
                logService.Info($"destination file end of line sequence: {endLine.ToString()}");

                File.Copy(sourceFilePath, destinationFilePath, @override);

                await AdjustEndLineSequence(destinationFilePath, endLine);

                return destinationFilePath;
            })
            .CatchAsync(ex => logService.Error(ex.ToFormattedString()));
    }

    /// <inheritdoc />
    public SafeProcess ExecuteProcess(SafeProcessStartInfo startInfo, ProcessPriorityClass? priority = null, params scoped ReadOnlySpan<string> args)
    {
        SafeProcess safeProcess = new(startInfo);

        args.ForEach((argument, _) => {
            string formattedArg = argument.Contains(' ') ? $"\"{argument}\"" : argument;

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
            .Catch(ex => logService.Error(ex.ToFormattedString()));

        return safeProcess;
    }

    /// <inheritdoc />
    public void PerformSystemCheck()
    {
        logService.Info("Performing system check...");

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("This application is only supported on Windows.");
        }

        if (!IsAdministrator)
        {
            logService.Warn("The application is not running with administrator privileges. Some features may be limited.");
        }

        logService.Info("System check completed successfully.");
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
            pattern: $"{Common.Chars.RETURN}{Common.Chars.LINE_FEED}|{Common.Chars.RETURN}|{Common.Chars.LINE_FEED}",
            replacement: $"{Common.Chars.LINE_FEED}"
        );
        string adjustContent = endLine == EndLineSequence.CRLF
            ? normalized.Replace($"{Common.Chars.LINE_FEED}", $"{Common.Chars.RETURN}{Common.Chars.LINE_FEED}")
            : normalized.Replace($"{Common.Chars.LINE_FEED}", $"{Common.Chars.LINE_FEED}");

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
        int idx = txt.IndexOf(Common.Chars.LINE_FEED);

        return idx switch {
            > 0 when txt[idx - 1] == Common.Chars.RETURN => EndLineSequence.CRLF,
            >= 0 => EndLineSequence.LF,
            _ => txt.Contains(Common.Chars.RETURN)
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
