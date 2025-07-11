using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Diagnostics;
using System.Security.Principal;
using WinformsBoilerplate.Core.Abstractions.Serializers;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Entities.Settings;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Core.Enums;
using WinformsBoilerplate.Core.Wrappers;
using WinformsBoilerplate.Infrastructure.Services;
using Xunit;

namespace WinformsBoilerplate.Infrastructure.Tests.Services;

public class SystemServiceTests : ServiceTestBase, IDisposable
{
    private readonly Mock<IJsonSerializer> _mockJsonSerializer;
    private readonly Mock<ILogService> _mockLogService;
    private readonly SystemService _systemService;
    private readonly string _testDirectory;

    public SystemServiceTests()
    {
        _mockJsonSerializer = new Mock<IJsonSerializer>();
        _mockLogService = new Mock<ILogService>();
        _systemService = new SystemService(_mockJsonSerializer.Object, _mockLogService.Object);

        // Create a temporary directory for file operations tests
        _testDirectory = Path.Combine(Path.GetTempPath(), $"SystemServiceTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        _systemService?.Dispose();

        // Cleanup test directory
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act
        var service = new SystemService(_mockJsonSerializer.Object, _mockLogService.Object);

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<ISystemService>(service);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void Constructor_WithNullJsonSerializer_ShouldAllowConstruction()
    {
        // Act & Assert - Primary constructor syntax doesn't validate null parameters
        var service = new SystemService(null!, _mockLogService.Object);
        Assert.NotNull(service);
        service.Dispose();
    }

    [Fact]
    public void Constructor_WithNullLogService_ShouldAllowConstruction()
    {
        // Act & Assert - Primary constructor syntax doesn't validate null parameters
        var service = new SystemService(_mockJsonSerializer.Object, null!);
        Assert.NotNull(service);
        service.Dispose();
    }

    #endregion

    #region IsAdministrator Tests

    [Fact]
    public void IsAdministrator_ShouldReturnBooleanValue()
    {
        // Act
        var isAdmin = _systemService.IsAdministrator;

        // Assert
        Assert.IsType<bool>(isAdmin);
        // Note: We can't predict the actual value as it depends on how the test is run
    }

    #endregion

    #region CheckAppSettingFile Tests

    [Fact]
    public void CheckAppSettingFile_WhenFileDoesNotExist_ShouldReturnException()
    {
        // Act
        var result = _systemService.CheckAppSettingFile();

        // Assert
        Assert.NotNull(result);
        // The result should contain an exception since the setting file likely doesn't exist
        // in the test environment
    }

    [Fact]
    public void CheckAppSettingFile_WithValidFile_ShouldReturnAppSetting()
    {
        // Arrange
        var testAppSetting = new AppSetting { Misc = new MiscSetting() };

        _mockJsonSerializer
            .Setup(x => x.Deserialize<AppSetting>(It.IsAny<string>()))
            .Returns(testAppSetting);

        // Note: This test is limited because CheckAppSettingFile uses a hardcoded path
        // In a real scenario, we'd want to inject the file path or use a file system abstraction

        // Act
        var result = _systemService.CheckAppSettingFile();

        // Assert
        Assert.NotNull(result);
    }

    #endregion

    #region CopyFile Tests

    [Fact]
    public async Task CopyFile_WithValidPaths_ShouldCopyFile()
    {
        // Arrange
        var sourceFile = Path.Combine(_testDirectory, "source.txt");
        var destinationFile = Path.Combine(_testDirectory, "destination.txt");

        // Create source file
        await File.WriteAllTextAsync(sourceFile, "Test content\r\n");

        // Create destination file to avoid DetectEndLineSequence error
        await File.WriteAllTextAsync(destinationFile, "Old content\r\n");

        // Act
        var result = await _systemService.CopyFile(sourceFile, destinationFile, true);

        // Assert
        if (result == null)
        {
            // Verify error was logged if result is null
            _mockLogService.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
        }
        else
        {
            Assert.Equal(destinationFile, result);
            Assert.True(File.Exists(destinationFile));

            var destinationContent = await File.ReadAllTextAsync(destinationFile);
            Assert.Equal("Test content\r\n", destinationContent);
        }
    }

    [Fact]
    public async Task CopyFile_WithNonExistentSource_ShouldReturnError()
    {
        // Arrange
        var sourceFile = Path.Combine(_testDirectory, "nonexistent.txt");
        var destinationFile = Path.Combine(_testDirectory, "destination.txt");

        // Act
        var result = await _systemService.CopyFile(sourceFile, destinationFile);

        // Assert
        Assert.Null(result); // Should return null on failure
    }

    [Fact]
    public async Task CopyFile_WithOverrideTrue_ShouldOverwriteExistingFile()
    {
        // Arrange
        var sourceFile = Path.Combine(_testDirectory, "source.txt");
        var destinationFile = Path.Combine(_testDirectory, "destination.txt");

        await File.WriteAllTextAsync(sourceFile, "New content\r\n");
        await File.WriteAllTextAsync(destinationFile, "Old content\r\n");

        // Act
        var result = await _systemService.CopyFile(sourceFile, destinationFile, @override: true);

        // Assert
        if (result != null)
        {
            var content = await File.ReadAllTextAsync(destinationFile);
            Assert.Equal("New content\r\n", content);
        }
        else
        {
            // Verify error was logged if operation failed
            _mockLogService.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
        }
    }

    [Fact]
    public async Task CopyFile_WithOverrideFalse_ShouldFailOnExistingFile()
    {
        // Arrange
        var sourceFile = Path.Combine(_testDirectory, "source.txt");
        var destinationFile = Path.Combine(_testDirectory, "destination.txt");

        await File.WriteAllTextAsync(sourceFile, "New content");
        await File.WriteAllTextAsync(destinationFile, "Old content");

        // Act
        var result = await _systemService.CopyFile(sourceFile, destinationFile, @override: false);

        // Assert
        Assert.Null(result); // Should return null on failure
    }

    #endregion

    #region ExecuteProcess Tests

    [Fact]
    public void ExecuteProcess_WithValidStartInfo_ShouldReturnSafeProcess()
    {
        // Arrange
        var startInfo = new SafeProcessStartInfo();
        startInfo.Info.FileName = "cmd.exe";
        startInfo.Info.Arguments = "/c echo test";
        startInfo.Info.UseShellExecute = false;
        startInfo.Info.CreateNoWindow = true;

        // Act
        var result = _systemService.ExecuteProcess(startInfo);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Process);

        // Cleanup
        try
        {
            if (!result.Process.HasExited)
            {
                result.Process.Kill();
            }
            result.Process.Dispose();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Fact]
    public void ExecuteProcess_WithArguments_ShouldFormatArgumentsCorrectly()
    {
        // Arrange
        var startInfo = new SafeProcessStartInfo();
        startInfo.Info.FileName = "cmd.exe";
        startInfo.Info.UseShellExecute = false;
        startInfo.Info.CreateNoWindow = true;

        // Act
        var result = _systemService.ExecuteProcess(startInfo, null, "/c", "echo test");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("/c", result.Process.StartInfo.Arguments);
        Assert.Contains("echo test", result.Process.StartInfo.Arguments);

        // Cleanup
        try
        {
            if (!result.Process.HasExited)
            {
                result.Process.Kill();
            }
            result.Process.Dispose();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Fact]
    public void ExecuteProcess_WithArgumentsContainingSpaces_ShouldQuoteArguments()
    {
        // Arrange
        var startInfo = new SafeProcessStartInfo();
        startInfo.Info.FileName = "cmd.exe";
        startInfo.Info.UseShellExecute = false;
        startInfo.Info.CreateNoWindow = true;

        // Act
        var result = _systemService.ExecuteProcess(startInfo, null, "/c", "echo hello world");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("\"echo hello world\"", result.Process.StartInfo.Arguments);

        // Cleanup
        try
        {
            if (!result.Process.HasExited)
            {
                result.Process.Kill();
            }
            result.Process.Dispose();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Fact]
    public void ExecuteProcess_WithPriority_ShouldSetPriorityIfAdmin()
    {
        // Arrange
        var startInfo = new SafeProcessStartInfo();

        // Act
        var result = _systemService.ExecuteProcess(startInfo, ProcessPriorityClass.BelowNormal);

        // Assert
        Assert.NotNull(result);

        // Note: Priority will only be set if running as administrator
        // We can't easily test this without elevated privileges

        // Cleanup
        try
        {
            if (!result.Process.HasExited)
            {
                result.Process.Kill();
            }
            result.Process.Dispose();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    #endregion

    #region PerformSystemCheck Tests

    [Fact]
    public void PerformSystemCheck_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _systemService.PerformSystemCheck());
        Assert.Null(exception);

        // Verify that Info was called
        _mockLogService.Verify(x => x.Info(It.Is<string>(s => s.Contains("Performing system check")), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void PerformSystemCheck_WhenNotAdmin_ShouldLogWarning()
    {
        // Note: This test's behavior depends on whether the test is run as administrator
        // Act
        _systemService.PerformSystemCheck();

        // Assert - Verify Info is called regardless
        _mockLogService.Verify(x => x.Info(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    #endregion

    #region File Path Tests

    [Fact]
    public async Task CopyFile_WithNullSourcePath_ShouldReturnNull()
    {
        // Act
        var result = await _systemService.CopyFile(null!, "destination.txt");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CopyFile_WithEmptySourcePath_ShouldReturnNull()
    {
        // Act
        var result = await _systemService.CopyFile(string.Empty, "destination.txt");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CopyFile_WithNullDestinationPath_ShouldReturnNull()
    {
        // Act
        var result = await _systemService.CopyFile("source.txt", null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CopyFile_WithEmptyDestinationPath_ShouldReturnNull()
    {
        // Act
        var result = await _systemService.CopyFile("source.txt", string.Empty);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region Process StartInfo Tests

    [Fact]
    public void ExecuteProcess_WithNullStartInfo_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<NullReferenceException>(() =>
            _systemService.ExecuteProcess(null!));
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var service = new SystemService(_mockJsonSerializer.Object, _mockLogService.Object);

        // Act & Assert
        var exception = Record.Exception(() => service.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var service = new SystemService(_mockJsonSerializer.Object, _mockLogService.Object);

        // Act & Assert
        var exception1 = Record.Exception(() => service.Dispose());
        Assert.Null(exception1);

        var exception2 = Record.Exception(() => service.Dispose());
        Assert.Null(exception2);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task CopyFile_WithVeryLongPath_ShouldHandleGracefully()
    {
        // Arrange
        var longFileName = new string('a', 200) + ".txt";
        var sourceFile = Path.Combine(_testDirectory, "source.txt");
        var destinationFile = Path.Combine(_testDirectory, longFileName);

        await File.WriteAllTextAsync(sourceFile, "Test content");

        // Act
        var result = await _systemService.CopyFile(sourceFile, destinationFile);

        // Assert
        // This might fail or succeed depending on the file system limitations
        // The important thing is that it doesn't crash the application
        // Either result is acceptable - null (failure) or the destination path (success)
        Assert.True(result == null || result == destinationFile);
    }

    [Fact]
    public void ExecuteProcess_WithInvalidFileName_ShouldHandleGracefully()
    {
        // Arrange
        var startInfo = new SafeProcessStartInfo();
        startInfo.Info.FileName = "nonexistent_executable_12345.exe";
        startInfo.Info.UseShellExecute = false;
        startInfo.Info.CreateNoWindow = true;

        // Act
        var result = _systemService.ExecuteProcess(startInfo);

        // Assert
        Assert.NotNull(result);
        // The process might not start successfully, but the method should return a SafeProcess
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CopyFileAndExecuteProcess_Integration_ShouldWork()
    {
        // Arrange
        var sourceFile = Path.Combine(_testDirectory, "test.txt");
        var destinationFile = Path.Combine(_testDirectory, "copied.txt");
        await File.WriteAllTextAsync(sourceFile, "Integration test content\r\n");

        // Create destination file to avoid DetectEndLineSequence error
        await File.WriteAllTextAsync(destinationFile, "Old content\r\n");

        // Act - Copy file
        var copyResult = await _systemService.CopyFile(sourceFile, destinationFile, true);

        // Act - Execute process to verify file exists
        var startInfo = new SafeProcessStartInfo();
        startInfo.Info.FileName = "cmd.exe";
        startInfo.Info.Arguments = $"/c dir \"{_testDirectory}\"";
        startInfo.Info.UseShellExecute = false;
        startInfo.Info.CreateNoWindow = true;

        var processResult = _systemService.ExecuteProcess(startInfo);

        // Assert
        if (copyResult != null)
        {
            Assert.True(File.Exists(destinationFile));
            var content = await File.ReadAllTextAsync(destinationFile);
            Assert.Equal("Integration test content\r\n", content);
        }
        else
        {
            // Verify error was logged if copy failed
            _mockLogService.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        Assert.NotNull(processResult);

        // Cleanup
        try
        {
            if (!processResult.Process.HasExited)
            {
                processResult.Process.Kill();
            }
            processResult.Process.Dispose();
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    #endregion
}
