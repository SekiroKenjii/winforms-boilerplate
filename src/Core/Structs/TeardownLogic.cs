namespace Core.Structs;

/// <summary>
/// Represents a pair of an action name and its corresponding event handler used for managing event teardown operations.
/// This structure is primarily used by EventStore to track and manage event subscriptions and their cleanup.
/// </summary>
/// <param name="ActionName">The name of the property or action on the target object that holds the event subscription.</param>
/// <param name="EventHandler">The delegate representing the event handler that will be removed from the target's event during teardown.
/// This handler is combined with existing handlers during subscription and removed during unsubscription.</param>
public record struct TeardownLogic(string ActionName, Delegate EventHandler);
