using System.Collections.Concurrent;
using WinformsBoilerplate.Core.Abstractions.Stores;

namespace WinformsBoilerplate.Infrastructure.Stores;

public class SessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, object?> _store = [];

    /// <inheritdoc cref="IKeyValueStore.Set{T}(string, T)" />
    public void Set<T>(string key, T value)
    {
        _ = _store.AddOrUpdate(key, value, (_, _) => value);
    }

    /// <inheritdoc cref="IKeyValueStore.Clear" />
    public void Clear()
    {
        _store.Clear();
    }

    /// <inheritdoc cref="IKeyValueStore.Get{T}(string)" />
    public T? Get<T>(string key)
    {
        object? value = _store.GetValueOrDefault(key);

        return value is not T result ? default : result;
    }

    /// <inheritdoc cref="IKeyValueStore.Remove(string)" />
    public void Remove(string key)
    {
        _ = _store.Remove(key, out object? _);
    }

    /// <inheritdoc cref="ISessionStore.Inc(string, int)" />
    public void Inc(string key, int value = 1)
    {
        int currentCommitCount = Get<int>(key);

        Set(key, currentCommitCount + value);
    }

    /// <inheritdoc cref="ISessionStore.Dec(string, int)" />
    public void Dec(string key, int value = 1)
    {
        int currentCommitCount = Get<int>(key);

        Set(key, currentCommitCount - value);
    }
}
