using Core.Wrappers;

namespace Core.Extensions;

/// <summary>
/// Provides extension methods for handling exceptions in <see cref="ThrowableAction{E}"/> and <see cref="ThrowableFunction{T, E}"/> instances.
/// These operators allow for controlled exception handling and result retrieval from throwable wrappers.
/// </summary>
public static class ThrowableOperators
{
    /// <summary>
    /// Handles any exception stored in a <see cref="ThrowableAction{E}"/> instance, providing control over exception handling behavior.
    /// </summary>
    /// <typeparam name="E">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwable">The throwable wrapper containing the potential exception.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <exception cref="E">Thrown when an exception exists in the throwable and no exception handler is provided.</exception>
    /// <remarks>
    /// This method provides three different behaviors based on the state of the throwable and the provided handler:
    /// <list type="bullet">
    /// <item>
    ///     <description>If no exception occurred (Exception is null): Method returns without any action.</description>
    /// </item>
    /// <item>
    ///     <description>If an exception exists and no handler is provided: The original exception is rethrown.</description>
    /// </item>
    /// <item>
    ///     <description>If an exception exists and a handler is provided: The handler is invoked with the exception.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static void Catch<E>(this ThrowableAction<E> throwable, Action<E>? exceptionAction = null) where E : Exception
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
    /// Handles any exception stored in a <see cref="ThrowableFunction{T, E}"/> instance and returns the function's result.
    /// </summary>
    /// <typeparam name="T">The type of the function's result, which must be a reference type.</typeparam>
    /// <typeparam name="E">The type of exception that may have been caught, which must derive from <see cref="Exception"/>.</typeparam>
    /// <param name="throwable">The throwable wrapper containing the potential exception and result.</param>
    /// <param name="exceptionAction">An optional delegate to handle the exception if one was caught.
    /// If not provided and an exception exists, the exception will be rethrown.</param>
    /// <returns>
    /// The result of the wrapped function. If no exception occurred, returns the actual result;
    /// if an exception was caught and handled, returns the default result (which may be null).
    /// </returns>
    /// <exception cref="E">Thrown when an exception exists in the throwable and no exception handler is provided.</exception>
    /// <remarks>
    /// This method provides three different behaviors based on the state of the throwable and the provided handler:
    /// <list type="bullet">
    /// <item>
    ///     <description>If no exception occurred (Exception is null): Returns the successful result from the function.</description>
    /// </item>
    /// <item>
    ///     <description>If an exception exists and no handler is provided: The original exception is rethrown.</description>
    /// </item>
    /// <item>
    ///     <description>If an exception exists and a handler is provided: The handler is invoked with the exception,
    ///     and the method returns the default result (which may be null).</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static T? Catch<T, E>(this ThrowableFunction<T, E> throwable, Action<E>? exceptionAction = null)
        where T : class
        where E : Exception
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
}
