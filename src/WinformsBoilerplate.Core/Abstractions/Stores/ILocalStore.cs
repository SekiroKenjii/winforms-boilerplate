namespace WinformsBoilerplate.Core.Abstractions.Stores;

public interface ILocalStore : IKeyValueStore
{
    /// <summary>
    /// Cleans up the local data store by removing obsolete or temporary files.
    /// </summary>
    /// <remarks>This method is typically used to free up disk space and ensure the local store remains in a
    /// consistent state.  It should be called periodically or when the application determines that cleanup is
    /// necessary.</remarks>
    void Cleanup();
}
