namespace WinformsBoilerplate.Core.Abstractions;

/// <summary>
/// Provides a base class for implementing the <see cref="IDisposable"/> interface, ensuring proper resource cleanup.
/// </summary>
/// <remarks>
/// The <see cref="Disposable"/> class simplifies the implementation of the <see cref="IDisposable"/>
/// pattern by providing a virtual <see cref="Dispose(bool)"/> method that can be overridden in derived classes to
/// release additional resources. It ensures that resources are disposed only once, preventing redundant disposal
/// operations. Derived classes should override <see cref="Dispose(bool)"/> to release both managed and unmanaged
/// resources.
/// </remarks>
public abstract class Disposable : IDisposable
{
    private bool _disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the resources used by the current instance.
    /// </summary>
    /// <remarks>
    /// This method is called to free both managed and unmanaged resources. Override this method in
    /// a derived class to release additional resources. Ensure that the method is called only once to avoid redundant
    /// disposal.
    /// </remarks>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only
    /// unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _disposed)
        {
            return;
        }

        _disposed = true;
    }
}
