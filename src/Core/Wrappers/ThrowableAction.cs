namespace Core.Wrappers;

/// <summary>
/// A wrapper class that executes an action and provides type-safe exception handling.
/// This class extends <see cref="Throwable{E}"/> to provide action-specific exception handling.
/// </summary>
/// <typeparam name="E">The type of exception to catch and handle. Must derive from <see cref="Exception"/>.</typeparam>
/// <remarks>
/// Used for wrapping void operations where only exception handling is needed, without returning a value.
/// Any exceptions not of type <typeparamref name="E"/> will propagate normally through the call stack.
/// </remarks>
public class ThrowableAction<E> : Throwable<E> where E : Exception
{
    /// <summary>
    /// Gets a singleton instance of <see cref="ThrowableAction{E}"/>.
    /// </summary>
    /// <value>
    /// A new instance of <see cref="ThrowableAction{E}"/>.
    /// </value>
    /// <remarks>
    /// Creates a new instance per access to ensure thread safety when handling multiple operations.
    /// This prevents potential race conditions in multi-threaded scenarios.
    /// </remarks>
    private static ThrowableAction<E> Instance => new();

    /// <summary>
    /// Executes the specified action and catches any exception of type <typeparamref name="E"/>.
    /// </summary>
    /// <param name="action">The action to execute. This delegate encapsulates a method that takes no parameters and returns no value.</param>
    /// <returns>An instance of <see cref="ThrowableAction{E}"/> containing the caught exception, if any.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: The action executes normally with no stored exception</description></item>
    /// <item><description>On caught exception: Stores the exception of type <typeparamref name="E"/> for later handling</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="E"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static ThrowableAction<E> Run(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception) when (exception is E ex)
        {
            Instance.Exception = ex;
        }

        return Instance;
    }
}
