using Core.Abstractions.Stores;
using Core.Extensions;
using Core.Structs;
using System.Reflection;

namespace Infrastructure.Stores;

/// <summary>
/// Implements event storage and management functionality, handling the subscription and cleanup of event handlers.
/// Maintains weak references to target objects to prevent memory leaks.
/// </summary>
public class EventStore : Dispatchable, IEventStore
{
    /// <summary>
    /// Internal storage for event subscriptions. Maps action names to tuples containing weak references of target objects
    /// and their corresponding event handlers.
    /// </summary>
    private readonly Dictionary<string, (WeakReference<object>, Delegate)> _store = [];

    /// <inheritdoc/>
    public void Add(object target, IEnumerable<TeardownLogic> teardownLogics)
    {
        foreach (TeardownLogic teardownLogic in teardownLogics)
        {
            Subscribe(target, teardownLogic);
            _store.Add(teardownLogic.ActionName, (new WeakReference<object>(target), teardownLogic.EventHandler));
        }
    }

    /// <inheritdoc/>
    public void Flush(object? target = null)
    {
        if (target != null)
        {
            string[] keys = [.. _store.Keys];

            for (int i = keys.Length - 1; i >= 0; i--)
            {
                string scopeName = keys[i];
                (WeakReference<object> scopeTargetRef, Delegate scopeEventHandler) = _store[scopeName];

                if (scopeTargetRef.TryGetTarget(out object? scopeTarget) && ReferenceEquals(scopeTarget, target))
                {
                    Unsubscribe(scopeName, scopeTarget, scopeEventHandler);
                    _store.Remove(scopeName);
                }
            }

            return;
        }

        foreach ((string name, (WeakReference<object> storeTargetRef, Delegate storeEventHandler)) in _store)
        {
            if (storeTargetRef.TryGetTarget(out object? storeTarget))
            {
                Unsubscribe(name, storeTarget, storeEventHandler);
            }
        }

        _store.Clear();
    }

    /// <inheritdoc/>
    protected override object? GetTarget(string action)
    {
        (WeakReference<object> targetRef, _) = _store[action];
        _ = targetRef.TryGetTarget(out object? target);

        return target;
    }

    /// <summary>
    /// Subscribes an event handler to a target object's event.
    /// </summary>
    /// <param name="target">The object containing the event to subscribe to.</param>
    /// <param name="teardownLogic">The teardown logic containing the event name and handler to subscribe.</param>
    /// <exception cref="ArgumentException">Thrown when the specified event name is not found on the target object.</exception>
    private static void Subscribe(object target, TeardownLogic teardownLogic)
    {
        (string name, Delegate eventHandler) = teardownLogic;
        Type targetType = target.GetType();
        PropertyInfo property = targetType.FindPropertyInfoByName(name, ignoreCase: true)
            ?? throw new ArgumentException($"Request {name} not found on target of type {targetType.Name}");

        var targetAction = property.GetValue(target) as Delegate;
        var combineEventHandler = Delegate.Combine(targetAction, eventHandler);
        property.SetValue(target, combineEventHandler);
    }

    /// <summary>
    /// Unsubscribes an event handler from a target object's event.
    /// </summary>
    /// <param name="name">The name of the event to unsubscribe from.</param>
    /// <param name="target">The object containing the event.</param>
    /// <param name="eventHandler">The event handler to remove from the event.</param>
    private static void Unsubscribe(string name, object target, Delegate eventHandler)
    {
        PropertyInfo? property = target.GetType().FindPropertyInfoByName(name, ignoreCase: true);

        if (property == null)
        {
            return;
        }

        var targetAction = property.GetValue(target) as Delegate;
        var removeEventHandler = Delegate.Remove(targetAction, eventHandler);
        property.SetValue(target, removeEventHandler);
    }
}
