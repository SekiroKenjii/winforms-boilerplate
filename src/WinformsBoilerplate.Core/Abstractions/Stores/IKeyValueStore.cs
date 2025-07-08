namespace WinformsBoilerplate.Core.Abstractions.Stores;

/// <summary>
/// Represents a key-value store that supports storing, retrieving, incrementing, decrementing, and removing values.
/// </summary>
public interface IKeyValueStore : IDisposable, ISingletonDependency
{
    /// <summary>
    /// Stores a value with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The unique key used to identify the saved value. Cannot be null or empty.</param>
    /// <param name="value">The value to store. If the value is null, the key will be removed from the store.</param>
    void Set<T>(string key, T value);

    /// <summary>
    /// Retrieves the value associated with the specified key and attempts to cast it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be cast.</typeparam>
    /// <param name="key">The unique key used to identify the saved value. Cannot be null or empty.</param>
    /// <returns>The value associated with the specified key, cast to the specified type, or <see langword="null"/> if the key
    /// does not exist or the value cannot be cast to the specified type.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Removes the item associated with the specified key from the store.
    /// </summary>
    /// <remarks>If the specified key does not exist in the store, no action is taken.</remarks>
    /// <param name="key">The key of the item to remove. Cannot be <see langword="null"/> or empty.</param>
    void Remove(string key);

    /// <summary>
    /// Removes all elements from the store, leaving it empty.
    /// </summary>
    /// <remarks>After calling this method, the collection will contain no elements, and its count will be
    /// reset to zero. This operation may affect any iterators currently in use, which will become invalid.</remarks>
    void Clear();
}
