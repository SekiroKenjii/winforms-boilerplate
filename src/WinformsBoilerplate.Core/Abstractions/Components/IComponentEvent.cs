namespace WinformsBoilerplate.Core.Abstractions.Components;

/// <summary>
/// Defines a contract for initializing event-handling logic within UI or form-based components.
/// </summary>
/// <remarks>
/// Implement this interface in classes responsible for configuring or wiring up event handlers,
/// ensuring a consistent approach to event initialization across different UI components.
/// </remarks>
public interface IComponentEvent
{
    /// <summary>
    /// Initializes and attaches event handlers necessary for component operation.
    /// </summary>
    /// <remarks>
    /// Call this method after constructing a component or loading the UI to ensure that events
    /// are properly registered. This method centralizes the logic for connecting UI actions to their handlers.
    /// </remarks>
    void InitializeFormEvents();
}
