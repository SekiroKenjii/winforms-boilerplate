namespace WinformsBoilerplate.Core.Tests.Abstractions;

public class DisposableTests
{
    [Fact]
    public void Dispose_WhenCalled_ShouldSetDisposedToTrue()
    {
        // Arrange
        var disposable = new TestDisposable();

        // Act
        disposable.Dispose();

        // Assert
        disposable.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var disposable = new TestDisposable();

        // Act & Assert
        disposable.Dispose();
        var act = () => disposable.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_ShouldOnlyDisposeOnce()
    {
        // Arrange
        var disposable = new TestDisposable();

        // Act
        disposable.Dispose();
        disposable.Dispose();
        disposable.Dispose();

        // Assert
        disposable.DisposeCallCount.Should().Be(1);
    }

    [Fact]
    public void Disposed_InitiallyFalse_ShouldReturnFalse()
    {
        // Arrange & Act
        var disposable = new TestDisposable();

        // Assert
        disposable.IsDisposed.Should().BeFalse();
    }

    private class TestDisposable : Disposable
    {
        public int DisposeCallCount { get; private set; }
        public bool IsDisposed => Disposed;

        protected override void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                DisposeCallCount++;
            }

            base.Dispose(disposing);
        }
    }
}
