namespace WinformsBoilerplate.Core.Wrappers;

/// <summary>
/// A generic wrapper class that encapsulates an exception of a specific type.
/// Provides a foundation for type-safe exception handling and propagation.
/// </summary>
/// <typeparam name="TException">The type of exception to be wrapped. Must inherit from <see cref="Exception"/>.</typeparam>
/// <remarks>
/// This class serves as a base class for specialized throwable wrappers such as:
/// <list type="bullet">
/// <item><description><see cref="ThrowableAction{E}"/> for wrapping void operations</description></item>
/// <item><description><see cref="ThrowableFunction{T, E}"/> for wrapping operations that return values</description></item>
/// </list>
/// </remarks>
public class Throwable<TException> where TException : Exception
{
    /// <summary>
    /// Gets or sets the exception that was captured during the execution of a wrapped operation.
    /// </summary>
    /// <value>
    /// The captured exception of type <typeparamref name="TException"/>, or null if no exception occurred.
    /// </value>
    public TException? Exception { get; set; }
}
