namespace WinformsBoilerplate.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Executes the specified action on each element of the enumerable, providing the element and its index.
    /// </summary>
    /// <remarks>
    /// This method efficiently iterates through the enumerable only once, tracking the index internally
    /// while processing each element.
    /// </remarks>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable whose elements the action will be applied to.</param>
    /// <param name="action">The action to perform on each element. The first parameter of the action is the element, and the second
    /// parameter is its zero-based index.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(action);

        int index = 0;

        foreach (T item in enumerable)
        {
            action(item, index++);
        }
    }
}
