namespace WinformsBoilerplate.Core.Extensions;

/// <summary>
/// Provides extension methods for working with spans.
/// </summary>
public static class SpanExtensions
{
    /// <summary>
    /// Performs the specified action on each element of the span.
    /// The action receives both the element and its index in the span.
    /// </summary>
    /// <param name="span">The span to iterate over.</param>
    /// <param name="action">The action to perform on each element.</param>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    public static void ForEach<T>(this Span<T> span, Action<T, int> action)
    {
        for (int i = 0; i < span.Length; i++)
        {
            action(span[i], i);
        }
    }
}
