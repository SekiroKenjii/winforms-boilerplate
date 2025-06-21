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

    /// <summary>
    /// Executes the specified action for each element in the <see cref="ReadOnlySpan{T}"/>,  providing both the element
    /// and its index.
    /// </summary>
    /// <remarks>This method iterates over the elements of the <see cref="ReadOnlySpan{T}"/> in order, 
    /// invoking the specified action for each element. The action receives both the element and  its zero-based
    /// index.</remarks>
    /// <typeparam name="T">The type of the elements in the <see cref="ReadOnlySpan{T}"/>.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> containing the elements to iterate over.</param>
    /// <param name="action">The action to execute for each element. The first parameter of the action is the element,  and the second
    /// parameter is its index within the span.</param>
    public static void ForEach<T>(this ReadOnlySpan<T> span, Action<T, int> action)
    {
        for (int i = 0; i < span.Length; i++)
        {
            action(span[i], i);
        }
    }
}
