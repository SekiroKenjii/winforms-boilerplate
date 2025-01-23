using System.Linq.Expressions;

namespace Core.Abstractions.Stores;

/// <summary>
/// Defines a contract for dispatching actions with varying numbers of parameters.
/// </summary>
public interface IDispatchable
{
    /// <summary>
    /// Dispatches a parameterless action for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that contains the action to be dispatched.</typeparam>
    /// <param name="action">An expression that selects the action to be dispatched from the event type.</param>
    void Dispatch<TEvent>(Expression<Func<TEvent, Action?>> action);

    /// <summary>
    /// Dispatches an action with one parameter for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that contains the action to be dispatched.</typeparam>
    /// <typeparam name="TParam">The type of the parameter to be passed to the action.</typeparam>
    /// <param name="action">An expression that selects the action to be dispatched from the event type.</param>
    /// <param name="param">The parameter value to be passed to the action.</param>
    void Dispatch<TEvent, TParam>(Expression<Func<TEvent, Action<TParam>?>> action, TParam param);

    /// <summary>
    /// Dispatches an action with two parameters for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that contains the action to be dispatched.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
    /// <param name="action">An expression that selects the action to be dispatched from the event type.</param>
    /// <param name="param1">The first parameter value to be passed to the action.</param>
    /// <param name="param2">The second parameter value to be passed to the action.</param>
    void Dispatch<TEvent, TParam1, TParam2>(Expression<Func<TEvent, Action<TParam1, TParam2>?>> action, TParam1 param1, TParam2 param2);

    /// <summary>
    /// Dispatches an action with three parameters for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that contains the action to be dispatched.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter.</typeparam>
    /// <param name="action">An expression that selects the action to be dispatched from the event type.</param>
    /// <param name="param1">The first parameter value to be passed to the action.</param>
    /// <param name="param2">The second parameter value to be passed to the action.</param>
    /// <param name="param3">The third parameter value to be passed to the action.</param>
    void Dispatch<TEvent, TParam1, TParam2, TParam3>(Expression<Func<TEvent, Action<TParam1, TParam2, TParam3>?>> action, TParam1 param1, TParam2 param2, TParam3 param3);
}
