using WinformsBoilerplate.Core.Wrappers;

namespace WinformsBoilerplate.Core.Extensions;

/// <summary>
/// Provides extension methods for handling exceptions in <see cref="ThrowableAction{TException}"/> and <see cref="ThrowableFunction{TOut, TException}"/> instances.
/// These operators allow for controlled exception handling and result retrieval from throwable wrappers.
/// </summary>
public static class ThrowableOperators
{
    /// <summary>
    /// Handles any exception stored in a <see cref="ThrowableAction{TException}"/> instance, providing control over exception handling behavior.
    /// </summary>
    /// <typeparam name="TException">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwable">The throwable wrapper containing the potential exception.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <exception>Thrown when an exception exists in the throwable and no exception handler is provided.</exception>
    public static void Catch<TException>(this ThrowableAction<TException> throwable, Action<TException>? exceptionAction = null) where TException : Exception
    {
        if (throwable.Exception == null)
        {
            return;
        }

        if (exceptionAction == null)
        {
            throw throwable.Exception;
        }

        exceptionAction(throwable.Exception);
    }

    /// <summary>
    /// Handles any exception stored in a <see cref="ThrowableFunction{TOut, TException}"/> instance and returns the function's result.
    /// </summary>
    /// <typeparam name="TOut">The type of the function's result</typeparam>
    /// <typeparam name="TException">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwable">The throwable wrapper containing the potential exception and result.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <returns>
    /// The result of the wrapped function. If no exception occurred, returns the actual result;
    /// if an exception was caught and handled, returns the default result (which may be null).
    /// </returns>
    /// <exception>Thrown when an exception exists in the throwable and no exception handler is provided.</exception>
    public static TOut? Catch<TOut, TException>(this ThrowableFunction<TOut, TException> throwable, Action<TException>? exceptionAction = null)
        where TException : Exception
    {
        if (throwable.Exception == null)
        {
            return throwable.Result;
        }

        if (exceptionAction == null)
        {
            throw throwable.Exception;
        }

        exceptionAction(throwable.Exception);

        return throwable.Result;
    }

    /// <summary>
    /// Catches exceptions from a task that returns a <see cref="ThrowableAction{TException}"/> and executes an action if an exception occurs.
    /// If no exception occurs, it simply returns.
    /// </summary>
    /// <typeparam name="TException">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwableTask">The task to await.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task CatchAsync<TException>(this Task<ThrowableAction<TException>> throwableTask, Action<TException>? exceptionAction = null) where TException : Exception
    {
        ThrowableAction<TException> result = await throwableTask;

        if (result.Exception == null)
        {
            return;
        }

        if (exceptionAction == null)
        {
            throw result.Exception;
        }

        exceptionAction(result.Exception);
    }

    /// <summary>
    /// Catches exceptions from a task that returns a <see cref="ThrowableFunction{TOut, TException}"/> and executes an action if an exception occurs.
    /// If no exception occurs, it returns the result of the function.
    /// </summary>
    /// <typeparam name="TOut">The type of the function's result</typeparam>
    /// <typeparam name="TException">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwableTask">The task to await.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <returns>The result of the wrapped function. If no exception occurred, returns the actual result;
    /// if an exception was caught and handled, returns the default result (which may be null).</returns>
    public static async Task<TOut?> CatchAsync<TOut, TException>(this Task<ThrowableFunction<TOut, TException>> throwableTask, Action<TException>? exceptionAction = null)
        where TException : Exception
    {
        ThrowableFunction<TOut, TException> result = await throwableTask;

        if (result.Exception == null)
        {
            return result.Result;
        }

        if (exceptionAction == null)
        {
            throw result.Exception;
        }

        exceptionAction(result.Exception);

        return result.Result;
    }
}
