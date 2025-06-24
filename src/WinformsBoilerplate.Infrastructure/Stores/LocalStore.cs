using System.Collections.Concurrent;
using System.Text.Json;
using WinformsBoilerplate.Core.Abstractions.Services;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Extensions;
using WinformsBoilerplate.Core.Helpers;
using WinformsBoilerplate.Core.Wrappers;

namespace WinformsBoilerplate.Infrastructure.Stores;

public class LocalStore : ILocalStore
{
    private readonly ILogService _logService;
    private readonly ISystemService _systemService;
    private readonly ConcurrentDictionary<string, object?> _store;

    public LocalStore(ILogService logService, ISystemService systemService)
    {
        _logService = logService;
        _systemService = systemService;

        _store = _systemService.ReadLocalStore();
    }

    public void Set<T>(string key, T value)
    {
        _ = _store.AddOrUpdate(key, value, (_, _) => value);

        _systemService.SaveLocalStore(_store);
    }

    public void Clear()
    {
        _store.Clear();

        _systemService.SaveLocalStore(_store);
    }

    public T? Get<T>(string key)
    {
        object? value = _store.GetValueOrDefault(key);

        if (value is JsonElement el)
        {
            Type type = typeof(T);
            string typeName = type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Nullable.GetUnderlyingType(type)?.Name ?? ""
                : type.Name;

            switch (typeName)
            {
                case "Int32":
                    _ = el.TryGetInt32(out int integer);
                    return integer as dynamic;
                case "String":
                    return el.GetString() as dynamic;
                case "DateTime":
                    _ = el.TryGetDateTime(out DateTime dateTime);
                    return dateTime as dynamic;
                default:
                    return default;
            }
        }

        return value is not T result ? default : result;
    }

    public void Remove(string key)
    {
        _ = _store.Remove(key, out object? _);

        _systemService.SaveLocalStore(_store);
    }

    public void Cleanup()
    {
        // Implement cleanup logic here
        _logService.Info("Cleaning up local store...");

        ThrowableAction
            .Run(() => {
                _store.Clear();

                string localStorePath = CommonHelpers.AppStartupPath();

                if (Directory.Exists(localStorePath))
                {
                    Directory.Delete(localStorePath, true);
                    _logService.Info("Local store cleaned successfully.");

                    return;
                }

                _logService.Warn("Local store directory does not exist. No cleanup needed.");
            })
            .Catch(ex => _logService.Error($"Error during local store cleanup: {ex.ToFormattedString()}"));
    }
}
