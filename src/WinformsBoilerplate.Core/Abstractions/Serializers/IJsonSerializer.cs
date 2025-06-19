namespace WinformsBoilerplate.Core.Abstractions.Serializers;

/// <summary>
/// Represents a JSON serializer interface.
/// </summary>
public interface IJsonSerializer
{
    /// <summary>
    /// Serializes the specified object into a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize. Cannot be null.</param>
    /// <returns>A string representation of the serialized object.</returns>
    string Serialize<T>(T obj);

    /// <summary>
    /// Deserializes the specified JSON string into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>Ensure that the JSON string matches the structure of the type <typeparamref name="T"/>  to
    /// avoid deserialization errors. This method may return <see langword="null"/> if the  JSON string is malformed or
    /// incompatible with the target type.</remarks>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="data">The JSON string to deserialize. Cannot be null or empty.</param>
    /// <returns>An instance of type <typeparamref name="T"/> populated with data from the JSON string,  or <see
    /// langword="null"/> if the deserialization fails or the input is invalid.</returns>
    T? Deserialize<T>(string data);
}
