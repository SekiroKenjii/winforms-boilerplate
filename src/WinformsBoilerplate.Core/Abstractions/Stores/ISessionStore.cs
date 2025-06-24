namespace WinformsBoilerplate.Core.Abstractions.Stores;

public interface ISessionStore : IKeyValueStore
{
    /// <summary>
    /// Increments the value associated with the specified key by the given amount.
    /// </summary>
    /// <remarks>If the key does not exist, a new entry may be created with the specified increment value,
    /// depending on the implementation.</remarks>
    /// <param name="key">The key whose associated value will be incremented. Cannot be null or empty.</param>
    /// <param name="value">The amount by which to increment the value. Defaults to <see langword="1"/>.</param>
    void Inc(string key, int value = 1);

    /// <summary>
    /// Decrements the value associated with the specified key by the given amount.
    /// </summary>
    /// <remarks>If the key does not exist, behavior may depend on the implementation. Ensure the key is valid
    /// and initialized before calling this method.</remarks>
    /// <param name="key">The key whose associated value will be decremented. Cannot be null or empty.</param>
    /// <param name="value">The amount by which to decrement the value. Defaults to 1 if not specified.</param>
    void Dec(string key, int value = 1);
}
