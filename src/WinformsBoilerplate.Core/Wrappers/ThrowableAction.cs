namespace WinformsBoilerplate.Core.Wrappers;

/// <summary>
/// A wrapper class that executes an action and provides type-safe exception handling.
/// This class extends <see cref="Throwable{TException}"/> to provide action-specific exception handling.
/// </summary>
/// <typeparam name="TException">The type of exception to catch and handle. Must derive from <see cref="Exception"/>.</typeparam>
/// <remarks>
/// Used for wrapping void operations where only exception handling is needed, without returning a value.
/// Any exceptions not of type <typeparamref name="TException"/> will propagate normally through the call stack.
/// </remarks>
public class ThrowableAction<TException> : Throwable<TException> where TException : Exception
{
    /// <summary>
    /// Executes the specified action and catches any exception of type <typeparamref name="TException"/>.
    /// </summary>
    /// <param name="action">The action to execute. This delegate encapsulates a method that takes no parameters and returns no value.</param>
    /// <returns>An instance of <see cref="ThrowableAction{TException}"/> containing the caught exception, if any.</returns>
    /// <remarks>
    /// Execution behavior:
    /// <list type="bullet">
    /// <item><description>On success: The action executes normally with no stored exception</description></item>
    /// <item><description>On caught exception: Stores the exception of type <typeparamref name="TException"/> for later handling</description></item>
    /// <item><description>Only catches exceptions of type <typeparamref name="TException"/>; others propagate normally</description></item>
    /// </list>
    /// </remarks>
    public static ThrowableAction<TException> Run(Action action)
    {
        ThrowableAction<TException> instance = new();

        try
        {
            action();
        }
        catch (Exception exception) when (exception is TException ex)
        {
            instance.Exception = ex;
        }

        return instance;
    }

    public static async Task<ThrowableAction<TException>> RunAsync(Func<Task> action)
    {
        ThrowableAction<TException> instance = new();

        try
        {
            await action();
        }
        catch (Exception exception) when (exception is TException ex)
        {
            instance.Exception = ex;
        }

        return instance;
    }
}

/// <summary>
/// A convenience class for wrapping actions that throw exceptions of type <see cref="Exception"/>.
/// </summary>
public class ThrowableAction : ThrowableAction<Exception>;
