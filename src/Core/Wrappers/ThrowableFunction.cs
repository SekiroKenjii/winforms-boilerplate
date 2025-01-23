namespace Core.Wrappers;

/// <summary>
/// A wrapper class that executes a function and provides type-safe exception handling with a return value.
/// Extends <see cref="Throwable{E}"/> to handle both the function's result and any caught exceptions.
/// </summary>
/// <typeparam name="T">The return type of the function. Must be a reference type.</typeparam>
/// <typeparam name="E">The type of exception to catch and handle. Must derive from <see cref="Exception"/>.</typeparam>
public class ThrowableFunction<T, E> : Throwable<E>
    where T : class
    where E : Exception
{
    /// <summary>
    /// Gets a singleton instance of <see cref="ThrowableFunction{T, E}"/>.
    /// </summary>
    /// <value>
    /// A new instance of <see cref="ThrowableFunction{T, E}"/>.
    /// </value>
    /// <remarks>
    /// Creates a new instance per access to ensure thread safety when handling multiple operations.
    /// This prevents potential race conditions in multi-threaded scenarios.
    /// </remarks>
    private static ThrowableFunction<T, E> Instance => new();

    /// <summary>
    /// Gets the result of the executed function.
    /// </summary>
    /// <value>
    /// The function's result of type <typeparamref name="T"/> if execution was successful;
    /// null if an exception occurred or the function returned null.
    /// </value>
    public T? Result { get; private set; }

    /// <summary>
    /// Executes a synchronous function with exception handling.
    /// </summary>
    /// <param name="function">The function to execute, which returns a value of type <typeparamref name="T"/>.</param>
    /// <returns>A <see cref="ThrowableFunction{T, E}"/> instance containing either the result or the caught exception.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: Sets <see cref="Result"/> to the function's return value</description></item>
    /// <item><description>On caught exception: Stores the exception and sets <see cref="Result"/> to null</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="E"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static ThrowableFunction<T, E> Run(Func<T> function)
    {
        try
        {
            Instance.Result = function();
        }
        catch (Exception exception) when (exception is E ex)
        {
            Instance.Exception = ex;
            Instance.Result = default;
        }

        return Instance;
    }

    /// <summary>
    /// Executes an asynchronous function with exception handling.
    /// </summary>
    /// <param name="function">The async function to execute, which returns a <see cref="Task{T}"/>.</param>
    /// <returns>A task that resolves to a <see cref="ThrowableFunction{T, E}"/> containing either the result or the caught exception.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: Sets <see cref="Result"/> to the awaited function's return value</description></item>
    /// <item><description>On caught exception: Stores the exception and sets <see cref="Result"/> to null</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="E"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static async Task<ThrowableFunction<T, E>> Run(Func<Task<T>> function)
    {
        try
        {
            Instance.Result = await function();
        }
        catch (Exception exception) when (exception is E ex)
        {
            Instance.Exception = ex;
            Instance.Result = default;
        }

        return Instance;
    }
}
