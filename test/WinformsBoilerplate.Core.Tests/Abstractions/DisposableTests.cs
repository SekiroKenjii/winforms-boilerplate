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
        Assert.True(disposable.IsDisposed);

    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var disposable = new TestDisposable();

        // Act & Assert
        disposable.Dispose();
        var exception = Record.Exception(() => disposable.Dispose());
        Assert.Null(exception);
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
        Assert.Equal(1, disposable.DisposeCallCount);
    }

    [Fact]
    public void Disposed_InitiallyFalse_ShouldReturnFalse()
    {
        // Arrange & Act
        var disposable = new TestDisposable();

        // Assert
        Assert.False(disposable.IsDisposed);
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
