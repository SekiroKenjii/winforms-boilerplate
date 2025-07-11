using Microsoft.Extensions.DependencyInjection;
using Moq;
using WinformsBoilerplate.Core.Abstractions.Components.Controls;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Infrastructure.Services;

namespace WinformsBoilerplate.Infrastructure.Tests.Services;

public class LayoutServiceTests : IDisposable
{
    private readonly Control _testControl;
    private bool _disposed = false;

    public LayoutServiceTests()
    {
        _testControl = new Control { Name = "TestControl" };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _testControl?.Dispose();
            _disposed = true;
        }
    }

    private static LayoutService CreateLayoutService(out Mock<IOverlayControl> mockOverlayControl, out Mock<IServiceProvider> mockServiceProvider)
    {
        mockOverlayControl = new Mock<IOverlayControl>();
        mockServiceProvider = new Mock<IServiceProvider>();

        // Create a local reference to avoid ref/out issues in lambda
        var overlayControlMock = mockOverlayControl.Object;

        // Setup ServiceProvider to create a real ServiceCollection for testing
        var services = new ServiceCollection();
        services.AddTransient<IOverlayControl>(_ => overlayControlMock);
        var realServiceProvider = services.BuildServiceProvider();

        return new LayoutService(realServiceProvider);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidServiceProvider_ShouldCreateInstance()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IOverlayControl>(_ => new Mock<IOverlayControl>().Object);
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var service = new LayoutService(serviceProvider);

        // Assert
        Assert.NotNull(service);
        Assert.IsAssignableFrom<ILayoutService>(service);

        // Cleanup
        service.Dispose();
        serviceProvider.Dispose();
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldNotThrow()
    {
        // Note: The actual LayoutService constructor doesn't validate null parameters
        // Arrange & Act & Assert
        var act = () => new LayoutService(null!);
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    #endregion

    #region ShowOverlay Tests

    [Fact]
    public void ShowOverlay_WithValidControlAndText_ShouldCreateAndShowOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var overlayText = "Loading...";

        // Act
        service.ShowOverlay(_testControl, overlayText);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = overlayText, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ShowOverlay_CalledTwiceOnSameControl_ShouldReuseExistingOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var overlayText1 = "Loading...";
        var overlayText2 = "Processing...";

        // Act
        service.ShowOverlay(_testControl, overlayText1);
        service.ShowOverlay(_testControl, overlayText2);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = overlayText1, Times.Once);
        mockOverlayControl.VerifySet(x => x.OverlayText = overlayText2, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Exactly(2));

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ShowOverlay_WithNullControl_ShouldThrowNullReferenceException()
    {
        // Note: The actual service accesses ctrl.Name without null checking
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.ShowOverlay(null!, "test");
        Assert.Throws<NullReferenceException>(act);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ShowOverlay_WithEmptyText_ShouldStillCreateOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);

        // Act
        service.ShowOverlay(_testControl, string.Empty);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = string.Empty, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Once);

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region HideOverlay Tests

    [Fact]
    public void HideOverlay_WithExistingOverlay_ShouldHideAndDisposeOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        service.HideOverlay(_testControl, dispose: true);

        // Assert
        mockOverlayControl.Verify(x => x.HideOverlay(true), Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void HideOverlay_WithExistingOverlayButNoDispose_ShouldHideWithoutDispose()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        service.HideOverlay(_testControl, dispose: false);

        // Assert
        mockOverlayControl.Verify(x => x.HideOverlay(false), Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void HideOverlay_WithNonExistentOverlay_ShouldNotThrow()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.HideOverlay(_testControl);
        var exception = Record.Exception(act);
        Assert.Null(exception);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void HideOverlay_WithNullControl_ShouldThrowNullReferenceException()
    {
        // Note: The actual service accesses ctrl.Name without null checking
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.HideOverlay(null!);
        Assert.Throws<NullReferenceException>(act);

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region IsOverlayVisible Tests

    [Fact]
    public void IsOverlayVisible_WithVisibleOverlay_ShouldReturnTrue()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        mockOverlayControl.Setup(x => x.Visible).Returns(true);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        var result = service.IsOverlayVisible(_testControl.Name);

        // Assert
        Assert.True(result);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void IsOverlayVisible_WithHiddenOverlay_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        mockOverlayControl.Setup(x => x.Visible).Returns(false);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        var result = service.IsOverlayVisible(_testControl.Name);

        // Assert
        Assert.False(result);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void IsOverlayVisible_WithNonExistentOverlay_ShouldReturnFalse()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act
        var result = service.IsOverlayVisible("NonExistentControl");

        // Assert
        Assert.False(result);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void IsOverlayVisible_WithNullOrEmptyControlName_ShouldThrowOrReturnFalse()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        // Null control name throws ArgumentNullException from Dictionary.TryGetValue
        Assert.Throws<ArgumentNullException>(() => service.IsOverlayVisible(null!));

        // Empty string should work fine
        Assert.False(service.IsOverlayVisible(string.Empty));

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region ToggleOverlay Tests

    [Fact]
    public void ToggleOverlay_WithStateTrue_ShouldShowOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var overlayText = "Loading...";

        // Act
        service.ToggleOverlay(_testControl, overlayText, state: true);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = overlayText, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ToggleOverlay_WithStateFalseAndExistingOverlay_ShouldHideOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        service.ToggleOverlay(_testControl, "ignored", state: false);

        // Assert
        mockOverlayControl.Verify(x => x.HideOverlay(false), Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ToggleOverlay_WithStateFalseAndNoExistingOverlay_ShouldNotThrow()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.ToggleOverlay(_testControl, "test", state: false);
        var exception = Record.Exception(act);
        Assert.Null(exception);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ToggleOverlay_WithStateTrueAndExistingOverlay_ShouldUpdateExistingOverlay()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var firstText = "Loading...";
        var secondText = "Processing...";
        service.ShowOverlay(_testControl, firstText);

        // Act
        service.ToggleOverlay(_testControl, secondText, state: true);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = firstText, Times.Once);
        mockOverlayControl.VerifySet(x => x.OverlayText = secondText, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Exactly(2));

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void ToggleOverlay_WithNullControl_ShouldThrowNullReferenceException()
    {
        // Note: The actual service accesses ctrl.Name without null checking
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.ToggleOverlay(null!, "test", true);
        Assert.Throws<NullReferenceException>(act);

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region UpdateOverlayText Tests

    [Fact]
    public void UpdateOverlayText_WithExistingOverlay_ShouldUpdateText()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var originalText = "Loading...";
        var newText = "Processing...";
        service.ShowOverlay(_testControl, originalText);

        // Act
        service.UpdateOverlayText(_testControl, newText);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = originalText, Times.Once);
        mockOverlayControl.VerifySet(x => x.OverlayText = newText, Times.Once);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void UpdateOverlayText_WithNonExistentOverlay_ShouldNotThrow()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.UpdateOverlayText(_testControl, "new text");
        var exception = Record.Exception(act);
        Assert.Null(exception);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void UpdateOverlayText_WithNullControl_ShouldThrowNullReferenceException()
    {
        // Note: The actual service accesses ctrl.Name without null checking
        // Arrange
        var service = CreateLayoutService(out _, out _);

        // Act & Assert
        var act = () => service.UpdateOverlayText(null!, "test");
        Assert.Throws<NullReferenceException>(act);

        // Cleanup
        service.Dispose();
    }

    [Fact]
    public void UpdateOverlayText_WithEmptyText_ShouldUpdateToEmptyText()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        service.ShowOverlay(_testControl, "Loading...");

        // Act
        service.UpdateOverlayText(_testControl, string.Empty);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = string.Empty, Times.Once);

        // Cleanup
        service.Dispose();
    }

    #endregion

    #region Disposal Tests

    [Fact]
    public void Dispose_WithMultipleOverlays_ShouldNotThrow()
    {
        // Note: We can't easily verify disposal calls due to service provider lifecycle management
        // This test ensures disposal works without throwing exceptions
        // Arrange
        var control1 = new Control { Name = "Control1" };
        var control2 = new Control { Name = "Control2" };

        var services = new ServiceCollection();
        services.AddTransient<IOverlayControl>(_ => new Mock<IOverlayControl>().Object);
        var serviceProvider = services.BuildServiceProvider();
        var service = new LayoutService(serviceProvider);

        try
        {
            service.ShowOverlay(control1, "Test1");
            service.ShowOverlay(control2, "Test2");

            // Act & Assert
            var act = () => service.Dispose();
            var exception = Record.Exception(act);
            Assert.Null(exception);
        }
        finally
        {
            // Cleanup
            control1.Dispose();
            control2.Dispose();
            serviceProvider.Dispose();
        }
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Note: We can't easily verify disposal calls due to how the service manages overlays
        // This test just ensures dispose doesn't throw when called multiple times
        // Arrange
        var service = CreateLayoutService(out _, out _);
        service.ShowOverlay(_testControl, "Test");

        // Act & Assert
        var act1 = () => service.Dispose();
        var act2 = () => service.Dispose(); // Second call

        var exception1 = Record.Exception(act1);
        Assert.Null(exception1);
        var exception2 = Record.Exception(act2);
        Assert.Null(exception2);
    }

    [Fact]
    public void Dispose_WithNullOverlaysInDictionary_ShouldNotThrow()
    {
        // Arrange
        var service = CreateLayoutService(out _, out _);
        service.ShowOverlay(_testControl, "Test");
        service.HideOverlay(_testControl, dispose: true); // This sets the dictionary value to null

        // Act & Assert
        var act = () => service.Dispose();
        var exception = Record.Exception(act);
        Assert.Null(exception);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void CompleteWorkflow_ShowHideUpdate_ShouldWorkCorrectly()
    {
        // Arrange
        var service = CreateLayoutService(out var mockOverlayControl, out _);
        var initialText = "Loading...";
        var updatedText = "Processing...";
        var finalText = "Complete";

        // Act - Show overlay
        service.ShowOverlay(_testControl, initialText);

        // Act - Update text
        service.UpdateOverlayText(_testControl, updatedText);

        // Act - Show again with different text
        service.ShowOverlay(_testControl, finalText);

        // Act - Hide overlay
        service.HideOverlay(_testControl);

        // Assert
        mockOverlayControl.VerifySet(x => x.OverlayText = initialText, Times.Once);
        mockOverlayControl.VerifySet(x => x.OverlayText = updatedText, Times.Once);
        mockOverlayControl.VerifySet(x => x.OverlayText = finalText, Times.Once);
        mockOverlayControl.Verify(x => x.ShowOverlay(_testControl), Times.Exactly(2));
        mockOverlayControl.Verify(x => x.HideOverlay(true), Times.Once);

        // Cleanup
        service.Dispose();
    }

    #endregion
}
