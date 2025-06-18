using WinformsBoilerplate.Core.Entities.Systems;

namespace WinformsBoilerplate.Core.Abstractions.Stores;

/// <summary>
/// Defines a contract for managing event-related teardown logic storage and execution.
/// </summary>
public interface IEventStore : IDispatchable
{
    /// <summary>
    /// Adds teardown logic entries associated with a specific target object to the store.
    /// </summary>
    /// <typeparam name="T">The type of the entity associated with the teardown logic.</typeparam>
    /// <param name="teardownLogics">An array of <see cref="TeardownLogic"/> instances to be added.</param>
    /// <remarks>
    /// This method is typically used for adding multiple teardown logics at once.
    /// </remarks>
    void Add<T>(params ReadOnlySpan<TeardownLogic> teardownLogics) where T : class;

    /// <summary>
    /// Adds a single teardown logic entry associated with a specific target object to the store.
    /// </summary>
    /// <typeparam name="T">The type of the entity associated with the teardown logic.</typeparam>
    /// <param name="action">The action to be performed during teardown.</param>
    /// <param name="eventHandler">The event handler to be invoked during teardown.</param>
    /// <remarks>
    /// This method is typically used for adding a single teardown logic entry.
    /// </remarks>
    void Add<T>(string action, Delegate eventHandler) where T : class;

    /// <summary>
    /// Flushes all teardown logic entries for a specific target object.
    /// </summary>
    /// <typeparam name="T">The type of the entity associated with the teardown logic.</typeparam>
    /// <remarks>
    /// This method is typically used to clear all teardown logics for the specified entity type.
    /// </remarks>
    void Flush<T>();

    /// <summary>
    /// Flushes all teardown logic entries for all target objects.
    /// </summary>
    /// <remarks>
    /// This method is typically used to clear all teardown logics across all entities.
    /// </remarks>
    void Flush();
}
