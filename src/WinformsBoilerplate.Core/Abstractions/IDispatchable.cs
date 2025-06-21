using System.Linq.Expressions;

namespace WinformsBoilerplate.Core.Abstractions;

/// <summary>
/// Defines a contract for dispatching actions with varying numbers of parameters.
/// </summary>
public interface IDispatchable
{
    /// <summary>
    /// Dispatches an event by invoking the specified action with no parameters.
    /// </summary>
    /// <param name="action">The lambda expression used to select the action to invoke.</param>
    /// <typeparam name="TEvent">The type of the event containing the action to be invoked.</typeparam>
    /// <remarks>
    /// This method is typically used for events that do not require any parameters.
    /// The <paramref name="action"/> expression must resolve to a valid action within the specified
    /// event type. This method invokes the action, allowing dynamic event dispatching.
    /// </remarks>
    void Dispatch<TEvent>(Expression<Func<TEvent, Action?>> action);

    /// <summary>
    /// Dispatches an event by invoking the specified action with the provided parameters.
    /// </summary>
    /// <param name="action">The lambda expression used to select the action to invoke.</param>
    /// <typeparam name="TEvent">The type of the event containing the action to be invoked.</typeparam>
    /// <remarks>
    /// This method is typically used for events that do not require any parameters.
    /// The <paramref name="action"/> expression must resolve to a valid action within the specified
    /// event type. This method invokes the action, allowing dynamic event dispatching.
    /// </remarks>
    void Dispatch<TEvent>(Expression<Func<TEvent, Func<Task>?>> action);

    /// <summary>
    /// Dispatches an event by invoking the specified action with the provided parameters.
    /// </summary>
    /// <param name="action">The lambda expression used to select the action to invoke.</param>
    /// <param name="param">The parameter to pass to the action.</param>
    /// <typeparam name="TEvent">The type of the event containing the action to be invoked.</typeparam>
    /// <typeparam name="TParam">The type of the parameter to pass to the action.</typeparam>
    /// <remarks>
    /// This method is typically used for events that require a single parameter.
    /// The <paramref name="action"/> expression must resolve to a valid action within the specified
    /// event type. This method invokes the action with the provided parameter, allowing dynamic event
    /// dispatching.
    /// </remarks>
    void Dispatch<TEvent, TParam>(Expression<Func<TEvent, Action<TParam>?>> action, TParam param);

    /// <summary>
    /// Dispatches an event by invoking the specified action with the provided parameters.
    /// </summary>
    /// <param name="action">The lambda expression used to select the action to invoke.</param>
    /// <param name="param1">The first parameter to pass to the action.</param>
    /// <param name="param2">The second parameter to pass to the action.</param>
    /// <typeparam name="TEvent">The type of the event containing the action to be invoked.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter passed to the action.</typeparam>
    /// <remarks>
    /// This method is typically used for events that require two parameters.
    /// /// The <paramref name="action"/> expression must resolve to a valid action within the specified
    /// event type. This method invokes the action with the provided parameters, allowing dynamic event
    /// dispatching.
    /// </remarks>
    void Dispatch<TEvent, TParam1, TParam2>(Expression<Func<TEvent, Action<TParam1, TParam2>?>> action, TParam1 param1, TParam2 param2);

    /// <summary>
    /// Dispatches an event by invoking the specified action with the provided parameters.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event containing the action to be invoked.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter passed to the action.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter passed to the action.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter passed to the action.</typeparam>
    /// <param name="action">An expression specifying the action to invoke within the event.</param>
    /// <param name="param1">The first parameter to pass to the action.</param>
    /// <param name="param2">The second parameter to pass to the action.</param>
    /// <param name="param3">The third parameter to pass to the action.</param>
    /// <remarks>
    /// The <paramref name="action"/> expression must resolve to a valid action within the specified
    /// event type. This method invokes the action with the provided parameters, allowing dynamic event
    /// dispatching.
    /// </remarks>
    void Dispatch<TEvent, TParam1, TParam2, TParam3>(Expression<Func<TEvent, Action<TParam1, TParam2, TParam3>?>> action, TParam1 param1, TParam2 param2, TParam3 param3);
}
