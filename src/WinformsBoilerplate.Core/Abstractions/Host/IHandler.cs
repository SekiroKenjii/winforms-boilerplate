namespace WinformsBoilerplate.Core.Abstractions.Host;

/// <summary>
/// Defines a contract for handlers that can be invoked by the host.
/// Handlers are typically used to perform initialization or setup tasks within the application.
/// </summary>
public interface IHandler
{
    /// <summary>
    /// Invokes the handler with the provided service provider.
    /// </summary>
    /// <param name="sp">The service provider instance.</param>
    /// <remarks>
    /// This method is called by the host to execute the handler's logic.
    /// Handlers can use the service provider to resolve dependencies and perform their tasks.
    /// </remarks>
    void Invoke(IServiceProvider sp);
}
