using System.Collections.Concurrent;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Stores;

namespace WinformsBoilerplate.Infrastructure.Stores;

public class SessionStore : Disposable, ISessionStore
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

    /// <inheritdoc cref="Disposable.Dispose(bool)" />
    protected override void Dispose(bool disposing)
    {
        if (!disposing || Disposed)
        {
            return;
        }

        foreach (string key in _store.Keys)
        {
            _store[key] = null;
        }

        _store.Clear();

        base.Dispose(disposing);
    }
}
