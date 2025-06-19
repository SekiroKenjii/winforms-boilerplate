using System.Text.Json;
using WinformsBoilerplate.Core.Abstractions.Serializers;

namespace WinformsBoilerplate.Infrastructure.Serializer;

public class DefaultSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _options = new() {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IgnoreReadOnlyProperties = true,
        IgnoreReadOnlyFields = true,
        WriteIndented = true
    };

    /// <inheritdoc />
    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _options);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(string data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }
}
