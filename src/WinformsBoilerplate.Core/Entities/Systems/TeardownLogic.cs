namespace WinformsBoilerplate.Core.Entities.Systems;

/// <summary>
/// Represents a teardown logic that can be executed when a specific action occurs.
/// </summary>
/// <param name="action">the name of the action to listen for</param>
/// <param name="eventHandler">the event handler to execute when the action occurs</param>
public class TeardownLogic(string action, Delegate eventHandler)
{
    /// <summary>
    /// The name of the action to listen for.
    /// </summary>
    public string? ActionName { get; set; } = action;

    /// <summary>
    /// The event handler to execute when the action occurs.
    /// This can be any delegate type, such as an event handler or a custom delegate.
    /// </summary>
    public Delegate? EventHandler { get; set; } = eventHandler;
}
