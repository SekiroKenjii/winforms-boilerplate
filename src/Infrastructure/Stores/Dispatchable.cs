using Core.Abstractions.Stores;
using Core.Extensions;
using Core.Wrappers;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Stores;

/// <summary>
/// Provides a base implementation for dispatching actions with varying numbers of parameters.
/// </summary>
public abstract class Dispatchable : IDispatchable
{
    /// <inheritdoc/>
    public void Dispatch<TEvent>(Expression<Func<TEvent, Action?>> action)
    {
        ThrowableAction<Exception>.Run(() => {
            Action? eventAction = GetEventAction<Action>(GetActionName(action));
            eventAction?.Invoke();
        }).Catch();
    }

    /// <inheritdoc/>
    public void Dispatch<TEvent, TParam>(Expression<Func<TEvent, Action<TParam>?>> action, TParam param)
    {
        ThrowableAction<Exception>.Run(() => {
            Action<TParam>? eventAction = GetEventAction<Action<TParam>>(GetActionName(action));
            eventAction?.Invoke(param);
        }).Catch();
    }

    /// <inheritdoc/>
    public void Dispatch<TEvent, TParam1, TParam2>(
        Expression<Func<TEvent, Action<TParam1, TParam2>?>> action,
        TParam1 param1,
        TParam2 param2)
    {
        ThrowableAction<Exception>.Run(() => {
            Action<TParam1, TParam2>? eventAction =
                GetEventAction<Action<TParam1, TParam2>>(GetActionName(action));
            eventAction?.Invoke(param1, param2);
        }).Catch();
    }

    /// <inheritdoc/>
    public void Dispatch<TEvent, TParam1, TParam2, TParam3>(
        Expression<Func<TEvent, Action<TParam1, TParam2, TParam3>?>> action,
        TParam1 param1,
        TParam2 param2,
        TParam3 param3)
    {
        ThrowableAction<Exception>.Run(() => {
            Action<TParam1, TParam2, TParam3>? eventAction =
                GetEventAction<Action<TParam1, TParam2, TParam3>>(GetActionName(action));
            eventAction?.Invoke(param1, param2, param3);
        }).Catch();
    }

    /// <summary>
    /// Gets the target object associated with the specified action name.
    /// </summary>
    /// <param name="action">The name of the action to retrieve the target for.</param>
    /// <returns>The target object, or null if not found.</returns>
    protected abstract object? GetTarget(string action);

    /// <summary>
    /// Extracts the action name from an expression that represents a property access.
    /// </summary>
    /// <typeparam name="TEvent">The type of event that contains the action.</typeparam>
    /// <typeparam name="TAction">The type of the action delegate.</typeparam>
    /// <param name="action">The expression representing the property access.</param>
    /// <returns>The name of the accessed property.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the expression does not represent a valid property access.</exception>
    private static string GetActionName<TEvent, TAction>(Expression<Func<TEvent, TAction>> action)
    {
        var actionInfo = (action.Body as MemberExpression)?.Member as PropertyInfo;
        ArgumentNullException.ThrowIfNull(actionInfo);

        return actionInfo.Name;
    }

    /// <summary>
    /// Retrieves an action delegate from the target object using reflection.
    /// </summary>
    /// <typeparam name="TAction">The type of the action delegate to retrieve.</typeparam>
    /// <param name="action">The name of the action to retrieve.</param>
    /// <returns>The action delegate, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the action name is null, or when no target is found for the action.</exception>
    private TAction? GetEventAction<TAction>(string action)
    {
        ArgumentNullException.ThrowIfNull(action);
        object? target = GetTarget(action);
        ArgumentNullException.ThrowIfNull(target);
        PropertyInfo? property = target.GetType().FindPropertyInfoByName(action, ignoreCase: true);

        return (TAction?)property?.GetValue(target);
    }
}
