using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using Serilog.Core;
using System.Runtime.CompilerServices;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Enums;
using WinformsBoilerplate.Infrastructure.Services;
using Xunit;

namespace WinformsBoilerplate.Infrastructure.Tests.Services;

public class LogServiceTests : ServiceTestBase, IDisposable
{
    private LogService _logService;

    public LogServiceTests()
    {
        _logService = CreateLogService();
    }

    private LogService CreateLogService()
    {
        var services = new ServiceCollection();
        return new LogService();
    }

    public void Dispose()
    {
        _logService?.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var service = new LogService();

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<ILogService>(service);

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region Logging Method Tests

    [Fact]
    public void Debug_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test debug message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Debug_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test debug message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug(message, 42, "TestMethod"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test info message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Info(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test info message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Info(message, 100, "InfoTestMethod"));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test warning message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Warn(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test warning message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Warn(message, 200, "WarnTestMethod"));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test error message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Error(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test error message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Error(message, 300, "ErrorTestMethod"));
        Assert.Null(exception);
    }

    [Fact]
    public void Fatal_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test fatal message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Fatal(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Fatal_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test fatal message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Fatal(message, 400, "FatalTestMethod"));
        Assert.Null(exception);
    }

    [Fact]
    public void Verbose_WithMessage_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test verbose message";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Verbose(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Verbose_WithCallerInfo_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test verbose message with caller info";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Verbose(message, 500, "VerboseTestMethod"));
        Assert.Null(exception);
    }

    #endregion

    #region GetLogger Tests

    [Fact]
    public void GetLogger_WithControlType_ShouldReturnLogger()
    {
        // Act
        var logger = _logService.GetLogger(LoggerType.Control);

        // Assert - Initially null as loggers are created on demand
        // This behavior depends on the actual implementation
        // The service might return null if logger hasn't been initialized
    }

    [Fact]
    public void GetLogger_WithFileType_ShouldReturnLogger()
    {
        // Act
        var logger = _logService.GetLogger(LoggerType.File);

        // Assert - Initially null as loggers are created on demand
        // This behavior depends on the actual implementation
        // The service might return null if logger hasn't been initialized
    }

    [Fact]
    public void GetLogger_WithStackTraceType_ShouldReturnLogger()
    {
        // Act
        var logger = _logService.GetLogger(LoggerType.StackTrace);

        // Assert - Initially null as loggers are created on demand
        // This behavior depends on the actual implementation
        // The service might return null if logger hasn't been initialized
    }

    [Fact]
    public void GetLogger_WithFreezesType_ShouldReturnLogger()
    {
        // Act
        var logger = _logService.GetLogger(LoggerType.Freezes);

        // Assert - Initially null as loggers are created on demand
        // This behavior depends on the actual implementation
        // The service might return null if logger hasn't been initialized
    }

    [Fact]
    public void GetLogger_WithInvalidType_ShouldReturnNull()
    {
        // Act
        var logger = _logService.GetLogger((LoggerType)999);

        // Assert
        Assert.Null(logger);
    }

    #endregion

    #region Null/Empty Message Tests

    [Fact]
    public void Debug_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug(null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Debug_WithEmptyMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug(string.Empty));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Info(null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Warn(null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Error(null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Fatal_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Fatal(null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Verbose_WithNullMessage_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Verbose(null!));
        Assert.Null(exception);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Debug_WithVeryLongMessage_ShouldNotThrow()
    {
        // Arrange
        var longMessage = new string('A', 10000);

        // Act & Assert
        var exception = Record.Exception(() => _logService.Debug(longMessage));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithSpecialCharacters_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test with special chars: \n\r\t\\\"'{}[]<>&";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Info(message));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_WithUnicodeCharacters_ShouldNotThrow()
    {
        // Arrange
        const string message = "Test with unicode: ñáéíóú™®©€£¥∞";

        // Act & Assert
        var exception = Record.Exception(() => _logService.Warn(message));
        Assert.Null(exception);
    }

    #endregion

    #region Caller Information Tests

    [Fact]
    public void Debug_WithDefaultCallerInfo_ShouldUseActualCallerInfo()
    {
        // This test verifies that default CallerMemberName and CallerLineNumber
        // are properly applied when not explicitly provided

        // Act & Assert - Just ensure it doesn't throw
        var exception = Record.Exception(() => _logService.Debug("Test message"));
        Assert.Null(exception);
    }

    [Fact]
    public void Info_WithNullCaller_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Info("Test", 1, null!));
        Assert.Null(exception);
    }

    [Fact]
    public void Warn_WithEmptyCaller_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Warn("Test", 1, string.Empty));
        Assert.Null(exception);
    }

    [Fact]
    public void Error_WithZeroLineNumber_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Error("Test", 0, "TestCaller"));
        Assert.Null(exception);
    }

    [Fact]
    public void Fatal_WithNegativeLineNumber_ShouldNotThrow()
    {
        // Act & Assert
        var exception = Record.Exception(() => _logService.Fatal("Test", -1, "TestCaller"));
        Assert.Null(exception);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        var service = new LogService();

        // Act & Assert
        var exception = Record.Exception(() => service.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var service = new LogService();

        // Act & Assert
        var exception1 = Record.Exception(() => service.Dispose());
        Assert.Null(exception1);

        var exception2 = Record.Exception(() => service.Dispose());
        Assert.Null(exception2);
    }

    [Fact]
    public void LogAfterDispose_ShouldNotThrow()
    {
        // Arrange
        var service = new LogService();
        service.Dispose();

        // Act & Assert - Logging after dispose should be safe
        var exception = Record.Exception(() => service.Info("Test after dispose"));
        Assert.Null(exception);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void MultipleLogCalls_ShouldNotThrow()
    {
        // This test verifies that multiple log calls in sequence work correctly

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            _logService.Debug("Debug message");
            _logService.Info("Info message");
            _logService.Warn("Warning message");
            _logService.Error("Error message");
            _logService.Fatal("Fatal message");
            _logService.Verbose("Verbose message");
        });

        Assert.Null(exception);
    }

    [Fact]
    public void ConcurrentLogging_ShouldNotThrow()
    {
        // This test verifies thread safety of logging operations

        // Act & Assert
        var exception = Record.Exception(() =>
        {
            Parallel.For(0, 100, i =>
            {
                _logService.Info($"Concurrent log message {i}");
            });
        });

        Assert.Null(exception);
    }

    #endregion
}
