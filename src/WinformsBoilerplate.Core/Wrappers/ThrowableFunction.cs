namespace WinformsBoilerplate.Core.Wrappers;

/// <summary>
/// A wrapper class that executes a function and provides type-safe exception handling with a return value.
/// Extends <see cref="Throwable{TException}"/> to handle both the function's result and any caught exceptions.
/// </summary>
/// <typeparam name="T">The return type of the function.</typeparam>
/// <typeparam name="TException">The type of exception to catch and handle. Must derive from <see cref="Exception"/>.</typeparam>
public class ThrowableFunction<TOut, TException> : Throwable<TException>
    where TException : Exception
{
    /// <summary>
    /// Gets the result of the executed function.
    /// </summary>
    /// <value>
    /// The function's result of type <typeparamref name="TOut"/> if execution was successful;
    /// null if an exception occurred or the function returned null.
    /// </value>
    public TOut? Result { get; private set; }

    /// <summary>
    /// Executes a synchronous function with exception handling.
    /// </summary>
    /// <param name="function">The function to execute, which returns a value of type <typeparamref name="TOut"/>.</param>
    /// <returns>A <see cref="ThrowableFunction{TOut, TException}"/> instance containing either the result or the caught exception.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: Sets <see cref="Result"/> to the function's return value</description></item>
    /// <item><description>On caught exception: Stores the exception and sets <see cref="Result"/> to null</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="TException"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static ThrowableFunction<TOut, TException> Run(Func<TOut> function)
    {
        ThrowableFunction<TOut, TException> instance = new();

        try
        {
            instance.Result = function();
        }
        catch (Exception exception) when (exception is TException ex)
        {
            instance.Exception = ex;
            instance.Result = default;
        }

        return instance;
    }

    /// <summary>
    /// Executes an asynchronous function with exception handling.
    /// </summary>
    /// <param name="function">The async function to execute, which returns a <see cref="Task{TOut}"/>.</param>
    /// <returns>A task that resolves to a <see cref="ThrowableFunction{TOut, TException}"/> containing either the result or the caught exception.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: Sets <see cref="Result"/> to the awaited function's return value</description></item>
    /// <item><description>On caught exception: Stores the exception and sets <see cref="Result"/> to null</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="TException"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static async Task<ThrowableFunction<TOut, TException>> RunAsync(Func<Task<TOut>> function)
    {
        ThrowableFunction<TOut, TException> instance = new();

        try
        {
            instance.Result = await function();
        }
        catch (Exception exception) when (exception is TException ex)
        {
            instance.Exception = ex;
            instance.Result = default;
        }

        return instance;
    }
}

/// <summary>
/// A convenience wrapper for <see cref="ThrowableFunction{TOut, TException}"/> that uses <see cref="Exception"/> as the exception type.
/// </summary>
/// <typeparam name="TOut">The return type of the function.</typeparam>
public class ThrowableFunction<TOut> : ThrowableFunction<TOut, Exception>;
