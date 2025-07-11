using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WinformsBoilerplate.Core.Abstractions;
using WinformsBoilerplate.Core.Abstractions.Components;
using WinformsBoilerplate.Core.Abstractions.Stores;
using WinformsBoilerplate.Core.Entities.Systems;
using WinformsBoilerplate.Core.Extensions;

namespace WinformsBoilerplate.Infrastructure.Stores;

public class EventStore(IServiceProvider sp) : Dispatchable, IEventStore
{
    /// <summary>
    /// Internal storage for event subscriptions. Maps action names to tuples containing weak references of target objects
    /// and their corresponding event handlers.
    /// </summary>
    private readonly Dictionary<string, object> _store = [];

    /// <inheritdoc cref="IEventStore.Add{T}(ReadOnlySpan{TeardownLogic})" />
    public void Add<T>(params ReadOnlySpan<TeardownLogic> teardownLogics) where T : IComponentEvent
    {
        T target = sp.GetRequiredService<T>();

        foreach (TeardownLogic teardownLogic in teardownLogics)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(teardownLogic.ActionName);
            ArgumentNullException.ThrowIfNull(teardownLogic.EventHandler);

            Subscribe(target, teardownLogic);

            _store.Add(teardownLogic.ActionName, target);
        }
    }

    /// <inheritdoc cref="IEventStore.Add{T}(string, Delegate)" />
    public void Add<T>(string action, Delegate eventHandler) where T : IComponentEvent
    {
        T target = sp.GetRequiredService<T>();

        Subscribe(target, action, eventHandler);

        _store.Add(action, target);
    }

    /// <inheritdoc cref="IEventStore.Flush{T}()" />
    public void Flush<T>() where T : IComponentEvent
    {
        string[] keys = [.. _store.Keys];

        for (int i = keys.Length - 1; i >= 0; i--)
        {
            string scopeName = keys[i];
            object target = _store[scopeName];

            if (target.GetType() != typeof(T))
            {
                continue;
            }

            Unsubscribe(scopeName, target);

            _ = _store.Remove(scopeName);
        }
    }

    /// <inheritdoc cref="IEventStore.Flush()" />
    public void Flush()
    {
        foreach ((string name, object target) in _store)
        {
            Unsubscribe(name, target);
        }

        _store.Clear();
    }

    /// <inheritdoc cref="Dispatchable.GetTarget{TEvent}(string)" />
    protected override object? GetTarget<TEvent>(string action)
    {
        if (_store.TryGetValue(action, out object? target))
        {
            return target;
        }

        TEvent? eventInstance = sp.GetService<TEvent>();

        if (eventInstance is not IComponentEvent componentEvent)
        {
            return null;
        }

        componentEvent.InitializeComponentEvents();

        return componentEvent;
    }

    /// <summary>
    /// Subscribes an event handler to a target object's event.
    /// </summary>
    /// <param name="target">The object containing the event to subscribe to.</param>
    /// <param name="teardownLogic">The teardown logic containing the event name and handler to subscribe.</param>
    /// <exception cref="ArgumentException">Thrown when the specified event name is not found on the target object.</exception>
    private static void Subscribe(object target, TeardownLogic teardownLogic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teardownLogic.ActionName);
        ArgumentNullException.ThrowIfNull(teardownLogic.EventHandler);

        Type targetType = target.GetType();
        PropertyInfo property = targetType.FindPropertyInfoByName(teardownLogic.ActionName, ignoreCase: true)
            ?? throw new ArgumentException($"Request {teardownLogic.ActionName} not found on target of type {targetType.Name}");
        property.SetValue(target, teardownLogic.EventHandler);
    }

    /// <summary>
    /// Subscribes an event handler to a specified action on the target object.
    /// </summary>
    /// <remarks>This method dynamically locates the property corresponding to the specified action on the
    /// target object and assigns the provided event handler to it. Ensure that the target object has a property
    /// matching the action name and that the property is compatible with the provided delegate.</remarks>
    /// <param name="target">The object on which the action is defined. Cannot be <see langword="null"/>.</param>
    /// <param name="action">The name of the action to subscribe to. This is case-insensitive.</param>
    /// <param name="eventHandler">The delegate representing the event handler to associate with the action. Cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown if the specified action is not found on the target object.</exception>
    private static void Subscribe(object target, string action, Delegate eventHandler)
    {
        Type targetType = target.GetType();
        PropertyInfo property = targetType.FindPropertyInfoByName(action, ignoreCase: true)
            ?? throw new ArgumentException($"Request {action} not found on target of type {targetType.Name}");
        property.SetValue(target, eventHandler);
    }

    /// <summary>
    /// Removes the subscription for the specified property on the target object.
    /// </summary>
    /// <remarks>This method attempts to locate the specified property on the target object and sets its value
    /// to <see langword="null"/>. The target object is resolved using the provided <paramref name="targetType"/> from
    /// the service provider. If the property is not found or the target object cannot be resolved, no action is
    /// taken.</remarks>
    /// <param name="name">The name of the property to unsubscribe from. This parameter is case-insensitive.</param>
    /// <param name="target">The target object containing the property to unsubscribe.</param>
    private static void Unsubscribe(string name, object target)
    {
        PropertyInfo? property = target?.GetType().FindPropertyInfoByName(name, ignoreCase: true);
        property?.SetValue(target, null);
    }

    /// <inheritdoc cref="IDisposable.Dispose(bool)" />
    protected override void Dispose(bool disposing)
    {
        if (!disposing || Disposed)
        {
            return;
        }

        Flush();

        base.Dispose(disposing);
    }
}
