namespace WinformsBoilerplate.Core.Abstractions.Stores;

public interface IKeyValueStore
{
    void Set<T>(string key, T value);
    T? Get<T>(string key);
    void Inc(string key, int value = 1);
    void Dec(string key, int value = 1);
    void Remove(string key);
    void Clear();
}
