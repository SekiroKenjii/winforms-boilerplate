using Core.Structs;

namespace Core.Abstractions.Stores;

/// <summary>
/// Defines a contract for managing event-related teardown logic storage and execution.
/// </summary>
public interface IEventStore : IDispatchable
{
    /// <summary>
    /// Adds teardown logic entries associated with a specific target object.
    /// </summary>
    /// <param name="target">The object associated with the teardown logic.</param>
    /// <param name="teardownLogics">A collection of teardown logic entries to be stored.</param>
    void Add(object target, IEnumerable<TeardownLogic> teardownLogics);

    /// <summary>
    /// Executes and removes stored teardown logic for the specified target.
    /// If no target is specified, flushes all stored teardown logic.
    /// </summary>
    /// <param name="target">Optional. The specific target object whose teardown logic should be flushed.
    /// If null, all teardown logic will be flushed.</param>
    void Flush(object? target = null);
}
