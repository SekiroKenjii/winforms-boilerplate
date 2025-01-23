using Core.Abstractions.Services;
using Core.Constants;
using Core.Extensions;
using Core.Wrappers;
using System.Diagnostics;

namespace Infrastructure.Services;

public class SystemService : ISystemService
{
    public Process ExecuteProcess(
        string fileName, ProcessPriorityClass? priority = null, params string[] args)
    {
        Process process = new();
        process.StartInfo.FileName = fileName;

        if (args is { Length: > 0 })
        {
            Array.ForEach(args, arg => {
                string formattedArg = arg.Contains(' ') ? $"\"{arg}\"" : arg;

                if (!string.IsNullOrEmpty(process.StartInfo.Arguments))
                {
                    process.StartInfo.Arguments += " ";
                }

                process.StartInfo.Arguments += formattedArg;
            });
        }

        _ = process.Start();

        if (priority != null)
        {
            process.PriorityClass = (ProcessPriorityClass)priority;
        }

        return process;
    }

    public string? MoveFile(string sourceFilePath, string destinationFilePath, bool @override = false)
    {
        string? sourcePath = Path.GetDirectoryName(sourceFilePath);

        if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourceFilePath))
        {
            return default;
        }

        string? destinationPath = Path.GetDirectoryName(destinationFilePath);

        if (string.IsNullOrEmpty(destinationPath))
        {
            return default;
        }

        if (!Directory.Exists(destinationPath))
        {
            _ = Directory.CreateDirectory(destinationPath);
        }

        if (File.Exists(destinationFilePath))
        {
            string destinationFile = Path.GetFileName(destinationFilePath);
            string destinationFileRenamed = RenameDuplicateFile(destinationFile);
            string destinationFilePathRenamed = destinationFilePath.Replace(destinationFile, RenameDuplicateFile(destinationFile));

            return ThrowableFunction<string, Exception>.Run(() => {
                if (@override)
                {
                    string bakFolder = Path.Combine(destinationPath, Folders.BACKUP);

                    if (!Directory.Exists(bakFolder))
                    {
                        _ = Directory.CreateDirectory(bakFolder);
                    }

                    File.Replace(sourceFilePath, destinationFilePath, Path.Combine(bakFolder, $"{destinationFileRenamed}.bak"));

                    return destinationFilePath;
                }

                File.Move(sourceFilePath, destinationFilePathRenamed);

                return destinationFilePathRenamed;
            }).Catch(_ => File.Copy(sourceFilePath, destinationFilePathRenamed));
        }

        ThrowableAction<Exception>
            .Run(() => File.Move(sourceFilePath, destinationFilePath))
            .Catch(_ => File.Copy(sourceFilePath, destinationFilePath));

        return destinationFilePath;
    }

    public void WatchFileChanged(string filePath)
    {
        throw new NotImplementedException();
    }

    private static string RenameDuplicateFile(string file, string padding = "")
    {
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
        string ext = Path.GetExtension(file);

        if (!string.IsNullOrEmpty(padding))
        {
            return $"{fileNameWithoutExt}{padding}{ext}";
        }

        string currentTime = DateTime.Now.ToString("MMddyy_hhmmsstt");

        return $"{fileNameWithoutExt}_{currentTime}{ext}";
    }
}
